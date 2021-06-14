using System.Collections.Generic;
using TechRadarApi.V1.Domain;
using System;

namespace TechRadarApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Technology GetEntityById(Guid id);

        List<Technology> GetAll();
    }
}
