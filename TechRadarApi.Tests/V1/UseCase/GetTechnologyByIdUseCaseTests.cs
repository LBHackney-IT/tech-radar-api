using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase;
using TechRadarApi.V1.Domain;
using Moq;
using NUnit.Framework;
using AutoFixture;
using FluentAssertions;
using System.Threading.Tasks;

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
        public async Task GetsTechnologyFromGatewayUsingId()
        {
            // Arrange
            var stubbedEntity = _fixture.Create<Technology>();
            _mockGateway.Setup(x => x.GetTechnologyById(stubbedEntity.Id)).ReturnsAsync(stubbedEntity);
            // Act
            var response = await _classUnderTest.Execute(stubbedEntity.Id).ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(stubbedEntity);
        }
    }
}
