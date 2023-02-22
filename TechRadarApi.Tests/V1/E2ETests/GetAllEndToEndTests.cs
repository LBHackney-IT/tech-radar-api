using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Newtonsoft.Json;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using Xunit;

namespace TechRadarApi.Tests.V1.E2ETests
{
    [Collection("DynamoDb Collection")]
    public class GetAllEndToEndTests : IDisposable
    {

        private readonly Fixture _fixture = new Fixture();
        public TechnologyDbEntity Technology { get; private set; }
        private readonly IDynamoDbFixture _dbFixture;
        private readonly HttpClient _client;
        private readonly List<Action> _cleanupActions = new List<Action>();

        public GetAllEndToEndTests(AwsMockWebApplicationFactory<Startup> appFactory)
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

        private async Task<List<Technology>> CreateAndInsertTechnologies(int count)
        {
            var technologies = _fixture.CreateMany<TechnologyDbEntity>(count).ToList();
            var output = new List<Technology>();

            var tasks = technologies.Select(async technology =>
            {
                await _dbFixture.DynamoDbContext.SaveAsync(technology).ConfigureAwait(false);
                _cleanupActions.Add((() => _dbFixture.DynamoDbContext.DeleteAsync(technology).GetAwaiter().GetResult()));
                output.Add(technology.ToDomain());
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return output;
        }


        [Fact]
        public async Task GetAllTechnologiesReturnsOkResponse()
        {
            // Arrange
            var technologies = await CreateAndInsertTechnologies(5).ConfigureAwait(false);
            var expectedResponse = new TechnologyResponseObjectList
            {
                Technologies = technologies.Select(x => x.ToResponse()).ToList()
            };

            // Act
            var uri = new Uri($"api/v1/technologies", UriKind.Relative);
            var response = await _client.GetAsync(uri).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<TechnologyResponseObjectList>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetAllTechnologiesReturnsOkResponseWhenTheTableIsEmpty()
        {
            // Arrange
            var uri = new Uri($"api/v1/technologies", UriKind.Relative);
            // Act
            var response = await _client.GetAsync(uri).ConfigureAwait(false);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
