using TechRadarApi.V1.Boundary.Response;

namespace TechRadarApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ResponseObject Execute(int id);
    }
}
