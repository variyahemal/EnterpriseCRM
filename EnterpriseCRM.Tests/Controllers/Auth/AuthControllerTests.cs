using Xunit;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using EnterpriseCRM.API.DTOs.Auth;
using EnterpriseCRM.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EnterpriseCRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EnterpriseCRM.Domain;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCRM.Tests.Controllers.Auth
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOk()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Email = "test@example.com",
                Password = "TestPassword123!",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/auth/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal(registerDto.Email, result.Email);
        }

        [Fact]
        public async Task Register_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Email = "",
                Password = ""
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/auth/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange - First register a user
            var registerDto = new RegisterUserDto
            {
                Email = "login@example.com",
                Password = "TestPassword123!",
                FirstName = "Login",
                LastName = "User"
            };
            await _client.PostAsJsonAsync("api/v1/auth/register", registerDto);

            var loginDto = new LoginUserDto
            {
                Email = "login@example.com",
                Password = "TestPassword123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "nonexistent@example.com",
                Password = "WrongPassword"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsOk()
        {
            // Arrange - Register and get tokens
            var registerDto = new RegisterUserDto
            {
                Email = "refresh@example.com",
                Password = "TestPassword123!",
                FirstName = "Refresh",
                LastName = "User"
            };
            var registerResponse = await _client.PostAsJsonAsync("api/v1/auth/register", registerDto);
            var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResultDto>();

            var refreshDto = new RefreshTokenDto
            {
                RefreshToken = authResult!.RefreshToken
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/auth/refresh-token", refreshDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var refreshDto = new RefreshTokenDto
            {
                RefreshToken = "invalid-token"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/auth/refresh-token", refreshDto);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}

