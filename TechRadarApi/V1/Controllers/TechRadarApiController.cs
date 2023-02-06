using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
        private readonly IDeleteTechnologyByIdUseCase _deleteTechnologyByIdUseCase;
        public TechRadarApiController(IGetAllTechnologiesUseCase getAllUseCase, IGetTechnologyByIdUseCase getByIdUseCase, IDeleteTechnologyByIdUseCase deleteTechnologyByIdUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _getByIdUseCase = getByIdUseCase;
            _deleteTechnologyByIdUseCase = deleteTechnologyByIdUseCase;
        }

        [ProducesResponseType(typeof(TechnologyResponseObjectList), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> ListTechnologies()
        {
            var result = await _getAllUseCase.Execute().ConfigureAwait(false);
            return Ok(result);
        }

        [ProducesResponseType(typeof(TechnologyResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ViewTechnology(Guid Id)
        {
            var result = await _getByIdUseCase.Execute(Id).ConfigureAwait(false);
            if (result == null) return NotFound(Id);
            return Ok(result);
        }

        [ProducesResponseType(typeof(TechnologyResponseObject), StatusCodes.Status200OK)]
        [HttpDelete]
        [Route("{id}")]

        public async Task<IActionResult> DeleteTechnology(Guid Id)
        {
            var response = await _deleteTechnologyByIdUseCase.Execute(Id).ConfigureAwait(false);
            if (response == null) return NotFound();

            return Ok(response);
        }

    }
}
