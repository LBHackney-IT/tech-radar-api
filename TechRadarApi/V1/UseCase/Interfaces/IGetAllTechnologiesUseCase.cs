using TechRadarApi.V1.Boundary.Response;

namespace TechRadarApi.V1.UseCase.Interfaces
{
    public interface IGetAllTechnologiesUseCase
    {
        TechnologyResponseObjectList Execute();
    }
}