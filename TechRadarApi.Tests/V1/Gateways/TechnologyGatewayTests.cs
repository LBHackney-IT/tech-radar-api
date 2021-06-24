using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using System.Linq;
using TechRadarApi.Tests.V1.Helper;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechRadarApi.Tests.V1.Gateways
{
    [TestFixture]
    public class TechnologyGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IDynamoDBContext> _dynamoDb;
        private TechnologyGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _dynamoDb = new Mock<IDynamoDBContext>();
            _classUnderTest = new TechnologyGateway(_dynamoDb.Object);
        }

        [Test]
        public async Task GetTechnologyByIdReturnsNullIfTechnologyDoesntExist()
        {
            // Assert
            var id = Guid.NewGuid();
            // Act
            var response = await _classUnderTest.GetTechnologyById(id).ConfigureAwait(false);
            // Assert
            response.Should().BeNull();
        }

        [Test]
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

            response.Id.Should().Be(entity.Id.ToString());
            response.Name.Should().Be(entity.Name);
            response.Description.Should().Be(entity.Description);
            response.Category.Should().Be(entity.Category);
            response.Technique.Should().Be(entity.Technique);
        }

        [Test]
        [Ignore("Getting a bug - can't stub the DB response")]
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

        [Test]
        [Ignore("Getting a bug - can't stub the DB response")]
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
    }
}
