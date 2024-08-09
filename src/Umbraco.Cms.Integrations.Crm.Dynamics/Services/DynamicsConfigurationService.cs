using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Scoping;
using System;
using Umbraco.Cms.Integrations.Crm.Dynamics.Migrations;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public class DynamicsConfigurationService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<DynamicsService> _logger;

        public DynamicsConfigurationService(IScopeProvider scopeProvider, ILogger<DynamicsService> logger)
        {
            _scopeProvider = scopeProvider;

            _logger = logger;
        }

        public string AddorUpdateOAuthConfiguration(string accessToken, string userId, string fullName)
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database
                        .Query<DynamicsOAuthConfigurationTable>()
                        .FirstOrDefault(p => p.UserId == userId) ?? new DynamicsOAuthConfigurationTable { UserId = userId, FullName = fullName };

                    entity.AccessToken = accessToken;

                    if (entity.Id == 0)
                        scope.Database.Insert(entity);
                    else
                        scope.Database.Update(entity);

                    scope.Complete();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                var message = $"An error has occurred while adding the OAuth configuration: {ex.Message}";

                _logger.LogError(message);
                return message;
            }
        }

        public OAuthConfigurationDto GetOAuthConfiguration()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var entity = scope.Database
                    .Query<DynamicsOAuthConfigurationTable>()
                    .FirstOrDefault();

                return entity != null
                    ? new OAuthConfigurationDto { Id = entity.Id, AccessToken = entity.AccessToken, UserId = entity.UserId, FullName = entity.FullName } 
                    : new OAuthConfigurationDto { Id = 2, AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1HTHFqOThWTkxvWGFGZnBKQ0JwZ0I0SmFLcyIsImtpZCI6Ik1HTHFqOThWTkxvWGFGZnBKQ0JwZ0I0SmFLcyJ9.eyJhdWQiOiJodHRwczovL29yZzA5MmMyMzUwLmNybTQuZHluYW1pY3MuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvZTQ3OTRlYTMtMjVjZS00NjUwLWExMzAtYWM5ZmE0MzMzYWQ2LyIsImlhdCI6MTcyMzAyNDMxMCwibmJmIjoxNzIzMDI0MzEwLCJleHAiOjE3MjMwMjk4MjUsImFjY3QiOjAsImFjciI6IjEiLCJhaW8iOiJBVlFBcS84WEFBQUFuQjJ6eXk0YjhqV2lPb3FGemhqUGhQSTF2M1MyeGxWSkhuNnVCSFhicmVianEwcHJRRG43THJybXEvWGJTMHBxb0UvRzF1MHd3MHNYandXNDVkc25uWERFQXEzNlFUOUpkMXZxYWtRSzhnZz0iLCJhbXIiOlsicHdkIiwibWZhIl0sImFwcGlkIjoiODEzYzVhNjUtY2ZkNi00OGQ2LTg5MjgtZGZmZTAyYWFmNjFhIiwiYXBwaWRhY3IiOiIxIiwiZmFtaWx5X25hbWUiOiJEb2FuIFRoYW5oIiwiZ2l2ZW5fbmFtZSI6Ik5ndXllbiIsImlkdHlwIjoidXNlciIsImlwYWRkciI6IjEwMy4zNy4yOS45MCIsImxvZ2luX2hpbnQiOiJPLkNpUXlPVGxoTVRBd01DMHhPVFEzTFRRek5tTXRPVGc1WXkweVlqWmpOVGRtTXpWbE1tSVNKR1UwTnprMFpXRXpMVEkxWTJVdE5EWTFNQzFoTVRNd0xXRmpPV1poTkRNek0yRmtOaG9PYm1SMFFIVnRZbkpoWTI4dVpHc2c4UUU9IiwibmFtZSI6Ik5ndXllbiBEb2FuIFRoYW5oIiwib2lkIjoiMjk5YTEwMDAtMTk0Ny00MzZjLTk4OWMtMmI2YzU3ZjM1ZTJiIiwicHVpZCI6IjEwMDMyMDAyRTYyQkFDOUIiLCJyaCI6IjAuQVNFQW8wNTU1TTRsVUVhaE1LeWZwRE02MWdjQUFBQUFBQUFBd0FBQUFBQUFBQUFZQWYwLiIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6Il9UVlV6SmctVlNoU1FJekhRTEl0Wk85anNabTdsSHN5czA0dzJlOFI3MzQiLCJ0ZW5hbnRfcmVnaW9uX3Njb3BlIjoiRVUiLCJ0aWQiOiJlNDc5NGVhMy0yNWNlLTQ2NTAtYTEzMC1hYzlmYTQzMzNhZDYiLCJ1bmlxdWVfbmFtZSI6Im5kdEB1bWJyYWNvLmRrIiwidXBuIjoibmR0QHVtYnJhY28uZGsiLCJ1dGkiOiJwR21Md3JTZDNrTzF6OXdoeEJNU0FBIiwidmVyIjoiMS4wIiwid2lkcyI6WyJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXSwieG1zX2lkcmVsIjoiMTggMSJ9.gDRlSprCJnaSgME7iaW2OHGLdZ05FBXzUQ4rbbH9elIZUNBm0U1aOo8-OhOKYriiBnqWWpsU4ZHIkaxZRr9blJoEGh9iqlfaUJMgRCgHplGC8mJobtdkbTZr8Wna7mcMRlAkzu-dftJeD3NlWK2gXvmkPAgp083OPSCf9oMcxRYeEEz0eKGHgsawg1Y-06KDB3et4jXEOyMWAMIVUe2s42pzRuAApDz8XSjbXA1laGt05GuNIxoy0kKVEc1aBbeEcO5bw2bs2qNm8uMxxxBFy66fd1WEgQCLH-4r7OKR6SLrOqSJo8ICtAUd4uorje-qNW8EblPa64l5LJhkGfNS_A", 
                        UserId = "9fb05178-0d53-ef11-bfe3-000d3a674175", FullName = "Nguyen Doan Thanh"
                    };
            }
        }

        public string GetSystemUserFullName()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var entity = scope.Database
                    .Query<DynamicsOAuthConfigurationTable>()
                    .FirstOrDefault();

                return entity != null ? entity.FullName : string.Empty;
            }
        }

        public string Delete()
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    var entity = scope.Database
                        .Query<DynamicsOAuthConfigurationTable>()
                        .FirstOrDefault();
                    if (entity != null)
                    {
                        scope.Database.Delete(entity);
                    }

                    scope.Complete();
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                var message = $"An error has occurred while deleting the OAuth configuration: {e.Message}";

                _logger.LogError(message);

                return message;
            }
        }

    }
}
