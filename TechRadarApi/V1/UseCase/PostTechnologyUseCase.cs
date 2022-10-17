using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace TechRadarApi.V1.UseCase
{
    public class PostTechnologyUseCase : IPostTechnologyUseCase
    {
        private ITechnologyGateway _gateway;

        public PostTechnologyUseCase(ITechnologyGateway gateway)

        {
            _gateway = gateway;
        }

        // public async Task<TechnologyRepsponseObject> Execute(Guid id)

        // {
    
        // }
    }
}