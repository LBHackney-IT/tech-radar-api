using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase;
using TechRadarApi.V1.Domain;
using Moq;
using NUnit.Framework;
using AutoFixture;
using FluentAssertions;

namespace TechRadarApi.Tests.V1.UseCase
{
    public class GetByIdUseCaseTests
    {
        private Mock<ITechnologyGateway> _mockGateway;
        private GetTechnologyByIdUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<ITechnologyGateway>();
            _classUnderTest = new GetTechnologyByIdUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetsTechnologyFromGatewayUsingId()
        {
            // Arrange
            var stubbedEntity = _fixture.Create<Technology>();
            _mockGateway.Setup(x => x.GetTechnologyById(stubbedEntity.Id)).Returns(stubbedEntity);
            // Act
            var response = _classUnderTest.Execute(stubbedEntity.Id);
            // Assert
            response.Should().BeEquivalentTo(stubbedEntity);
        }
    }
}
