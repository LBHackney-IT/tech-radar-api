using System.Linq;
using AutoFixture;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase;
using FluentAssertions;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace TechRadarApi.Tests.V1.UseCase
{
    public class GetAllUseCaseTests
    {
        private Mock<ITechnologyGateway> _mockGateway;
        private GetAllTechnologiesUseCase _classUnderTest;
        private Fixture _fixture;

        public GetAllUseCaseTests()
        {
            _mockGateway = new Mock<ITechnologyGateway>();
            _classUnderTest = new GetAllTechnologiesUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetsAllFromTheGateway()
        {
            // Arrange
            var stubbedEntities = _fixture.CreateMany<Technology>().ToList();
            _mockGateway.Setup(x => x.GetAll()).ReturnsAsync(stubbedEntities);
            var expectedResponse = new TechnologyResponseObjectList { Technologies = stubbedEntities.ToResponse() };
            // Act
            var actualResponse = await _classUnderTest.Execute().ConfigureAwait(false);
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task ReturnsEmptyResponseObjectListIfNoTechnologies()
        {
            // Arrange
            var stubbedEntities = new List<Technology>();
            _mockGateway.Setup(x => x.GetAll()).ReturnsAsync(stubbedEntities);
            var expectedResponse = new TechnologyResponseObjectList { Technologies = new List<TechnologyResponseObject>() };
            // Act
            var actualResponse = await _classUnderTest.Execute().ConfigureAwait(false);
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public void GetAllTechnologiesExceptionIsThrown()
        {
            // Assert
            var exception = new ApplicationException("Test Exception");
            _mockGateway.Setup(x => x.GetAll()).ThrowsAsync(exception);
            // Act
            Func<Task<TechnologyResponseObjectList>> func = async () => await _classUnderTest.Execute().ConfigureAwait(false);
            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
