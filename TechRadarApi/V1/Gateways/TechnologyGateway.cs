using Amazon.DynamoDBv2.DataModel;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechRadarApi.V1.Gateways
{
    public class TechnologyGateway : ITechnologyGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public TechnologyGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task<List<Technology>> GetAll()
        {
            var conditions = new List<ScanCondition>();
            var results = await _dynamoDbContext.ScanAsync<TechnologyDbEntity>(conditions).GetRemainingAsync().ConfigureAwait(false);
            return results.Select(x => x.ToDomain()).ToList();
        }

        public async Task<Technology> GetTechnologyById(Guid id)
        {
            var result = await _dynamoDbContext.LoadAsync<TechnologyDbEntity>(id.ToString()).ConfigureAwait(false);
            return result?.ToDomain();
        }

        public async Task DeleteTechnologyById(Technology technology)
        {
            await _dynamoDbContext.DeleteAsync<TechnologyDbEntity>(technology).ConfigureAwait(false);
        }

        public async Task SaveTechRadar(Technology technology)
        {
            var databaseTechnology = technology.ToDatabase();
            await _dynamoDbContext.SaveAsync<TechnologyDbEntity>(databaseTechnology).ConfigureAwait(false);
        }
    }
}
