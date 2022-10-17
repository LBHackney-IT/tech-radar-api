  using TechRadarApi.V1.Boundary.Response;
  using System.Threading.Tasks;

  namespace TechRadarApi.V1.UseCase.Interfaces{
   public interface IPostTechnologyUseCase
    {
        Task<TechnologyResponseObjectList> Execute();
    }
  }
 