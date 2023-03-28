using System.Collections.Generic;
using TechRadarApi.V1.Domain;
using System;
using System.Threading.Tasks;
using TechRadarApi.V1.Boundary.Request;

namespace TechRadarApi.V1.Gateways
{
    public interface ITechnologyGateway
    {
        Task<Technology> GetTechnologyById(Guid id);
        Task<List<Technology>> GetAll();
        Task<Technology> PostNewTechnology(CreateTechnologyRequest createTechnologyRequest);
        Task DeleteTechnologyById(Technology technology);
        Task<Technology> PatchTechnology(Technology technology);
    }
}
