using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Boundary.Request;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace TechRadarApi.V1.UseCase
{
    public class PatchTechnologyByIdUseCase : IPatchTechnologyByIdUseCase
    {
        private ITechnologyGateway _gateway;
        public PatchTechnologyByIdUseCase(ITechnologyGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task Execute(Guid id, PatchTechnologyRequest request)
        {
           await _gateway.PatchTechnology(id, request);
        }
    }
}