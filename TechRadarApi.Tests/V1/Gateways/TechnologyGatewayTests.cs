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
using Moq;

namespace TechRadarApi.Tests.V1.Gateways
{
    [Collection("DynamoDb Collection")]
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
                foreach (var action in _cleanup)
                    action();
                _dbFixture.Dispose();

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
                _cleanup.Add((() => _dbFixture.DynamoDbContext.DeleteAsync(technology).GetAwaiter().GetResult()));
                output.Add(technology.ToDomain());
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return output;
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
            var expectedResult = await CreateAndInsertTechnologies(1).ConfigureAwait(false);
            // Act
            var response = await _classUnderTest.GetTechnologyById(expectedResult.FirstOrDefault().Id).ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(expectedResult.FirstOrDefault());
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
            var expectedResult = await CreateAndInsertTechnologies(3).ConfigureAwait(false);
            // Act
            var response = await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetTechologyByIdExceptionIsThrown()
        {
            // Assert
            var mockDynamoDb = new Mock<IDynamoDBContext>();
            _classUnderTest = new TechnologyGateway(mockDynamoDb.Object);
            var id = Guid.NewGuid();
            var exception = new ApplicationException("Test Exception");
            mockDynamoDb.Setup(x => x.LoadAsync<TechnologyDbEntity>(id.ToString(), default))
                     .ThrowsAsync(exception);
            // Act
            Func<Task<Technology>> func = async () => await _classUnderTest.GetTechnologyById(id).ConfigureAwait(false);
            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
            mockDynamoDb.Verify(x => x.LoadAsync<TechnologyDbEntity>(id.ToString(), default), Times.Once);
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
            _cleanup.Add(() => _dbFixture.DynamoDbContext.DeleteAsync<TechnologyDbEntity>(dbEntity.Id).GetAwaiter().GetResult());
            // Assert
            dbEntity.Should().BeEquivalentTo(postRequest.ToDatabase(), c => c.Excluding(x => x.Id));
        }
    }
}
