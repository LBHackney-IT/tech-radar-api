using TechRadarApi.V1.Controllers;
using TechRadarApi.V1.UseCase.Interfaces;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Boundary.Request;
using AutoFixture;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace TechRadarApi.Tests.V1.Controllers
{
    public class TechRadarApiControllerFacts
    {
        private readonly TechRadarApiController _classUnderTest;
        private readonly Mock<IGetTechnologyByIdUseCase> _mockGetByIdUsecase;
        private readonly Mock<IGetAllTechnologiesUseCase> _mockGetAllUsecase;
        private readonly Mock<IPostNewTechnologyUseCase> _mockPostNewUsecase;
        private readonly Fixture _fixture = new Fixture();

        public TechRadarApiControllerFacts()
        {
            _mockGetAllUsecase = new Mock<IGetAllTechnologiesUseCase>();
            _mockGetByIdUsecase = new Mock<IGetTechnologyByIdUseCase>();
            _mockPostNewUsecase = new Mock<IPostNewTechnologyUseCase>();
            _classUnderTest = new TechRadarApiController(_mockGetAllUsecase.Object, _mockGetByIdUsecase.Object, _mockPostNewUsecase.Object);
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
        
        [Fact]
         public async void PostTechnologyReturnsCreated()
        {  
             //create fixture of technology response object
             // mockGateway and return response object
             // result of classundertest new request
             // result was CreatedResult.should/ value.should.be
            
            //Arrange
            var technology = _fixture.Create<TechnologyResponseObject>();

            _mockPostNewUsecase.Setup(x => x.Execute(It.IsAny<CreateTechnologyRequest>())).ReturnsAsync(technology);

            // Act

            var result = await _classUnderTest.PostTechnology(new CreateTechnologyRequest()).ConfigureAwait(false);

            // Assert
            (result as CreatedResult).Should().NotBe(null);
            (result as CreatedResult).Should().NotBeOfType<Uri>();
        }

        [Fact]
        public async Task PostTechnologyReturns200Response()
        {
            var guid = Guid.NewGuid();

            var request = new CreateTechnologyRequest()
            {
                Id = Guid.NewGuid(),
                Name = "DynamoDB",
                Description = "NoSQL database hosted on AWS",
                Category = "Languages & Frameworks",
                Technique = "Adopt"
            };
            var response = _fixture.Create<TechnologyResponseObject>();

            _mockPostNewUsecase.Setup(x => x.Execute(request)).ReturnsAsync(response);
        
            var result = await _classUnderTest.PostTechnology(request).ConfigureAwait(false);
            //Assert
            // Helper Methods
            (result as IStatusCodeActionResult).StatusCode.Should().Be(201);
            (result as ObjectResult)?.Value.Should().Be(response.Name);
        }
    }
}
