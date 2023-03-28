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
using TechRadarApi.V1.Domain;

namespace TechRadarApi.Tests.V1.Controllers
{
    public class TechRadarApiControllerTests
    {
        private readonly TechRadarApiController _classUnderTest;
        private readonly Mock<IGetTechnologyByIdUseCase> _mockGetByIdUseCase;
        private readonly Mock<IGetAllTechnologiesUseCase> _mockGetAllUseCase;
        private readonly Mock<IDeleteTechnologyByIdUseCase> _mockDeleteByIdUseCase;
        private readonly Mock<IPostNewTechnologyUseCase> _mockPostUseCase;
        private readonly Mock<IPatchTechnologyByIdUseCase> _mockPatchTechnologyByIdUseCase;


        private readonly Fixture _fixture = new Fixture();

        public TechRadarApiControllerTests()
        {
            _mockGetAllUseCase = new Mock<IGetAllTechnologiesUseCase>();
            _mockGetByIdUseCase = new Mock<IGetTechnologyByIdUseCase>();
            _mockDeleteByIdUseCase = new Mock<IDeleteTechnologyByIdUseCase>();
            _mockPostUseCase = new Mock<IPostNewTechnologyUseCase>();
            _mockPatchTechnologyByIdUseCase = new Mock<IPatchTechnologyByIdUseCase>();
            _classUnderTest = new TechRadarApiController(_mockGetAllUseCase.Object, _mockGetByIdUseCase.Object, _mockPostUseCase.Object, _mockDeleteByIdUseCase.Object, _mockPatchTechnologyByIdUseCase.Object);
        }

        [Fact]
        public async Task GetTechnologyWithValidIdReturnsOkResponse()
        {
            // Arrange
            var expectedResponse = _fixture.Create<TechnologyResponseObject>();
            _mockGetByIdUseCase.Setup(x => x.Execute(expectedResponse.Id)).ReturnsAsync(expectedResponse);

            // Act
            var actualResponse = await _classUnderTest.ViewTechnology(expectedResponse.Id).ConfigureAwait(false) as OkObjectResult;

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.StatusCode.Should().Be(200);
            actualResponse.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetTechnologyWithNonExistentIdReturnsNotFoundResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockGetByIdUseCase.Setup(x => x.Execute(id)).ReturnsAsync((TechnologyResponseObject) null);
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
            _mockGetByIdUseCase.Setup(x => x.Execute(id)).ThrowsAsync(exception);
            // Act + Assert
            _classUnderTest.Invoking(x => x.ViewTechnology(id))
                           .Should()
                           .ThrowAsync<ApplicationException>()
                           .WithMessage(exception.Message);
        }

        [Fact]
        public async Task GetAllTechnologiesReturnsOkResponse()
        {
            // Arrange
            var expectedResponse = new TechnologyResponseObjectList() { Technologies = _fixture.CreateMany<TechnologyResponseObject>().ToList() };
            _mockGetAllUseCase.Setup(x => x.Execute()).ReturnsAsync(expectedResponse);

            // Act
            var actualResponse = await _classUnderTest.ListTechnologies().ConfigureAwait(false) as OkObjectResult;

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.StatusCode.Should().Be(200);
            actualResponse.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetAllTechnologiesReturnsOkResponseWhenTheTableIsEmpty()
        {
            // Arrange
            var emptyResponseObject = new TechnologyResponseObjectList() { Technologies = new List<TechnologyResponseObject>() };
            _mockGetAllUseCase.Setup(x => x.Execute()).ReturnsAsync(emptyResponseObject);

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
            _mockGetAllUseCase.Setup(x => x.Execute()).ThrowsAsync(exception);
            // Act + Assert
            _classUnderTest.Invoking(x => x.ListTechnologies())
                           .Should()
                           .ThrowAsync<ApplicationException>()
                           .WithMessage(exception.Message);
        }


        [Fact]
        public async Task PostTechnologyReturns201Created()
        {
            var request = _fixture.Create<CreateTechnologyRequest>();
            var response = _fixture.Create<TechnologyResponseObject>();

            _mockPostUseCase.Setup(x => x.Execute(request)).ReturnsAsync(response);

            var result = await _classUnderTest.PostTechnology(request).ConfigureAwait(false);
            //Assert
            //
            (result as IStatusCodeActionResult).StatusCode.Should().Be(201);
            (result as ObjectResult)?.Value.Should().Be(response);
        }

        [Fact]
        public void PostTechnologyExceptionThrown()
        {
            // Arrange
            var request = _fixture.Create<CreateTechnologyRequest>();
            var exception = new Exception("Some Exception");
            _mockPostUseCase.Setup(x => x.Execute(request)).ThrowsAsync(exception);

            // Act + Assert
            _classUnderTest.Invoking(x => x.PostTechnology(request))
                           .Should()
                           .ThrowAsync<Exception>()
                           .WithMessage(exception.Message);
        }

        [Fact]
        public async Task DeleteTechnologyReturnsOkResponse()
        {
            var query = DeletionQuery();
            var technologyResponse = _fixture.Create<TechnologyResponseObject>();

            _mockDeleteByIdUseCase.Setup(x => x.Execute(query.Id)).ReturnsAsync(technologyResponse);

            var response = await _classUnderTest.DeleteTechnology(query.Id).ConfigureAwait(false);
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(technologyResponse);

        }

        [Fact]
        public async Task DeleteTechnologyReturnsNotFound()
        {
            var query = DeletionQuery();
            _mockDeleteByIdUseCase.Setup(x => x.Execute(query.Id)).ReturnsAsync((TechnologyResponseObject) null);

            var response = await _classUnderTest.DeleteTechnology(query.Id).ConfigureAwait(false);
            response.Should().BeOfType(typeof(NotFoundResult));
        }

        [Fact]
        public void DeleteTechnologyThrowsException()
        {
            var query = DeletionQuery();
            var exception = new ApplicationException("Test exception");
            _mockDeleteByIdUseCase.Setup(x => x.Execute(query.Id)).ThrowsAsync(exception);

            Func<Task<IActionResult>> func = async () => await _classUnderTest.DeleteTechnology(query.Id).ConfigureAwait(false);

            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);

        }

        [Fact]
        public async Task UpdateTechnologyAsyncReturnsNoContentResponse()
        {
            // Arrange
            (var pathParameters, var bodyParameters) = ConstructUpdateTechnologyQuery();
            var technology = _fixture.Create<Technology>();
            _mockPatchTechnologyByIdUseCase.Setup(x => x.Execute(pathParameters, bodyParameters)).ReturnsAsync((Technology) technology);

            // Act
            var response = await _classUnderTest.PatchTechnology(bodyParameters, pathParameters).ConfigureAwait(false); // double check order of body and path

            // Assert
            response.Should().BeOfType(typeof(NoContentResult));
        }
        [Fact]
        public async Task UpdateTechnologyAsyncNotFoundReturnsNotFound()
        {
            // Arrange
            (var pathParameters, var bodyParameters) = ConstructUpdateTechnologyQuery();
            _mockPatchTechnologyByIdUseCase.Setup(x => x.Execute(pathParameters, bodyParameters)).ReturnsAsync((Technology) null);

            // Act
            var response = await _classUnderTest.PatchTechnology(bodyParameters, pathParameters).ConfigureAwait(false); // double check order of body and path

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(pathParameters.Id);
        }

          private (TechnologyResponseObject, PatchTechnologyItem) ConstructUpdateTechnologyQuery()
        {
            var pathParameters = _fixture.Create<TechnologyResponseObject>();
            var bodyParameters = _fixture.Create<PatchTechnologyItem>();
            return (pathParameters, bodyParameters);
        }


        private static TechnologyResponseObject DeletionQuery()
        {
            return new TechnologyResponseObject() { Id = Guid.NewGuid(), Name = "TestTechnology" };
        }
    }
}
