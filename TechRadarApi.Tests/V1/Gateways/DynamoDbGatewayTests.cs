using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using TechRadarApi.Tests.V1.Helper;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace TechRadarApi.Tests.V1.Gateways
{
    //TODO: Remove this file if DynamoDb gateway not being used
    //TODO: Rename Tests to match gateway name
    //For instruction on how to run tests please see the wiki: https://github.com/LBHackney-IT/lbh-tech-radar-api/wiki/Running-the-test-suite.
    [TestFixture]
    public class DynamoDbGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IDynamoDBContext> _dynamoDb;
        private DynamoDbGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _dynamoDb = new Mock<IDynamoDBContext>();
            _classUnderTest = new DynamoDbGateway(_dynamoDb.Object);
        }

        [Test]
        public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            var id = Guid.NewGuid();
            var response = _classUnderTest.GetEntityById(id);

            response.Should().BeNull();
        }

        [Test]
        public void GetEntityByIdReturnsTheEntityIfItExists()
        {
            var entity = _fixture.Create<Technology>();
            var dbEntity = DatabaseEntityHelper.CreateDatabaseEntityFrom(entity);

            _dynamoDb.Setup(x => x.LoadAsync<TechnologyDbEntity>(entity.Id, default))
                     .ReturnsAsync(dbEntity);

            var response = _classUnderTest.GetEntityById(entity.Id);

            _dynamoDb.Verify(x => x.LoadAsync<TechnologyDbEntity>(entity.Id, default), Times.Once);

            entity.Id.Should().Be(response.Id);
            entity.Name.Should().Be(response.Name);
            entity.Description.Should().Be(response.Description);
            entity.Category.Should().Be(response.Category);
            entity.Technique.Should().Be(response.Technique);
        }
    }
}
