using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System;

namespace TechRadarApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetClaimantByIdUseCase
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private IExampleGateway _gateway;
        public GetByIdUseCase(IExampleGateway gateway)
        {
            _gateway = gateway;
        }
        public TechnologyResponseObject Execute(Guid id)
        {
            return _gateway.GetEntityById(id).ToResponse();
        }
    }
}
