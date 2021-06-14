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
        private readonly IGetAllUseCase _getAllUseCase;
        private readonly IGetByIdUseCase _getByIdUseCase;
        public TechRadarApiController(IGetAllUseCase getAllUseCase, IGetByIdUseCase getByIdUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _getByIdUseCase = getByIdUseCase;
        }

        [ProducesResponseType(typeof(TechnologyResponseObjectList), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListTechnologies()
        {
            return Ok(_getAllUseCase.Execute());
        }

        [ProducesResponseType(typeof(TechnologyResponseObject), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("{id}")]
        public IActionResult ViewRecord(Guid yourId)
        {
            return Ok(_getByIdUseCase.Execute(yourId));
        }
    }
}
