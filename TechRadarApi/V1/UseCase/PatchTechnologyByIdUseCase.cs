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
            var technology = await _gateway.GetAll().ConfigureAwait(false);
            if (technology == null)
                return null;
            if (pathParameters.Id != Guid.Empty)
            {
                var doesTechnologyExist = technology.Find(x => x.Id == pathParameters.Id);
                if (doesTechnologyExist == null) return null;
                technology.Remove(doesTechnologyExist);

                var TechnologyData = new Technology()
                {
                    Id = (Guid) pathParameters.Id,
                    Name = bodyParameters.Name ?? doesTechnologyExist.Name,
                    Description = bodyParameters.Description ?? doesTechnologyExist.Description,
                    Category = bodyParameters.Category ?? doesTechnologyExist.Category,
                    Technique = bodyParameters.Technique ?? doesTechnologyExist.Technique
                };

                technology.Add(TechnologyData);
            }
            else
            {
                var TechnologyData = new Technology()
                {
                    Id = Guid.NewGuid(),
                    Name = bodyParameters.Name,
                    Description = bodyParameters.Description,
                    Category = bodyParameters.Category,
                    Technique = bodyParameters.Technique
                };
                technology.Add(TechnologyData);
            }
            await _gateway.SaveTechRadar(technology).ConfigureAwait(false);
            return technology;
        }
    }
}