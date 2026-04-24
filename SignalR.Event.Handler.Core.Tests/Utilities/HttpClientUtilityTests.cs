using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SignalR.Event.Handler.Core.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SignalR.Event.Handler.Core.Tests.Utilities
{
    [TestFixture]
    public class HttpClientUtilityTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private HttpClientUtility _httpClientUtility;

        [SetUp]
        public void SetUp()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _httpClientUtility = new HttpClientUtility(_httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        #region Test Model

        [Serializable]
        private class TestModel : ISerializable
        {
            [JsonProperty("id")]
            public string Id { get; set; } = string.Empty;

            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            // Default constructor for normal deserialization
            public TestModel()
            {
            }

            // Deserialization constructor required by ISerializable
            protected TestModel(SerializationInfo info, StreamingContext context)
            {
                Id = info.GetString("id") ?? string.Empty;
                Name = info.GetString("name") ?? string.Empty;
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("id", Id);
                info.AddValue("name", Name);
            }
        }

        #endregion

        #region ExecuteHttpGetSync Tests

        [Test]
        public void ExecuteHttpGetSync_ValidUrl_ReturnsDeserializedObject()
        {
            // Arrange
            var url = "https://api.test.com/data";
            var expectedModel = new TestModel { Id = "123", Name = "Test" };
            var responseContent = JsonConvert.SerializeObject(expectedModel);

            _mockHttpMessageHandler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var result = _httpClientUtility.ExecuteHttpGetSync<TestModel>(url);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(expectedModel.Id);
            result.Name.Should().Be(expectedModel.Name);
        }

        [Test]
        public void ExecuteHttpGetSync_NullUrl_ThrowsArgumentException()
        {
            // Act
            Action act = () => _httpClientUtility.ExecuteHttpGetSync<TestModel>(null!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("url");
        }

        [Test]
        public void ExecuteHttpGetSync_EmptyUrl_ThrowsArgumentException()
        {
            // Act
            Action act = () => _httpClientUtility.ExecuteHttpGetSync<TestModel>(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("url");
        }

        [Test]
        public void ExecuteHttpGetSync_WhitespaceUrl_ThrowsArgumentException()
        {
            // Act
            Action act = () => _httpClientUtility.ExecuteHttpGetSync<TestModel>("   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("url");
        }

        [Test]
        public void ExecuteHttpGetSync_HttpErrorResponse_ThrowsHttpRequestException()
        {
            // Arrange
            var url = "https://api.test.com/data";

            _mockHttpMessageHandler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Server error")
                });

            // Act
            Action act = () => _httpClientUtility.ExecuteHttpGetSync<TestModel>(url);

            // Assert
            act.Should().Throw<HttpRequestException>();
        }

        [Test]
        public void ExecuteHttpGetSync_InvalidJson_ThrowsJsonException()
        {
            // Arrange
            var url = "https://api.test.com/data";

            _mockHttpMessageHandler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{ invalid json")
                });

            // Act
            Action act = () => _httpClientUtility.ExecuteHttpGetSync<TestModel>(url);

            // Assert
            act.Should().Throw<JsonException>();
        }

        #endregion

        #region ExecuteHttpGetAsync Tests

        [Test]
        public async Task ExecuteHttpGetAsync_ValidUrl_ReturnsDeserializedObject()
        {
            // Arrange
            var url = "https://api.test.com/data";
            var expectedModel = new TestModel { Id = "456", Name = "AsyncTest" };
            var responseContent = JsonConvert.SerializeObject(expectedModel);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var result = await _httpClientUtility.ExecuteHttpGetAsync<TestModel>(url);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(expectedModel.Id);
            result.Name.Should().Be(expectedModel.Name);
        }

        [Test]
        public void ExecuteHttpGetAsync_NullUrl_ThrowsArgumentException()
        {
            // Act
            Func<Task> act = async () => await _httpClientUtility.ExecuteHttpGetAsync<TestModel>(null!);

            // Assert
            act.Should().ThrowAsync<ArgumentException>()
                .Result.Which.ParamName.Should().Be("url");
        }

        [Test]
        public void ExecuteHttpGetAsync_EmptyUrl_ThrowsArgumentException()
        {
            // Act
            Func<Task> act = async () => await _httpClientUtility.ExecuteHttpGetAsync<TestModel>(string.Empty);

            // Assert
            act.Should().ThrowAsync<ArgumentException>()
                .Result.Which.ParamName.Should().Be("url");
        }

        #endregion

        #region ExecuteHttpPostSync Tests

        [Test]
        public void ExecuteHttpPostSync_ValidRequest_ReturnsDeserializedObject()
        {
            // Arrange
            var url = "https://api.test.com/data";
            var requestModel = new TestModel { Id = "789", Name = "PostRequest" };
            var responseModel = new TestModel { Id = "789", Name = "PostResponse" };
            var responseContent = JsonConvert.SerializeObject(responseModel);

            _mockHttpMessageHandler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var result = _httpClientUtility.ExecuteHttpPostSync<TestModel>(url, requestModel);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(responseModel.Id);
            result.Name.Should().Be(responseModel.Name);
        }

        [Test]
        public void ExecuteHttpPostSync_NullUrl_ThrowsArgumentException()
        {
            // Arrange
            var requestModel = new TestModel();

            // Act
            Action act = () => _httpClientUtility.ExecuteHttpPostSync<TestModel>(null!, requestModel);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("url");
        }

        [Test]
        public void ExecuteHttpPostSync_NullRequestObject_ThrowsArgumentNullException()
        {
            // Arrange
            var url = "https://api.test.com/data";

            // Act
            Action act = () => _httpClientUtility.ExecuteHttpPostSync<TestModel>(url, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("requestObject");
        }

        #endregion

        #region ExecuteHttpPostAsync Tests

        [Test]
        public async Task ExecuteHttpPostAsync_ValidRequest_ReturnsDeserializedObject()
        {
            // Arrange
            var url = "https://api.test.com/data";
            var requestModel = new TestModel { Id = "101", Name = "AsyncPostRequest" };
            var responseModel = new TestModel { Id = "101", Name = "AsyncPostResponse" };
            var responseContent = JsonConvert.SerializeObject(responseModel);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var result = await _httpClientUtility.ExecuteHttpPostAsync<TestModel>(url, requestModel);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(responseModel.Id);
            result.Name.Should().Be(responseModel.Name);
        }

        [Test]
        public void ExecuteHttpPostAsync_NullUrl_ThrowsArgumentException()
        {
            // Arrange
            var requestModel = new TestModel();

            // Act
            Func<Task> act = async () => await _httpClientUtility.ExecuteHttpPostAsync<TestModel>(null!, requestModel);

            // Assert
            act.Should().ThrowAsync<ArgumentException>()
                .Result.Which.ParamName.Should().Be("url");
        }

        [Test]
        public void ExecuteHttpPostAsync_NullRequestObject_ThrowsArgumentNullException()
        {
            // Arrange
            var url = "https://api.test.com/data";

            // Act
            Func<Task> act = async () => await _httpClientUtility.ExecuteHttpPostAsync<TestModel>(url, null!);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .Result.Which.ParamName.Should().Be("requestObject");
        }

        #endregion
    }
}
