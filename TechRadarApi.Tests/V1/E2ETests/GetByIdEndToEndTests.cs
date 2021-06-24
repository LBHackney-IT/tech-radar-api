using AutoFixture;
using TechRadarApi.V1.Domain;
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

        [Test]
        public async Task GetTechnologyByInvalidIdReturnsBadRequestResponse()
        {
            // Arrange
            var badId = _fixture.Create<int>(); // wrong type, should be a guid
            var uri = new Uri($"api/v1/technologies/{badId}", UriKind.Relative);
            // Act
            var response = await Client.GetAsync(uri).ConfigureAwait(false);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetTechnologyByNonExistentIdReturnsNotFoundResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var uri = new Uri($"api/v1/technologies/{id}", UriKind.Relative);
            // Act
            var response = await Client.GetAsync(uri).ConfigureAwait(false);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
