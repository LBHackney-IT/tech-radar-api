using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Boundary.Request;

namespace TechRadarApi.V1.UseCase
{
    public class PatchTechnologyByIdUseCase : IPatchTechnologyByIdUseCase
    {
        private ITechnologyGateway _gateway;
        public PatchTechnologyByIdUseCase(ITechnologyGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<Technology> Execute(TechnologyResponseObject pathParameters, PatchTechnologyItem bodyParameters)
        {
            var technology = await _gateway.GetTechnologyById(pathParameters.Id).ConfigureAwait(false);
            if (technology == null)
                return null;

            var TechnologyData = new Technology()
            {
                Id = (Guid) pathParameters.Id,
                Name = bodyParameters.Name ?? technology.Name,
                Description = bodyParameters.Description ?? technology.Description,
                Category = bodyParameters.Category ?? technology.Category,
                Technique = bodyParameters.Technique ?? technology.Technique
            };

            await _gateway.PatchTechnology(TechnologyData).ConfigureAwait(false);
            return technology;
        }
    }
}