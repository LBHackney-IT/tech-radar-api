using AutoFixture;
using TechRadarApi;
using TechRadarApi.Tests;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace TechRadarApi.Tests.V1.E2ETests
{
    public class TechnologyList
    {
        public List<Technology> Technologies { get; set; }
    }

    [TestFixture]
    public class EndToEndTechRadarTests : DynamoDbIntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();
        private Technology ConstructTestEntity()
        {
            var entity = _fixture.Create<Technology>();
            return entity;
        }

        private TechnologyList ConstructTestEntities()
        {
            var entities = new TechnologyList() { Technologies = _fixture.CreateMany<Technology>().ToList() };
            return entities;
        }

        private async Task SaveTestData(Technology entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync<TechnologyDbEntity>(entity.Id.ToString()).ConfigureAwait(false));
        }

        private async void SetupTestData(TechnologyList entities)
        {
            var tasks = entities.Technologies.Select(entity => SaveTestData(entity));
            await Task.WhenAll(tasks);
        }

        [Test]
        public async Task GetTechnologyByValidIdReturnsOKResponse()
        {
            // Arrange
            var entity = ConstructTestEntity();
            await SaveTestData(entity).ConfigureAwait(false);
            var uri = new Uri($"api/v1/technologies/{entity.Id}", UriKind.Relative);

            // Act
            var response = await Client.GetAsync(uri).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<Technology>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            apiEntity.Should().BeEquivalentTo(entity);
        }

        [Test]
        public async Task GetAllTechnologiesReturnsOKResponse()
        {
            // Arrange
            var entities = ConstructTestEntities();
            SetupTestData(entities);
            var uri = new Uri($"api/v1/technologies", UriKind.Relative);

            // Act
            var response = await Client.GetAsync(uri).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<TechnologyList>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            apiEntity.Technologies.Should().BeEquivalentTo(entities.Technologies);
        }
    }
}
