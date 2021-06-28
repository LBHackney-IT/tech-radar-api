using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Infrastructure;
using System;

namespace TechRadarApi.V1.Factories
{
    public static class EntityFactory
    {
        public static Technology ToDomain(this TechnologyDbEntity databaseEntity)
        {
            return new Technology
            {
                Id = Guid.Parse(databaseEntity.Id),
                Name = databaseEntity.Name,
                Description = databaseEntity.Description,
                Category = databaseEntity.Category,
                Technique = databaseEntity.Technique
            };
        }

        public static TechnologyDbEntity ToDatabase(this Technology entity)
        {
            return new TechnologyDbEntity
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Description = entity.Description,
                Category = entity.Category,
                Technique = entity.Technique
            };
        }
    }
}
