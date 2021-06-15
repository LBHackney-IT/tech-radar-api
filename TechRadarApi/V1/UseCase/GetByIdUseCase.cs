using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System;

namespace TechRadarApi.V1.UseCase
{
    public class GetTechnologyByIdUseCase : IGetTechnologyByIdUseCase
    {
        private ITechnologyGateway _gateway;
        public GetTechnologyByIdUseCase(ITechnologyGateway gateway)
        {
            _gateway = gateway;
        }
        public TechnologyResponseObject Execute(Guid id)
        {
            return _gateway.GetTechnologyById(id).ToResponse();
        }
    }
}
