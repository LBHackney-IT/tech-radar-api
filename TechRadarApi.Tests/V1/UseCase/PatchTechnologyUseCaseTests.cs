using System.Linq;
using AutoFixture;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Boundary.Request;
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
using Microsoft.AspNetCore.Mvc;
using TechRadarApi.V1.Infrastructure;
using System.Collections;

namespace TechRadarApi.Tests.V1.UseCase
{
    public class PatchTechnologyByIdUseCaseTests
    {
        private Mock<ITechnologyGateway> _mockGateway;
        private PatchTechnologyByIdUseCase _classUnderTest;
        private Fixture _fixture;

        public PatchTechnologyByIdUseCaseTests()
        {
            _mockGateway = new Mock<ITechnologyGateway>();
            _classUnderTest = new PatchTechnologyByIdUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
         public async Task AddsTechnologyToTechRadarSuccessfully()
        {
            //Arrange
            var pathParameters = _fixture.Build<TechnologyResponseObject>().With(x => x.Id, Guid.Empty).Create();
            var bodyParameters = _fixture.Create<PatchTechnologyItem>();
            var technologies = _fixture.Create<Technology>();

            _mockGateway.Setup(x => x.GetAll()).ReturnsAsync(technologies);

            //Act
            var result = await _classUnderTest.Execute(pathParameters, bodyParameters).ConfigureAwait(false);

            //Assert
            var numberOfTechnologies = result.Technologies.Count;
            _mockGateway.Verify(x => x.SaveTechRadar(It.IsAny<Technology>()), Times.Once()); // change gateway method
            result.Technolologies.Should().Contain(x => x.Name == bodyParameters.Name);
            result.Technolologies.Distinct().Count();
            numberOfTechnologies.Should().Be(technologies.Count);
        }

        [Fact]
        public async Task SuccessfullyPatchesAnExistingTechnology()
        {
            // Arrange
            var pathParameters = _fixture.Build<TechnologyResponseObject>().With(x => x.Id, Guid.Empty).Create();
            var bodyParameters = _fixture.Create<PatchTechnologyItem>();
            var technology = _fixture.Build<Technology>()
                              .With(x => x.Id, pathParameters.Id)
                              .Create();

            technology.ToDatabase();
            _mockGateway.Setup(x => x.GetTechnologyById(pathParameters.Id)).ReturnsAsync(technology);

            //Act
            var result = await _classUnderTest.Execute(pathParameters, bodyParameters).ConfigureAwait(false);

            //Assert
            _mockGateway.Verify(x => x.SaveTechRadar(It.IsAny<Technology>()), Times.Once());

            result.technology.Should().Contain(x => x.Id == pathParameters.Id);
            result.technology.Should().Contain(x => x.Name == bodyParameters.Name);
            result.technology.Should().Contain(x => x.Description == bodyParameters.Description);
            result.technology.Should().Contain(x => x.Category == bodyParameters.Category);
            result.technology.Should().Contain(x => x.Technique == bodyParameters.Technique);
        }

        [Fact]
        public void PatchTechnologyUseCaseAsyncExceptionIsThrown()
        {
            // Arrange
            var pathParameters = _fixture.Build<TechnologyResponseObject>().With(x => x.Id, Guid.Empty).Create();
            var bodyParameters = _fixture.Create<PatchTechnologyItem>();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.PatchTechnologyById(pathParameters.Id)).ThrowsAsync(exception);

            // Act
            Func<Task<Technology>> func = async () => await _classUnderTest.Execute(pathParameters, bodyParameters).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }

        [Fact]
        public async Task UpdateTechnologyUseCaseReturnsNullIfTechnologyDoesNotExist()
        {
            // Arrange
            var pathParameters = _fixture.Build<TechnologyResponseObject>().With(x => x.Id, Guid.Empty).Create();
            var bodyParameters = _fixture.Create<PatchTechnologyItem>();
            _mockGateway.Setup(x => x.GetTechnologyById(pathParameters.Id)).ReturnsAsync((Technology) null);

            // Act
            var result = await _classUnderTest.Execute(pathParameters, bodyParameters).ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }


    }
}