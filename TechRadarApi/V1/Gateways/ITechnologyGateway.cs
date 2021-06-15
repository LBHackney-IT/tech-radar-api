using System.Collections.Generic;
using TechRadarApi.V1.Domain;
using System;

namespace TechRadarApi.V1.Gateways
{
    public interface ITechnologyGateway
    {
        Technology GetTechnologyById(Guid id);

        List<Technology> GetAll();
    }
}
