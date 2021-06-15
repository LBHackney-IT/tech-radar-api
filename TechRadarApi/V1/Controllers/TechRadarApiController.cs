using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TechRadarApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/technologies")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TechRadarApiController : BaseController
    {
        private readonly IGetAllTechnologiesUseCase _getAllUseCase;
        private readonly IGetTechnologyByIdUseCase _getByIdUseCase;
        public TechRadarApiController(IGetAllTechnologiesUseCase getAllUseCase, IGetTechnologyByIdUseCase getByIdUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _getByIdUseCase = getByIdUseCase;
        }

        [ProducesResponseType(typeof(TechnologyResponseObjectList), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public IActionResult ListTechnologies()
        {
            var result = _getAllUseCase.Execute();
            if (result.Technologies.Count == 0) return NoContent();
            return Ok(result);
        }

        [ProducesResponseType(typeof(TechnologyResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        [Route("{id}")]
        public IActionResult ViewTechnology(Guid Id)
        {
            var result = _getByIdUseCase.Execute(Id);
            if (result == null) return NotFound(Id);
            return Ok(result);
        }
    }
}
