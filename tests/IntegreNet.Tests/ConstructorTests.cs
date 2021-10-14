using System;
using System.Net.Http;
using Moq;
using NUnit.Framework;

namespace IntegreNet.Tests
{
    [TestFixture]
    public class ConstructorTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidBaseUrl_Throws(string baseUrl)
        {
            Assert.Throws<ArgumentException>(() => new IntegreSql(baseUrl));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ConstructorWithHandler_WithInvalidBaseUrl_Throws(string baseUrl)
        {
            Assert.Throws<ArgumentException>(() => new IntegreSql(baseUrl, It.IsAny<HttpClientHandler>()));
        }

        [Test]
        public void ConstructorWithHandler_WithNullHandler_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new IntegreSql("http://localhost/api/", null));
        }
    }
}