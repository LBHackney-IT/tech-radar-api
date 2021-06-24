using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace TechRadarApi.V1.UseCase
{
    public class GetTechnologyByIdUseCase : IGetTechnologyByIdUseCase
    {
        private ITechnologyGateway _gateway;
        public GetTechnologyByIdUseCase(ITechnologyGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<TechnologyResponseObject> Execute(Guid id)
        {
            var technology = await _gateway.GetTechnologyById(id).ConfigureAwait(false);
            return technology.ToResponse();
        }
    }
}
