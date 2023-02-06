using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;
using TechRadarApi.V1.Domain;

namespace TechRadarApi.V1.UseCase
{
    public class PatchTechnologyByIdUseCase : IPatchTechnologyByIdUseCase
    {
        public async Task<Technology> Execute(PatchByIdRequest pathParameters, UpdateTechnologyListItem bodyParameters)
        {
            
        }
    }
}