using Amazon.DynamoDBv2.DataModel;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace TechRadarApi.V1.Gateways
{
    public class DynamoDbGateway : IExampleGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public List<Technology> GetAll()
        {
            var conditions = new List<ScanCondition>();
            var result = _dynamoDbContext.ScanAsync<TechnologyDbEntity>(conditions).GetRemainingAsync().GetAwaiter().GetResult();

            var response = new List<Technology>();
            result.ForEach(delegate (TechnologyDbEntity technology) { response.Add(technology?.ToDomain()); });
            return response;
        }

        public Technology GetEntityById(Guid id)
        {
            var result = _dynamoDbContext.LoadAsync<TechnologyDbEntity>(id).GetAwaiter().GetResult();
            return result?.ToDomain();
        }
    }
}
