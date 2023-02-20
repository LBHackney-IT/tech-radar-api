using TechRadarApi.V1.Boundary.Response;
using System.Threading.Tasks;
using System;

namespace TechRadarApi.V1.UseCase.Interfaces
{
    public interface IDeleteTechnologyByIdUseCase
    {
        Task<TechnologyResponseObject> Execute(Guid id);
    }
}
