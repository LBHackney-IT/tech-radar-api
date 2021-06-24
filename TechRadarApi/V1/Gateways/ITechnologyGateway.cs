using System.Collections.Generic;
using TechRadarApi.V1.Domain;
using System;
using System.Threading.Tasks;

namespace TechRadarApi.V1.Gateways
{
    public interface ITechnologyGateway
    {
        Task<Technology> GetTechnologyById(Guid id);

        Task<List<Technology>> GetAll();
    }
}
