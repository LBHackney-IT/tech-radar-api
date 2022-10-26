using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Boundary.Request;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace TechRadarApi.V1.UseCase
{
    public class PostNewTechnologyUseCase : IPostNewTechnologyUseCase
    {
        private ITechnologyGateway _gateway;
        
        public PostNewTechnologyUseCase(ITechnologyGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<TechnologyResponseObject> Execute(CreateTechnologyRequest createTechnologyRequest)
        {
           var technology = await _gateway.PostNewTechnology(createTechnologyRequest.ToDatabase()).ConfigureAwait(false);
           return technology.ToResponse();
        }

    }
}