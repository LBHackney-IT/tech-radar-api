using AutoFixture;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace TechRadarApi.Tests.V1.E2ETests
{
    public class TechnologyList
    {
        public List<Technology> Technologies { get; set; }
    }

    public class EndToEndTestsBase : DynamoDbIntegrationTests<Startup>
    {
        protected readonly Fixture _fixture = new Fixture();

        protected async Task SaveTestData(Technology entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync<TechnologyDbEntity>(entity.Id.ToString()).ConfigureAwait(false));
        }
    }

}
