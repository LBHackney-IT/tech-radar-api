using System.Collections.Generic;
using TechRadarApi.V1.Domain;

namespace TechRadarApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
