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

namespace TechRadarApi.Tests.V1.E2ETests
{

    [TestFixture]
    public class GetByIDEndToEndTests : EndToEndTestsBase
    {
        private readonly Fixture _fixture = new Fixture();

        private Technology ConstructTestEntity()
        {
            var entity = _fixture.Create<Technology>();
            return entity;
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
    }
}
