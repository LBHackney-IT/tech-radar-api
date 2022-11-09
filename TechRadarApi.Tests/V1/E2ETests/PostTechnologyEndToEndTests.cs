using AutoFixture;
using FluentAssertions;
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
        private readonly DynamoDbIntegrationTests<Startup> _dbFixture;
        private readonly List<Action> _cleanupActions = new List<Action>();

        public PostTechnologyEndToEndTests(DynamoDbIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
        }

        private CreateTechnologyRequest ConstructTestEntity()
        {
            var technologyRequest = _fixture.Build<CreateTechnologyRequest>()
                .With(x => x.Id == Guid.NewGuid())
                .With(x => x.Name == "DynamoDB")
                .With(x => x.Description == "NoSQL database hosted on AWS")
                .With(x => x.Category == "Language & Frameworks")
                .With(x => x.Technique == "Adopt")
                .Create();

            return technologyRequest;
        }

        private async Task SaveTestData(Technology entity)
        {
            await _dbFixture.DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            _cleanupActions.Add(async () => await _dbFixture.DynamoDbContext.DeleteAsync<TechnologyDbEntity>(entity.Id.ToString()).ConfigureAwait(false));
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
        public async Task PostTechnologyReturnsOkResponse()
        {
            // Arrange
            var technology = ConstructTestEntity();

            var uri = new Uri($"api/v1/technologies/{technology.Name}", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(technology), Encoding.UTF8, "application/json");
            var response = await _dbFixture.Client.PostAsync(uri, content).ConfigureAwait(false);

            
            //Act
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiPerson = JsonConvert.DeserializeObject<TechnologyResponseObject>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }   
    }
}
