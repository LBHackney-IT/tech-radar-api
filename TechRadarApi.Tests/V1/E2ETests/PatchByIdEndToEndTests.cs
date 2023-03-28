using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechRadarApi.V1.Boundary.Request;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using Xunit;

namespace TechRadarApi.Tests.V1.E2ETests
{
    [Collection("DynamoDb collection")]
    public class PatchByIdEndToEndTests
    {

        private readonly Fixture _fixture = new Fixture();
        public TechnologyDbEntity Technology { get; private set; }
        private readonly IDynamoDbFixture _dbFixture;
        private readonly HttpClient _client;
        private readonly List<Action> _cleanupActions = new List<Action>();

        public PatchByIdEndToEndTests(AwsMockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _client = appFactory.Client;
        }

        private Technology ConstructTestEntity()
        {
            var entity = _fixture.Create<Technology>();
            return entity;
        }

        private async Task SaveTestData(Technology entity)
        {
            await _dbFixture.DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            _cleanupActions.Add(async () => await _dbFixture.DynamoDbContext.DeleteAsync<TechnologyDbEntity>(entity.Id.ToString()).ConfigureAwait(false));
        }

        [Fact]
        public async Task UpdateTechnologyReturns404NotFound()
        {
            // Arrange
            var TestToken =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";
            var entity = ConstructTestEntity();
            await SaveTestData(entity).ConfigureAwait(false);
            var uri = new Uri($"api/v1/technologies/{entity.Id}", UriKind.Relative);
            var bodyParameters = _fixture.Create<PatchTechnologyItem>();

            // Act
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(bodyParameters), Encoding.UTF8, "application/json");
            message.Headers.Add("Authorization", TestToken);
            var response = await _dbFixture.Client.SendAsync(message).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            message.Dispose();
        }

        [Fact]
        public async Task PatchExistingTechnologyReturns204NoContent()
        {
            // Arrange
            var TestToken =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";
            var pathParameters = _fixture.Create<TechnologyResponseObject>();
            var bodyParameters = _fixture.Create<PatchTechnologyItem>();
            var technology = _fixture.Build<Technology>()
                              .With(x => x.Id, pathParameters.Id)
                              .Create();
            technology.ToDatabase();
            var uri = new Uri($"api/v1/technologies/{pathParameters.Id}", UriKind.Relative);
            await SaveTestData(technology).ConfigureAwait(false);

            // Act
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(bodyParameters), Encoding.UTF8, "application/json");
            message.Headers.Add("Authorization", TestToken);
            var response = await _dbFixture.Client.SendAsync(message).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var updatedTechnology = await _dbFixture.DynamoDbContext.SaveAsync<TechnologyDbEntity>(technology.Id).ConfigureAwait(false);
            updatedTechnology.LastOrDefault().Name.Should().Be(bodyParameters.Name);
            updatedTechnology.LastOrDefault().Description.Should().Be(bodyParameters.Description);
            updatedTechnology.LastOrDefault().Category.Should().Be(bodyParameters.Category);
            updatedTechnology.LastOrDefault().Technique.Should().Be(bodyParameters.Technique);
            message.Dispose();
        }

        [Fact]
        public async Task UpdateTechnologyReturns204NoContent()
        {
            // Arrange
            var TestToken =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";
            var pathParameters = _fixture.Create<TechnologyResponseObject>();
            var bodyParameters = _fixture.Create<PatchTechnologyItem>();
            var technology = _fixture.Build<Technology>()
                              .With(x => x.Id, pathParameters.Id)
                              .Create();
            var uri = new Uri($"api/v1/technologies/{pathParameters.Id}", UriKind.Relative);
            await SaveTestData(technology).ConfigureAwait(false);

            // Act
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(bodyParameters), Encoding.UTF8, "application/json");
            message.Headers.Add("Authorization", TestToken);
            var response = await _dbFixture.Client.SendAsync(message).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var updatedTechnology = await _dbFixture.DynamoDbContext.SaveAsync<TechnologyDbEntity>(technology.Id).ConfigureAwait(false);

            updatedTechnology.LastOrDefault().Name.Should().Be(bodyParameters.Name);
            updatedTechnology.LastOrDefault().Description.Should().Be(bodyParameters.Description);
            updatedTechnology.LastOrDefault().Category.Should().Be(bodyParameters.Category);
            updatedTechnology.LastOrDefault().Technique.Should().Be(bodyParameters.Technique);
            message.Dispose();
        }
    }
}