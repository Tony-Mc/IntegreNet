using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IntegreNet.Exceptions;
using IntegreNet.Tests.Extensions;
using Moq;
using NUnit.Framework;

namespace IntegreNet.Tests
{
    public class InitializeTemplateTests
    {
        private IntegreSql _integre;
        private Mock<HttpClientHandler> _handlerMock;

        [SetUp]
        public void SetUp()
        {
            _handlerMock = new Mock<HttpClientHandler>(MockBehavior.Strict);

            _integre = new IntegreSql("http://localhost/api/", _handlerMock.Object);
        }

        [Test]
        public async Task InitializeTemplateAsync_WhenSuccessful_ReturnsTemplate()
        {
            // arrange
            _handlerMock.SetupSendAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{
                    ""database"": {
                        ""templateHash"": ""123"",
                        ""config"": {
                            ""host"": ""hostname"",
                            ""port"": 5432,
                            ""username"": ""dbuser"",
                            ""password"": ""secret"",
                            ""database"": ""integre-123""
                        }
                    }
                }")
            });

            // act
            var template = await _integre.InitializeTemplateAsync("123");

            // assert
            Assert.That(template, Is.Not.Null);

            var database = template.Database;
            Assert.That(database, Is.Not.Null);
            Assert.That(database.Hash, Is.EqualTo("123"));

            var config = database.Config;
            Assert.That(config, Is.Not.Null);
            Assert.That(config.Host, Is.EqualTo("hostname"));
            Assert.That(config.Port, Is.EqualTo(5432));
            Assert.That(config.Username, Is.EqualTo("dbuser"));
            Assert.That(config.Password, Is.EqualTo("secret"));
            Assert.That(config.Database, Is.EqualTo("integre-123"));

            _handlerMock.VerifySendAsync(HttpMethod.Post, "http://localhost/api/v1/templates", @"{""hash"":""123""}", Times.Once());
        }

        [Test]
        public void InitializeTemplateAsync_WhenTemplateLocked_Throws()
        {
            // arrange
            _handlerMock.SetupSendAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Locked
            });

            // act
            var exception = Assert.ThrowsAsync<TemplateLockedException>(async () => await _integre.InitializeTemplateAsync("123"));

            // assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo("Some other process has already recreated a PostgreSQL template database for this hash (or is currently doing it), you can just consider the template ready at this point"));

            _handlerMock.VerifySendAsync(HttpMethod.Post, "http://localhost/api/v1/templates", @"{""hash"":""123""}", Times.Once());
        }

        [Test]
        public void InitializeTemplateAsync_WhenUnexpectedStatusCodeIsReturned_Throws()
        {
            // arrange
            _handlerMock.SetupSendAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

            // act
            var exception = Assert.ThrowsAsync<IntegreException>(async () => await _integre.InitializeTemplateAsync("123"));

            // assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo("Unexpected status code: BadRequest"));

            _handlerMock.VerifySendAsync(HttpMethod.Post, "http://localhost/api/v1/templates", @"{""hash"":""123""}", Times.Once());
        }

        [Test]
        public void InitializeTemplateAsync_WhenUnexpectedExceptionOccurs_Throws()
        {
            // arrange
            _handlerMock.SetupSendAsync(new Exception("Unexpected error"));

            // act
            var exception = Assert.ThrowsAsync<IntegreException>(async () => await _integre.InitializeTemplateAsync("123"));

            // assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo("Unexpected error occurred"));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException.Message, Is.EqualTo("Unexpected error"));

            _handlerMock.VerifySendAsync(HttpMethod.Post, "http://localhost/api/v1/templates", @"{""hash"":""123""}", Times.Once());
        }

        [TearDown]
        public void TearDown()
        {
            _handlerMock.VerifyAll();
        }
    }
}