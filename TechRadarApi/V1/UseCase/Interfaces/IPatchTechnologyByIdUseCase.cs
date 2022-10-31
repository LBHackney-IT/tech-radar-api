  using TechRadarApi.V1.Boundary.Response;
  using TechRadarApi.V1.Boundary.Request;
  using System;
  using System.Threading.Tasks;

  namespace TechRadarApi.V1.UseCase.Interfaces
  {
    public interface IPatchTechnologyByIdUseCase
    {
      Task Execute(Guid Id, PatchTechnologyRequest request);
    }
  }