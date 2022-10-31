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
        private readonly Mock<ITechnologyGateway> _mockGateway;
        private readonly PostNewTechnologyUseCase _classUnderTest;
        private readonly Fixture _fixture;

        public PostNewTechnologyUseCaseTests()
        {
            _mockGateway = new Mock<ITechnologyGateway>();
            _classUnderTest = new PostNewTechnologyUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
         public async Task CreateNewTechnologyReturnsCreatedResponse()
        {
            // Arrange
            var createTechnologyRequest = new CreateTechnologyRequest();

            var technology = _fixture.Create<Technology>();

            _mockGateway.Setup(x => x.PostNewTechnology(new CreateTechnologyRequest())).ReturnsAsync(technology);

            // Act
            var response = await _classUnderTest.Execute(new CreateTechnologyRequest()).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(technology.ToResponse());
        }

        [Fact]
        public async Task PostNewTechnologyUseCaseShouldCallTheGateWay(){
            //arrange 

            _mockGateway.Setup(x => x.PostNewTechnology(It.IsAny<CreateTechnologyRequest>()));
            //Act
            await _classUnderTest.Execute(new CreateTechnologyRequest()).ConfigureAwait(false);

            //Assert
            _mockGateway.Verify(x => x.PostNewTechnology(It.IsAny<CreateTechnologyRequest>()));
        }
        [Fact]
        public async Task PostNewTechnologyByIdAsyncReturnsResponse()
        {
            // Arrange
            var technologyRequest = new CreateTechnologyRequest();
   
            var technology = _fixture.Create<Technology>();

            _mockGateway.Setup(x => x.PostNewTechnology(technologyRequest)).ReturnsAsync(technology);

            // Act
            var response = await _classUnderTest.Execute(technologyRequest)
                .ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(technology.ToResponse());
        }
    }
    
}