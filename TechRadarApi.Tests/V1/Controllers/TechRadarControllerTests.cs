using TechRadarApi.V1.Controllers;
using TechRadarApi.V1.UseCase.Interfaces;
using TechRadarApi.V1.Boundary.Response;
using AutoFixture;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechRadarApi.Tests.V1.Controllers
{
    public class TechRadarApiControllerFacts
    {
        private readonly TechRadarApiController _classUnderTest;
        private readonly Mock<IGetTechnologyByIdUseCase> _mockGetByIdUsecase;
        private readonly Mock<IGetAllTechnologiesUseCase> _mockGetAllUsecase;
        private readonly Mock<IDeleteTechnologyByIdUseCase> _mockDeleteByIdUsecase;
        
        private readonly Fixture _fixture = new Fixture();

        public TechRadarApiControllerFacts()
        {
            _mockGetAllUsecase = new Mock<IGetAllTechnologiesUseCase>();
            _mockGetByIdUsecase = new Mock<IGetTechnologyByIdUseCase>();
            _mockDeleteByIdUsecase = new Mock<IDeleteTechnologyByIdUseCase>();
            _classUnderTest = new TechRadarApiController(_mockGetAllUsecase.Object, _mockGetByIdUsecase.Object, _mockDeleteByIdUsecase.Object);
        }

        [Fact]
        public async Task GetTechnologyWithValidIDReturnsOKResponse()
        {
            // Arrange
            var expectedResponse = _fixture.Create<TechnologyResponseObject>();
            _mockGetByIdUsecase.Setup(x => x.Execute(expectedResponse.Id)).ReturnsAsync(expectedResponse);

            // Act
            var actualResponse = await _classUnderTest.ViewTechnology(expectedResponse.Id).ConfigureAwait(false) as OkObjectResult;

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.StatusCode.Should().Be(200);
            actualResponse.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetTechnologyWithNonExistentIDReturnsNotFoundResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockGetByIdUsecase.Setup(x => x.Execute(id)).ReturnsAsync((TechnologyResponseObject) null);
            // Act
            var response = await _classUnderTest.ViewTechnology(id).ConfigureAwait(false) as NotFoundObjectResult;
            // Assert
            response.StatusCode.Should().Be(404);
        }

        [Fact]
        public void GetTechnologyByIdExceptionIsThrown()
        {
            // Arrange
            var id = Guid.NewGuid();
            var exception = new ApplicationException("Test exception");
            _mockGetByIdUsecase.Setup(x => x.Execute(id)).ThrowsAsync(exception);
            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.ViewTechnology(id).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }

        [Fact]
        public async Task GetAllTechnologiesReturnsOKResponse()
        {
            // Arrange
            var expectedResponse = new TechnologyResponseObjectList() { Technologies = _fixture.CreateMany<TechnologyResponseObject>().ToList() };
            _mockGetAllUsecase.Setup(x => x.Execute()).ReturnsAsync(expectedResponse);

            // Act
            var actualResponse = await _classUnderTest.ListTechnologies().ConfigureAwait(false) as OkObjectResult;

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.StatusCode.Should().Be(200);
            actualResponse.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetAllTechnologiesReturnsOKResponseWhenTheTableIsEmpty()
        {
            // Arrange
            var emptyResponseObject = new TechnologyResponseObjectList() { Technologies = new List<TechnologyResponseObject>() };
            _mockGetAllUsecase.Setup(x => x.Execute()).ReturnsAsync(emptyResponseObject);

            // Act
            var response = await _classUnderTest.ListTechnologies().ConfigureAwait(false) as OkObjectResult;

            // Assert
            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public void GetAllTechnologiesExceptionIsThrown()
        {
            // Arrange
            var exception = new ApplicationException("Test exception");
            _mockGetAllUsecase.Setup(x => x.Execute()).ThrowsAsync(exception);
            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.ListTechnologies().ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
