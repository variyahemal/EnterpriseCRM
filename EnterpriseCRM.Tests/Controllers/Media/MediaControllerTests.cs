using Xunit;
using System.Net.Http;
using System.Net;
using EnterpriseCRM.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using EnterpriseCRM.Tests;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseCRM.Tests.Controllers.Media
{
    public class MediaControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _editorToken;

        public MediaControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            using var scope = _factory.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            _editorToken = TestHelpers.GenerateJwtToken(
                "editor-id",
                "editor@test.com",
                new[] { "Editor" },
                config
            );
        }

        [Fact]
        public async Task ListMedia_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("api/v1/media");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ListMedia_WithAuth_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _editorToken);

            // Act
            var response = await _client.GetAsync("api/v1/media");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UploadMedia_WithoutFile_ReturnsBadRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _editorToken);

            var content = new MultipartFormDataContent();

            // Act
            var response = await _client.PostAsync("api/v1/media/upload", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

