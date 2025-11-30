using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using EnterpriseCRM.API;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace EnterpriseCRM.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        public Mock<IUnitOfWork> MockUnitOfWork { get; private set; }
        public Mock<IContactRepository> MockContactRepository { get; private set; }
        public Mock<ILeadRepository> MockLeadRepository { get; private set; }
        public Mock<UserManager<AppUser>> MockUserManager { get; private set; }
        public Mock<RoleManager<AppRole>> MockRoleManager { get; private set; }
        public Mock<IConfiguration> MockConfiguration { get; private set; }

        public CustomWebApplicationFactory()
        {
            MockUnitOfWork = new Mock<IUnitOfWork>();
            MockContactRepository = new Mock<IContactRepository>();
            MockLeadRepository = new Mock<ILeadRepository>();

            var userStoreMock = new Mock<IUserStore<AppUser>>();
            MockUserManager = new Mock<UserManager<AppUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<AppRole>>();
            MockRoleManager = new Mock<RoleManager<AppRole>>(
                roleStoreMock.Object, null, null, null, null);

            MockConfiguration = new Mock<IConfiguration>();
            SetupMockConfiguration();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"JwtSettings:SecurityKey", "supersecretkeyforenterprisecrmtestingpurposesonly12345"},
                    {"JwtSettings:ValidIssuer", "EnterpriseCRM-Test-Issuer"},
                    {"JwtSettings:ValidAudience", "EnterpriseCRM-Test-Audience"},
                    {"JwtSettings:ExpiryMinutes", "60"},
                    {"JwtSettings:RefreshTokenValidityInDays", "7"},
                    {"ConnectionStrings:DefaultConnection", "TestConnection"}
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove the existing ApplicationDbContext registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<EnterpriseCRM.Infrastructure.Persistence.ApplicationDbContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var dbName = $"TestDatabase_{Guid.NewGuid()}";
                
                // Add in-memory database for testing - Identity will use this automatically
                services.AddDbContext<EnterpriseCRM.Infrastructure.Persistence.ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                    options.EnableSensitiveDataLogging();
                });
            });
        }

        private void SetupMockConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JwtSettings:SecurityKey", "supersecretkeyforenterprisecrmtestingpurposesonly12345"},
                {"JwtSettings:ValidIssuer", "EnterpriseCRM-Test-Issuer"},
                {"JwtSettings:ValidAudience", "EnterpriseCRM-Test-Audience"},
                {"JwtSettings:ExpiryMinutes", "60"},
                {"JwtSettings:RefreshTokenValidityInDays", "7"}
            };

            var jwtSection = new ConfigurationSectionStub(inMemorySettings);
            MockConfiguration.Setup(x => x.GetSection("JwtSettings"))
                .Returns(jwtSection);
            MockConfiguration.Setup(x => x["JwtSettings:SecurityKey"])
                .Returns(inMemorySettings["JwtSettings:SecurityKey"]);
            MockConfiguration.Setup(x => x["JwtSettings:ValidIssuer"])
                .Returns(inMemorySettings["JwtSettings:ValidIssuer"]);
            MockConfiguration.Setup(x => x["JwtSettings:ValidAudience"])
                .Returns(inMemorySettings["JwtSettings:ValidAudience"]);
            MockConfiguration.Setup(x => x["JwtSettings:ExpiryMinutes"])
                .Returns(inMemorySettings["JwtSettings:ExpiryMinutes"]);
            MockConfiguration.Setup(x => x["JwtSettings:RefreshTokenValidityInDays"])
                .Returns(inMemorySettings["JwtSettings:RefreshTokenValidityInDays"]);
        }

        // Helper class for mocking IConfigurationSection
        private class ConfigurationSectionStub : IConfigurationSection
        {
            private readonly Dictionary<string, string> _settings;
            public string Key => "JwtSettings";
            public string Path => "JwtSettings";
            public string? Value { get; set; }

            public ConfigurationSectionStub(Dictionary<string, string> settings)
            {
                _settings = settings;
            }

            public IConfigurationSection GetSection(string key)
            {
                if (_settings.TryGetValue(Key + ":" + key, out var value))
                {
                    return new ConfigurationSectionStub(new Dictionary<string, string> { { key, value } }) { Value = value };
                }
                return new ConfigurationSectionStub(new Dictionary<string, string>());
            }

            public IEnumerable<IConfigurationSection> GetChildren()
            {
                return _settings.Where(s => s.Key.StartsWith(Key + ":"))
                                .Select(s => new ConfigurationSectionStub(new Dictionary<string, string> { { s.Key.Substring(Key.Length + 1), s.Value } }) { Value = s.Value });
            }

            public IChangeToken GetReloadToken() => new ConfigurationReloadToken();

            public string? this[string key]
            {
                get => _settings.TryGetValue(Key + ":" + key, out var value) ? value : null;
                set => _settings[Key + ":" + key] = value ?? string.Empty;
            }
        }
    }
}