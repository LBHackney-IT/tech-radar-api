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
using TechRadarApi.V1.Boundary.Request;

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

        private async Task InsertDatatoDynamoDB(Technology entity)
        {
            await _dynamoDb.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(entity.ToDatabase()).ConfigureAwait(false));
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
            var entity = _fixture.Create<Technology>();
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);
            var expectedResponse = new List<Technology>();
            expectedResponse.Add(entity);
            // Act
            var response = await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public async Task PostTechnologySuccessfullySaves()
        {
            //Arrange
            var postRequest = ConstructTestEntity();
            var mockDynamoDb = new Mock<IDynamoDBContext>();
            _classUnderTest = new TechnologyGateway(mockDynamoDb.Object);

            //Act
            _ = await _classUnderTest.PostNewTechnology(postRequest).ConfigureAwait(false);
            var dbEntity = await _dynamoDb.LoadAsync<TechnologyDbEntity>(postRequest.Id).ConfigureAwait(false);
            
            // Assert
            dbEntity.Should().BeEquivalentTo(postRequest);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync<TechnologyDbEntity>(dbEntity.Id).ConfigureAwait(false));
            
            
        }
    }
}
