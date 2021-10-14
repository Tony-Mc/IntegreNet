using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace IntegreNet.Tests.Extensions
{
    public static class HttpClientHandlerMockExtensions
    {
        public static void SetupSendAsync(this Mock<HttpClientHandler> mock, HttpResponseMessage value) =>
            mock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(value)
                .Verifiable();

        public static void VerifySendAsync(this Mock<HttpClientHandler> mock, HttpMethod method, string uri, string body, Times times) =>
            mock
                .Protected()
                .Verify("SendAsync", times,
                    ItExpr.Is<HttpRequestMessage>(req => Eq(method, req.Method) && Eq(new Uri(uri), req.RequestUri) && Eq(body, req.Content != null ? req.Content.ReadAsStringAsync().Result : null)), ItExpr.IsAny<CancellationToken>());

        public static void SetupSendAsync(this Mock<HttpClientHandler> mock, Exception exception) =>
            mock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(exception)
                .Verifiable();


        private static bool Eq(object expected, object actual)
        {
            Assert.That(actual, Is.EqualTo(expected));

            return true;
        }
    }
}