using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;

namespace TechRadarApi.V1.UseCase
{
    public class GetAllTechnologiesUseCase : IGetAllTechnologiesUseCase
    {
        private readonly ITechnologyGateway _gateway;
        public GetAllTechnologiesUseCase(ITechnologyGateway gateway)
        {
            _gateway = gateway;
        }

        public TechnologyResponseObjectList Execute()
        {
            return new TechnologyResponseObjectList { Technologies = _gateway.GetAll().ToResponse() };
        }
    }
}
