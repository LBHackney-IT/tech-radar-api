using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace TechRadarApi.V1.UseCase
{
    public class DeleteTechnologyByIdUseCase : IDeleteTechnologyByIdUseCase
    {
        private ITechnologyGateway _gateway;
        public DeleteTechnologyByIdUseCase(ITechnologyGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<TechnologyResponseObject> Execute(Guid id)
        {
            var technology = await _gateway.GetTechnologyById(id).ConfigureAwait(false);
            if (technology == null) return null;

            await _gateway.DeleteTechnologyById(technology).ConfigureAwait(false);

            return technology?.ToResponse();
        }
    }
}