using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Boundary.Request;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;

using Xunit;

namespace TechRadarApi.Tests.V1.E2ETests
{
    [Collection("DynamoDb collection")]
    public class PostTechnologyEndToEndTests : IDisposable
    {

        private readonly Fixture _fixture = new Fixture();
        public TechnologyDbEntity Technology { get; private set; }
        private readonly IDynamoDbFixture _dbFixture;
        private readonly HttpClient _client;
        private readonly List<Action> _cleanupActions = new List<Action>();

        public PostTechnologyEndToEndTests(AwsMockWebApplicationFactory<Startup> appFactory)
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

        [Fact]
        public async Task PostTechnologyReturnsCreatedResponse()
        {
            // Arrange
            var request = _fixture.Create<CreateTechnologyRequest>();
            var uri = new Uri("api/v1/technologies", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PostAsync(uri, content).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiTechnology = JsonConvert.DeserializeObject<TechnologyResponseObject>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var dbTechnology = await _dbFixture.DynamoDbContext.LoadAsync<TechnologyDbEntity>(apiTechnology.Id)
                                                               .ConfigureAwait(false);
            request.Should().BeEquivalentTo(dbTechnology, c => c.Excluding(x => x.Id));
            // cleanup
            _cleanupActions.Add(() => _dbFixture.DynamoDbContext.DeleteAsync<TechnologyDbEntity>(apiTechnology.Id));
            content.Dispose();
        }

        // TODO: Add validation tests
    }
}
