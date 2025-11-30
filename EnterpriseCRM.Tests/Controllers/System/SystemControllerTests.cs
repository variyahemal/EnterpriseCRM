using Xunit;
using System.Net.Http;
using System.Net;
using EnterpriseCRM.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using EnterpriseCRM.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseCRM.Tests.Controllers.System
{
    public class SystemControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _adminToken;

        public SystemControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            using var scope = _factory.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            _adminToken = TestHelpers.GenerateJwtToken(
                "admin-id",
                "admin@test.com",
                new[] { "Admin" },
                config
            );
        }

        [Fact]
        public async Task GetSettings_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("api/v1/system/settings");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetSettings_WithAuth_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.GetAsync("api/v1/system/settings");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAuditLogs_WithAuth_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.GetAsync("api/v1/system/audit-logs");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

