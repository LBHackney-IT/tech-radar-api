using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hackney.Core.Testing.DynamoDb;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using Xunit;

namespace TechRadarApi.Tests.V1.E2ETests
{
    [Collection("DynamoDb Collection")]
    public class DeleteByIdEndToEndTests : IDisposable
    {

        private readonly Fixture _fixture = new Fixture();
        private readonly IDynamoDbFixture _dbFixture;
        private readonly HttpClient _client;
        private readonly List<Action> _cleanupActions = new List<Action>();

        public DeleteByIdEndToEndTests(AwsMockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _client = appFactory.Client;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var action in _cleanupActions)
                    action();

                _disposed = true;
            }
        }

        private Technology ConstructTestEntity()
        {
            var entity = _fixture.Create<Technology>();
            return entity;
        }


        [Fact]
        public async Task DeleteTechnologyByIdDeletesTechnology()
        {
            // Arrange
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";
            var entity = ConstructTestEntity();
            await _dbFixture.SaveEntityAsync(entity.ToDatabase()).ConfigureAwait(false);
            var uri = new Uri($"api/v1/technologies/{entity.Id}", UriKind.Relative);
            var bodyParameters = _fixture.Create<TechnologyResponseObject>();

            // Act

            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Headers.Add("Authorization", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(bodyParameters), Encoding.UTF8, "application/json");
            var httpResponse = await _client.SendAsync(message).ConfigureAwait(false);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponse.Content.Equals(typeof(TechnologyResponseObject));
            message.Dispose();
        }

        [Fact]
        public async Task DeleteTechnologyByIdReturnsNotFoundResponse()
        {
            // Arrange
            var entity = ConstructTestEntity();
            var uri = new Uri($"api/v1/technologies/{entity.Id}", UriKind.Relative);
            var bodyParameters = _fixture.Create<TechnologyResponseObject>();

            // Act

            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(bodyParameters), Encoding.UTF8, "application/json");
            var httpResponse = await _client.SendAsync(message).ConfigureAwait(false);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            message.Dispose();
        }
    }
}
