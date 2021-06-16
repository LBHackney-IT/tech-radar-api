using AutoFixture;
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

    [TestFixture]
    public class GetAllEndToEndTests : EndToEndTestsBase
    {
        private readonly Fixture _fixture = new Fixture();

        private TechnologyList ConstructTestEntities()
        {
            var entities = new TechnologyList() { Technologies = _fixture.CreateMany<Technology>().ToList() };
            return entities;
        }

        private async void SetupTestData(TechnologyList entities)
        {
            var tasks = entities.Technologies.Select(entity => SaveTestData(entity));
            await Task.WhenAll(tasks).ConfigureAwait(false);
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

        [Test]
        public async Task GetAllTechnologiesReturnsNoContentResponseWhenTheTableIsEmpty()
        {
            // Arrange
            var uri = new Uri($"api/v1/technologies", UriKind.Relative);
            // Act
            var response = await Client.GetAsync(uri).ConfigureAwait(false);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
