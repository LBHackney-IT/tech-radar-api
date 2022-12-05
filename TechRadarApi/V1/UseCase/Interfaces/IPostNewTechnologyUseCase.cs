using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Boundary.Request;
using System;
using System.Threading.Tasks;

namespace TechRadarApi.V1.UseCase.Interfaces
{
    public interface IPostNewTechnologyUseCase
    {
        Task<TechnologyResponseObject> Execute(CreateTechnologyRequest createTechnologyRequest);
    }
}
