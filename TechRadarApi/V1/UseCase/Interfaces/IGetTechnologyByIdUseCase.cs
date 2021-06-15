using TechRadarApi.V1.Boundary.Response;
using System;

namespace TechRadarApi.V1.UseCase.Interfaces
{
    public interface IGetTechnologyByIdUseCase
    {
        TechnologyResponseObject Execute(Guid id);
    }
}
