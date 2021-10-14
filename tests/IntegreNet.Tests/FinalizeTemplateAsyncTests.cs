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
    public class FinalizeTemplateAsyncTests
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
        public async Task FinalizeTemplateAsync_WhenSuccessful_ReturnsTemplate()
        {
            // arrange
            _handlerMock.SetupSendAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

            // act
            await _integre.FinalizeTemplateAsync("123");

            // assert
            _handlerMock.VerifySendAsync(HttpMethod.Put, "http://localhost/api/v1/templates/123", null, Times.Once());
        }

        [Test]
        public void FinalizeTemplateAsync_WhenUnexpectedStatusCodeIsReturned_Throws()
        {
            // arrange
            _handlerMock.SetupSendAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

            // act
            var exception = Assert.ThrowsAsync<IntegreException>(async () => await _integre.FinalizeTemplateAsync("123"));

            // assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo("Unexpected status code: BadRequest"));

            _handlerMock.VerifySendAsync(HttpMethod.Put, "http://localhost/api/v1/templates/123", null, Times.Once());
        }

        [Test]
        public void FinalizeTemplateAsync_WhenUnexpectedExceptionOccurs_Throws()
        {
            // arrange
            _handlerMock.SetupSendAsync(new Exception("Unexpected error"));

            // act
            var exception = Assert.ThrowsAsync<IntegreException>(async () => await _integre.FinalizeTemplateAsync("123"));

            // assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo("Unexpected error occurred"));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException.Message, Is.EqualTo("Unexpected error"));

            _handlerMock.VerifySendAsync(HttpMethod.Put, "http://localhost/api/v1/templates/123", null, Times.Once());
        }

        [TearDown]
        public void TearDown()
        {
            _handlerMock.VerifyAll();
        }
    }
}