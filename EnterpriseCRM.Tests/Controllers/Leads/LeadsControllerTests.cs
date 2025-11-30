using Xunit;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using EnterpriseCRM.API.DTOs.Leads;
using EnterpriseCRM.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using EnterpriseCRM.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseCRM.Tests.Controllers.Leads
{
    public class LeadsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _adminToken;

        public LeadsControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetLeads_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("api/v1/leads");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetLeads_WithAuth_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.GetAsync("api/v1/leads");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateLead_WithValidData_ReturnsCreated()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            var createDto = new CreateLeadDto
            {
                FirstName = "Lead",
                LastName = "Test",
                Email = "lead@example.com",
                Phone = "123-456-7890",
                Company = "Test Company",
                Source = "Website"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/leads", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<LeadDto>();
            Assert.NotNull(result);
            Assert.Equal(createDto.FirstName, result.FirstName);
        }

        [Fact]
        public async Task GetLead_WithValidId_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            var createDto = new CreateLeadDto
            {
                FirstName = "Get",
                LastName = "Lead",
                Email = "getlead@example.com"
            };
            var createResponse = await _client.PostAsJsonAsync("api/v1/leads", createDto);
            var createdLead = await createResponse.Content.ReadFromJsonAsync<LeadDto>();

            // Act
            var response = await _client.GetAsync($"api/v1/leads/{createdLead!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<LeadDto>();
            Assert.NotNull(result);
            Assert.Equal(createdLead.Id, result.Id);
        }

        [Fact]
        public async Task UpdateLeadStatus_WithValidData_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            var createDto = new CreateLeadDto
            {
                FirstName = "Status",
                LastName = "Update",
                Email = "status@example.com"
            };
            var createResponse = await _client.PostAsJsonAsync("api/v1/leads", createDto);
            var createdLead = await createResponse.Content.ReadFromJsonAsync<LeadDto>();

            var updateDto = new UpdateLeadStatusDto
            {
                Status = "Qualified"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"api/v1/leads/{createdLead!.Id}/status", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<LeadDto>();
            Assert.NotNull(result);
            Assert.Equal(updateDto.Status, result.Status);
        }
    }
}

