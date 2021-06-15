using Amazon.DynamoDBv2.DataModel;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TechRadarApi.V1.Gateways
{
    public class TechnologyGateway : ITechnologyGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public TechnologyGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public List<Technology> GetAll()
        {
            var conditions = new List<ScanCondition>();
            var results = _dynamoDbContext.ScanAsync<TechnologyDbEntity>(conditions).GetRemainingAsync().GetAwaiter().GetResult();
            return results.Select(x => x.ToDomain()).ToList();
        }

        public Technology GetTechnologyById(Guid id)
        {
            var result = _dynamoDbContext.LoadAsync<TechnologyDbEntity>(id.ToString()).GetAwaiter().GetResult();
            return result?.ToDomain();
        }
    }
}