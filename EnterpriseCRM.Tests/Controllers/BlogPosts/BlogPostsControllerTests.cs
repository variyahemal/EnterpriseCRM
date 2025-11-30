using Xunit;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using EnterpriseCRM.API.DTOs.BlogPosts;
using EnterpriseCRM.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using EnterpriseCRM.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseCRM.Tests.Controllers.BlogPosts
{
    public class BlogPostsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _editorToken;

        public BlogPostsControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetPublicBlogPosts_WithoutAuth_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("api/v1/blogposts/public");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateBlogPost_WithAuth_ReturnsCreated()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _editorToken);

            var createDto = new CreateBlogPostDto
            {
                Title = "Test Blog Post",
                Content = "This is test content"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/blogposts", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<BlogPostDto>();
            Assert.NotNull(result);
            Assert.Equal(createDto.Title, result.Title);
        }

        [Fact]
        public async Task PublishBlogPost_WithAuth_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new global::System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _editorToken);

            var createDto = new CreateBlogPostDto
            {
                Title = "Publish Test",
                Content = "Content to publish"
            };
            var createResponse = await _client.PostAsJsonAsync("api/v1/blogposts", createDto);
            var createdPost = await createResponse.Content.ReadFromJsonAsync<BlogPostDto>();

            // Act
            var response = await _client.PutAsync($"api/v1/blogposts/{createdPost!.Id}/publish?publish=true", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<BlogPostDto>();
            Assert.NotNull(result);
            Assert.True(result.IsPublished);
        }
    }
}

