using TechRadarApi.V1.Controllers;
using TechRadarApi.V1.UseCase.Interfaces;
using TechRadarApi.V1.Boundary.Response;
using AutoFixture;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechRadarApi.Tests.V1.Controllers
{
    [TestFixture]
    public class TechRadarApiControllerTests
    {
        private readonly TechRadarApiController _classUnderTest;
        private readonly Mock<IGetTechnologyByIdUseCase> _mockGetByIdUsecase;
        private readonly Mock<IGetAllTechnologiesUseCase> _mockGetAllUsecase;
        private readonly Fixture _fixture = new Fixture();

        public TechRadarApiControllerTests()
        {
            _mockGetAllUsecase = new Mock<IGetAllTechnologiesUseCase>();
            _mockGetByIdUsecase = new Mock<IGetTechnologyByIdUseCase>();
            _classUnderTest = new TechRadarApiController(_mockGetAllUsecase.Object, _mockGetByIdUsecase.Object);
        }

        [Test]
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

        [Test]
        public async Task GetTechnologyWithNonExistentIDReturnsNotFoundResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            // Act
            var response = await _classUnderTest.ViewTechnology(id).ConfigureAwait(false) as NotFoundObjectResult;
            // Assert
            response.StatusCode.Should().Be(404);
        }

        [Test]
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

        [Test]
        public async Task GetAllTechnologiesReturnsNoContentResponseWhenTheTableIsEmpty()
        {
            // Arrange
            var emptyResponseObject = new TechnologyResponseObjectList() { Technologies = new List<TechnologyResponseObject>() };
            _mockGetAllUsecase.Setup(x => x.Execute()).ReturnsAsync(emptyResponseObject);

            // Act
            var response = await _classUnderTest.ListTechnologies().ConfigureAwait(false) as NoContentResult;

            // Assert
            response.StatusCode.Should().Be(204);
        }
    }
}
