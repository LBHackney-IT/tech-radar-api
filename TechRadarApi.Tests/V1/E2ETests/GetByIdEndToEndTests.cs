using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using Xunit;

namespace TechRadarApi.Tests.V1.E2ETests
{
    [Collection("DynamoDb collection")]
    public class GetByIdEndToEndTests : IDisposable
    {

        private readonly Fixture _fixture = new Fixture();
        public TechnologyDbEntity Technology { get; private set; }
        private readonly IDynamoDbFixture _dbFixture;
        private readonly HttpClient _client;
        private readonly List<Action> _cleanupActions = new List<Action>();

        public GetByIdEndToEndTests(AwsMockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _client = appFactory.Client;
        }

        public void Dispose()
        {
            _dbFixture?.Dispose();
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

        [Fact]
        public async Task GetTechnologyByValidIdReturnsOKResponse()
        {
            // Arrange
            var entity = _fixture.Create<Technology>();
            await _dbFixture.SaveEntityAsync<TechnologyDbEntity>(entity.ToDatabase()).ConfigureAwait(false);
            var uri = new Uri($"api/v1/technologies/{entity.Id}", UriKind.Relative);

            // Act
            var response = await _client.GetAsync(uri).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<Technology>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            apiEntity.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public async Task GetTechnologyByInvalidIdReturnsBadRequestResponse()
        {
            // Arrange
            var badId = _fixture.Create<int>(); // wrong type, should be a guid
            var uri = new Uri($"api/v1/technologies/{badId}", UriKind.Relative);
            // Act
            var response = await _client.GetAsync(uri).ConfigureAwait(false);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetTechnologyByNonExistentIdReturnsNotFoundResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var uri = new Uri($"api/v1/technologies/{id}", UriKind.Relative);
            // Act
            var response = await _client.GetAsync(uri).ConfigureAwait(false);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
