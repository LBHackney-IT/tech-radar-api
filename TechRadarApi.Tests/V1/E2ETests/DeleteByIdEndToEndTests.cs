using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using Xunit;

namespace TechRadarApi.Tests.V1.E2ETests
{
    [Collection("DynamoDb collection")]
    public class DeleteByIdEndToEndTests
    {

        private readonly Fixture _fixture = new Fixture();
        public TechnologyDbEntity Technology { get; private set; }
        private readonly DynamoDbIntegrationTests<Startup> _dbFixture;
        private readonly List<Action> _cleanupActions = new List<Action>();

        public DeleteByIdEndToEndTests(DynamoDbIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
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
        public async Task DeleteTechnologyByIdDeletesTechnology()
        {
            //// Arrange
            //var entity = ConstructTestEntity();
            //await SaveTestData(entity).ConfigureAwait(false);
            //var technologies = new List<TechnologyResponseObject>();
            //technologies.Add(entity.ToResponse());
            //// var expectedResponse = new TechnologyResponseObjectList { Technologies = technologies };

            //// Act
            //var uri = new Uri($"api/v1/technologies/{entity.Id}", UriKind.Relative);
            //var response = await _dbFixture.Client.DeleteAsync(uri).ConfigureAwait(false);
            //// var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            //// var actualResponse = JsonConvert.DeserializeObject<TechnologyResponseObjectList>(responseContent);

            //// Assert
            //response.StatusCode.Should().Be(HttpStatusCode.OK);
            //// actualResponse.Should().BeEquivalentTo(expectedResponse);
            ///
            // Arrange
            var entity = ConstructTestEntity();
            await SaveTestData(entity).ConfigureAwait(false);
            var uri = new Uri($"api/v1/technologies/{entity.Id}", UriKind.Relative);
            var bodyParameters = _fixture.Create<TechnologyResponseObject>();

            // Act

            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(bodyParameters), Encoding.UTF8, "application/json");
            var httpResponse = await _dbFixture.Client.SendAsync(message).ConfigureAwait(false);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
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
            var httpResponse = await _dbFixture.Client.SendAsync(message).ConfigureAwait(false);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            message.Dispose();
        }
    }
}
