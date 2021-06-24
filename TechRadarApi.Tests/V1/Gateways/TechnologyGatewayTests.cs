using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Moq;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.Infrastructure;

namespace TechRadarApi.Tests.V1.Gateways
{
    [Collection("DynamoDb collection")]
    public class TechnologyGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly IDynamoDBContext _dynamoDb;
        private TechnologyGateway _classUnderTest;
        private readonly List<Action> _cleanup = new List<Action>();


        public TechnologyGatewayTests(DynamoDbIntegrationTests<Startup> dbTestFixture)
        {
            _dynamoDb = dbTestFixture.DynamoDbContext;
            _classUnderTest = new TechnologyGateway(_dynamoDb);
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

                _disposed = true;
            }
        }
        private async void SetupTestData(List<Technology> entities)
        {
            var tasks = entities.Select(entity => InsertDatatoDynamoDB(entity));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task InsertDatatoDynamoDB(Technology entity)
        {
            await _dynamoDb.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(entity.ToDatabase()).ConfigureAwait(false));
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
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);
            // Act
            var response = await _classUnderTest.GetTechnologyById(entity.Id).ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(entity);
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
            var entities = _fixture.CreateMany<Technology>().ToList();
            SetupTestData(entities);
            // Act
            var response = await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(entities);
        }

        [Fact(Skip = "Cannot mock DynamoDb context")]
        public void GetAllTechologiesExceptionIsThrown()
        {
            // Arrange
            var mockDynamoDb = new Mock<IDynamoDBContext>();
            var exception = new ApplicationException("Test Exception");
            List<ScanCondition> conditions = new List<ScanCondition>();
            mockDynamoDb.Setup(x => x.ScanAsync<TechnologyDbEntity>(conditions, default).GetRemainingAsync(default))
                     .ThrowsAsync(exception);
            // Act
            Func<Task<List<Technology>>> func = async () => await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
            mockDynamoDb.Verify(x => x.ScanAsync<TechnologyDbEntity>(conditions, default).GetRemainingAsync(default), Times.Once);
        }
    }
}
