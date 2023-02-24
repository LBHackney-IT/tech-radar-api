using System;
using TechRadarApi.V1.Boundary.Request;
using TechRadarApi.V1.Infrastructure;

namespace TechRadarApi.V1.Factories
{
    public static class RequestFactory
    {
        public static TechnologyDbEntity ToDatabase(this CreateTechnologyRequest request)
        {
            return new TechnologyDbEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Technique = request.Technique
            };
        }
    }
}
