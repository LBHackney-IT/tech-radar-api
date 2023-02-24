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
using TechRadarApi.V1.Factories;

namespace TechRadarApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetByIdUseCaseTests
    {
        private Mock<ITechnologyGateway> _mockGateway;
        private GetTechnologyByIdUseCase _classUnderTest;
        private Fixture _fixture;

        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<ITechnologyGateway>();
            _classUnderTest = new GetTechnologyByIdUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetsTechnologyFromGatewayUsingId()
        {
            // Arrange
            var stubbedEntity = _fixture.Create<Technology>();
            _mockGateway.Setup(x => x.GetTechnologyById(stubbedEntity.Id)).ReturnsAsync(stubbedEntity);
            var expectedResponse = stubbedEntity.ToResponse();
            // Act
            var response = await _classUnderTest.Execute(stubbedEntity.Id).ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task ReturnsNullIfIdNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockGateway.Setup(x => x.GetTechnologyById(id)).ReturnsAsync((Technology) null);
            // Act
            var response = await _classUnderTest.Execute(id).ConfigureAwait(false);
            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public void GetTechnologyByIdExceptionIsThrown()
        {
            // Arrange
            var id = Guid.NewGuid();
            var exception = new ApplicationException("Test Exception");
            _mockGateway.Setup(x => x.GetTechnologyById(id)).ThrowsAsync(exception);
            // Act + Assert
            _classUnderTest.Invoking(x => x.Execute(id))
                           .Should()
                           .ThrowAsync<ApplicationException>()
                           .WithMessage(exception.Message);
        }
    }
}
