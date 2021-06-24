using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using System.Linq;
using TechRadarApi.Tests.V1.Helper;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechRadarApi.Tests.V1.Gateways
{
    public class TechnologyGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IDynamoDBContext> _dynamoDb;
        private TechnologyGateway _classUnderTest;

        public TechnologyGatewayTests()
        {
            _dynamoDb = new Mock<IDynamoDBContext>();
            _classUnderTest = new TechnologyGateway(_dynamoDb.Object);
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
            var dbEntity = DatabaseEntityHelper.CreateDatabaseEntityFrom(entity);

            _dynamoDb.Setup(x => x.LoadAsync<TechnologyDbEntity>(entity.Id.ToString(), default))
                     .ReturnsAsync(dbEntity);

            // Act
            var response = await _classUnderTest.GetTechnologyById(entity.Id).ConfigureAwait(false);

            // Assert
            _dynamoDb.Verify(x => x.LoadAsync<TechnologyDbEntity>(entity.Id.ToString(), default), Times.Once);

            response.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public void GetTechologyByIdExceptionIsThrown()
        {
            // Assert
            var id = Guid.NewGuid();
            var exception = new ApplicationException("Test Exception");
            _dynamoDb.Setup(x => x.LoadAsync<TechnologyDbEntity>(id.ToString(), default))
                     .ThrowsAsync(exception);
            // Act
            Func<Task<Technology>> func = async () => await _classUnderTest.GetTechnologyById(id).ConfigureAwait(false);
            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
            _dynamoDb.Verify(x => x.LoadAsync<TechnologyDbEntity>(id.ToString(), default), Times.Once);
        }

        [Fact(Skip = "Getting a bug - can't stub the DB response")]
        public async Task GetAllTechnologiesReturnsEmptyArrayIfNoTechnologiesExist()
        {
            // Arrange
            List<ScanCondition> conditions = new List<ScanCondition>();
            var stubbedResponse = new List<TechnologyDbEntity>();

            _dynamoDb.Setup(x => x.ScanAsync<TechnologyDbEntity>(conditions, default).GetRemainingAsync(default))
                    .ReturnsAsync(stubbedResponse);
            // Act
            var response = await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            _dynamoDb.Verify(x => x.ScanAsync<TechnologyDbEntity>(conditions, default).GetRemainingAsync(default), Times.Once);
            response.Should().BeEmpty();
        }

        [Fact(Skip = "Getting a bug - can't stub the DB response")]
        public async Task GetAllTechnologiesReturnsAnArrayOfAllTechnologiesInTheTable()
        {
            // Arrange
            var entities = _fixture.CreateMany<Technology>().ToList();
            var stubbedResponse = entities.Select(x => DatabaseEntityHelper.CreateDatabaseEntityFrom(x)).ToList();

            List<ScanCondition> conditions = new List<ScanCondition>();
            _dynamoDb.Setup(x => x.ScanAsync<TechnologyDbEntity>(conditions, default).GetRemainingAsync(default))
                    .ReturnsAsync(stubbedResponse);
            // Act
            var response = await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            _dynamoDb.Verify(x => x.ScanAsync<TechnologyDbEntity>(conditions, default).GetRemainingAsync(default), Times.Once);
            response.Should().BeEquivalentTo(entities);
        }

        [Fact(Skip = "Getting a bug - can't stub the DB response")]
        public void GetAllTechologiesExceptionIsThrown()
        {
            // Assert
            var exception = new ApplicationException("Test Exception");
            List<ScanCondition> conditions = new List<ScanCondition>();
            _dynamoDb.Setup(x => x.ScanAsync<TechnologyDbEntity>(conditions, default).GetRemainingAsync(default))
                     .ThrowsAsync(exception);
            // Act
            Func<Task<List<Technology>>> func = async () => await _classUnderTest.GetAll().ConfigureAwait(false);
            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
            _dynamoDb.Verify(x => x.ScanAsync<TechnologyDbEntity>(conditions, default).GetRemainingAsync(default), Times.Once);
        }
    }
}
