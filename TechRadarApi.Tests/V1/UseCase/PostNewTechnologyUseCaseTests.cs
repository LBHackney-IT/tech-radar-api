using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase;
using TechRadarApi.V1.Domain;
using Moq;
using Xunit;
using AutoFixture;
using FluentAssertions;
using System.Threading.Tasks;
using System;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Boundary.Request;
using TechRadarApi.V1.Factories;

namespace TechRadarApi.Tests.V1.UseCase
{
    public class PostNewTechnologyUseCaseTests
    {
        private Mock<ITechnologyGateway> _mockGateway;
        private PostNewTechnologyUseCase _classUnderTest;
        private Fixture _fixture;

        public PostNewTechnologyUseCaseTests()
        {
            _mockGateway = new Mock<ITechnologyGateway>();
            _classUnderTest = new GetTechnologyByIdUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
         public async Task CreateNewTechnologyReturnsCreatedResponse()
        {
            // Arrange
            var technologyRequest = new CreateTechnologyRequest();

            var technology = _fixture.Create<Technology>();

            _mockGateway.Setup(x => x.PostNewTechnology(technologyReqeust).ReturnsAsync(technology));

            // Act
            var response = await _classUnderTest.Execute(technologyRequest)
                .ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(technology.ToResponse());
        }
    }
}