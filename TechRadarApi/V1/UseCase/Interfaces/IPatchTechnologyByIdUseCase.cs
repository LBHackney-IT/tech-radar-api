using TechRadarApi.V1.Boundary.Response;
using System.Threading.Tasks;
using System;
using TechRadarApi.V1.Domain;

namespace TechRadarApi.V1.UseCase.Interfaces
{
    public interface IPatchTechnologyByIdUseCase
    {
        Task<Technology> Execute(PatchByIdRequest pathParameters, UpdateTechnologyListItem bodyParameters);
    }
}
