using Xunit;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using EnterpriseCRM.API.DTOs.Contacts;
using EnterpriseCRM.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using EnterpriseCRM.Tests;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseCRM.Tests.Controllers.Contact
{
    public class ContactsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _adminToken;

        public ContactsControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetContacts_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("api/v1/contacts");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetContacts_WithAuth_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.GetAsync("api/v1/contacts");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateContact_WithValidData_ReturnsCreated()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            var createDto = new CreateContactDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                Company = "Test Company"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/contacts", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ContactDto>();
            Assert.NotNull(result);
            Assert.Equal(createDto.FirstName, result.FirstName);
            Assert.Equal(createDto.Email, result.Email);
        }

        [Fact]
        public async Task GetContact_WithValidId_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            var createDto = new CreateContactDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "123-456-7891"
            };
            var createResponse = await _client.PostAsJsonAsync("api/v1/contacts", createDto);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdContact = await createResponse.Content.ReadFromJsonAsync<ContactDto>();
            Assert.NotNull(createdContact);

            // Act
            var response = await _client.GetAsync($"api/v1/contacts/{createdContact!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ContactDto>();
            Assert.NotNull(result);
            Assert.Equal(createdContact.Id, result.Id);
        }

        [Fact]
        public async Task UpdateContact_WithValidData_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            var createDto = new CreateContactDto
            {
                FirstName = "Update",
                LastName = "Test",
                Email = "update@example.com"
            };
            var createResponse = await _client.PostAsJsonAsync("api/v1/contacts", createDto);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdContact = await createResponse.Content.ReadFromJsonAsync<ContactDto>();
            Assert.NotNull(createdContact);

            var updateDto = new UpdateContactDto
            {
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@example.com"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"api/v1/contacts/{createdContact!.Id}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            if (response.Content.Headers.ContentLength > 0)
            {
                var result = await response.Content.ReadFromJsonAsync<ContactDto>();
                Assert.NotNull(result);
                Assert.Equal(updateDto.FirstName, result.FirstName);
            }
        }

        [Fact]
        public async Task DeleteContact_AsAdmin_ReturnsNoContent()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

            var createDto = new CreateContactDto
            {
                FirstName = "Delete",
                LastName = "Test",
                Email = "delete@example.com"
            };
            var createResponse = await _client.PostAsJsonAsync("api/v1/contacts", createDto);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdContact = await createResponse.Content.ReadFromJsonAsync<ContactDto>();
            Assert.NotNull(createdContact);

            // Act
            var response = await _client.DeleteAsync($"api/v1/contacts/{createdContact!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}

