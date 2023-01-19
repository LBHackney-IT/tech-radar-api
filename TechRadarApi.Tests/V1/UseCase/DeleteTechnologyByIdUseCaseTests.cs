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
using Microsoft.AspNetCore.Mvc;

namespace TechRadarApi.Tests.V1.UseCase
{
    public class DeleteTechnologyByIdUseCaseTests
    {
        private Mock<ITechnologyGateway> _mockGateway;
        private DeleteTechnologyByIdUseCase _classUnderTest;
        private Fixture _fixture;

        public DeleteTechnologyByIdUseCaseTests()
        {
            _mockGateway = new Mock<ITechnologyGateway>();
            _classUnderTest = new DeleteTechnologyByIdUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task DeleteTechnologyByIdUseCaseReturnsNullWhenTechnologyIsNotFound()
        {
            var query = _fixture.Build<TechnologyResponseObject>().With(x => x.Id).Create();

            var response = await _classUnderTest.Execute(query.Id).ConfigureAwait(false);
            response.Should().BeNull();
        }

        // TODO: complete once the post endpoint is done
        //[Fact]
        //public async Task DeleteTechnologyByIdUseCaseReturnsOkResponse()
        //{
        //    var technology = _fixture.Create<Technology>();
        //    var query = _fixture.Build<TechnologyResponseObject>().With(x => x.Id).Create();
            
            // will pass once post endpoint is created
            // _mockGateway.Setup(x => x.PostTechnologyById(technology));

        //    var response = await _classUnderTest.Execute(query.Id).ConfigureAwait(false);
        //    response.Should().BeOfType(typeof(NoContentResult));
        //}
    }
}
