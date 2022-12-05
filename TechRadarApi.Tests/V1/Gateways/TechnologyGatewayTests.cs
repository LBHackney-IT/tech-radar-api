using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.Infrastructure;
using TechRadarApi.V1.Boundary.Request;

namespace TechRadarApi.Tests.V1.Gateways
{
    [Collection("DynamoDb collection")]
    public class TechnologyGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly IDynamoDbFixture _dbFixture;
        private TechnologyGateway _classUnderTest;
        private readonly List<Action> _cleanup = new List<Action>();


        public TechnologyGatewayTests(AwsMockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _classUnderTest = new TechnologyGateway(_dbFixture.DynamoDbContext);
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
                _dbFixture.Dispose();
                foreach (var action in _cleanup)
                    action();

                _disposed = true;
            }
        }

        [Fact]
        public async Task GetTechnologyByIdReturnsNullIfTechnologyDoesntExist()
        {
            // Assert
            var id = Guid.NewGuid();
            // Act
            var response = await _classUnderTest.GetTechnologyById(id).ConfigureAwait(false);
            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetTechnologyByIdReturnsTheTechnologyIfItExists()
        {
            // Arrange
            var entity = _fixture.Create<Technology>();
            await _dbFixture.SaveEntityAsync<TechnologyDbEntity>(entity.ToDatabase()).ConfigureAwait(false);
            // Act
            var response = await _classUnderTest.GetTechnologyById(entity.Id).ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(entity);
        }

        private List<TechnologyDbEntity> CreateAndInsertTechnologies(int count)
        {
            var technologies = new List<TechnologyDbEntity>();

            for (int i = 0; i < count; i++)
            {
                var technology = _fixture.Create<TechnologyDbEntity>();
                _dbFixture.SaveEntityAsync(technology).GetAwaiter().GetResult();
                technologies.Add(technology);
            }

            return technologies;
        }

        [Fact]
        public async Task GetAllTechnologiesReturnsEmptyArrayIfNoTechnologiesExist()
        {
            // Arrange + Act
            var response = await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            response.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllTechnologiesReturnsAnArrayOfAllTechnologiesInTheTable()
        {
            // Arrange
            var technologies = CreateAndInsertTechnologies(5);
            var expectedResult = technologies.Select(x => x.ToDomain()).ToList();
            // Act
            var response = await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task PostTechnologySuccessfullySaves()
        {
            // Arrange
            var postRequest = _fixture.Create<CreateTechnologyRequest>();
            // Act
            var response = await _classUnderTest.PostNewTechnology(postRequest).ConfigureAwait(false);
            var dbEntity = await _dbFixture.DynamoDbContext.LoadAsync<TechnologyDbEntity>(response.Id)
                                                           .ConfigureAwait(false);
            // Assert
            dbEntity.Should().BeEquivalentTo(postRequest.ToDatabase(), c => c.Excluding(x => x.Id));
            _cleanup.Add(async () => await _dbFixture.DynamoDbContext.DeleteAsync<TechnologyDbEntity>(dbEntity.Id)
                                                                     .ConfigureAwait(false));
        }
    }
}
