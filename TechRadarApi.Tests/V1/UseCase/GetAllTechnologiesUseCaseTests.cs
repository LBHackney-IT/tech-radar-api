using System.Linq;
using AutoFixture;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace TechRadarApi.Tests.V1.UseCase
{
    public class GetAllUseCaseTests
    {
        private Mock<ITechnologyGateway> _mockGateway;
        private GetAllTechnologiesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<ITechnologyGateway>();
            _classUnderTest = new GetAllTechnologiesUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetsAllFromTheGateway()
        {
            // Arrange
            var stubbedEntities = _fixture.CreateMany<Technology>().ToList();
            _mockGateway.Setup(x => x.GetAll()).Returns(stubbedEntities);
            var expectedResponse = new TechnologyResponseObjectList { Technologies = stubbedEntities.ToResponse() };
            // Act
            var actualResponse = _classUnderTest.Execute();
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
