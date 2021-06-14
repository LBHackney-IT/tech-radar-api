using AutoFixture;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Infrastructure;

namespace TechRadarApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static TechnologyDbEntity CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<Technology>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static TechnologyDbEntity CreateDatabaseEntityFrom(Technology entity)
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
