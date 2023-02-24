using Amazon.DynamoDBv2.DataModel;
using System;

namespace TechRadarApi.V1.Infrastructure
{

    [DynamoDBTable("TechRadar", LowerCamelCaseProperties = true)]
    public class TechnologyDbEntity
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }
        [DynamoDBProperty]
        public string Category { get; set; }
        [DynamoDBProperty]
        public string Technique { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
    }
}
