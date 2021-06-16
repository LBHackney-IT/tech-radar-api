using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TechRadarApi.Tests.V1.E2ETests
{
    public class TechnologyList
    {
        public List<Technology> Technologies { get; set; }
    }

    public class EndToEndTestsBase : DynamoDbIntegrationTests<Startup>
    {
        protected async Task SaveTestData(Technology entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync<TechnologyDbEntity>(entity.Id.ToString()).ConfigureAwait(false));
        }
    }

}
