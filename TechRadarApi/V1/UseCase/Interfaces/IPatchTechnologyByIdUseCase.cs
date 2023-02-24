using TechRadarApi.V1.Boundary.Response;
using System.Threading.Tasks;
using System;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Boundary.Request;

namespace TechRadarApi.V1.UseCase.Interfaces
{
    public interface IPatchTechnologyByIdUseCase
    {
        Task<Technology> Execute(TechnologyResponseObject pathParameters, PatchTechnologyItem bodyParameters);
    }
}
