using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase;
using TechRadarApi.V1.Domain;
using Moq;
using NUnit.Framework;
using AutoFixture;
using FluentAssertions;
using System.Threading.Tasks;
using System;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;

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
            var expectedResponse = stubbedEntity.ToResponse();
            // Act
            var response = await _classUnderTest.Execute(stubbedEntity.Id).ConfigureAwait(false);
            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
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

        [Test]
        public void GetTechnologyByIdExceptionIsThrown()
        {
            // Assert
            var id = Guid.NewGuid();
            var exception = new ApplicationException("Test Exception");
            _mockGateway.Setup(x => x.GetTechnologyById(id)).ThrowsAsync(exception);
            // Act
            Func<Task<TechnologyResponseObject>> func = async () => await _classUnderTest.Execute(id).ConfigureAwait(false);
            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
