using AutoFixture;
using System.Linq;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Gateways;
using TechRadarApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TechRadarApi.V1.Factories;
using System.Threading.Tasks;

namespace TechRadarApi.Tests.V1.Gateways
{
    [TestFixture]
    public class TechnologyGatewayTests : DynamoDbIntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();
        private TechnologyGateway _classUnderTest;

        protected async Task SaveTestData(Technology entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync<TechnologyDbEntity>(entity.Id.ToString()).ConfigureAwait(false));
        }
        private async void SetupTestData(List<Technology> entities)
        {
            var tasks = entities.Select(entity => SaveTestData(entity));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new TechnologyGateway(DynamoDbContext);
        }

        [Test]
        public void GetTechnologyByIdReturnsNullIfTechnologyDoesntExist()
        {
            // Assert
            var id = Guid.NewGuid();
            // Act
            var response = _classUnderTest.GetTechnologyById(id);
            // Assert
            response.Should().BeNull();
        }

        [Test]
        public async Task GetTechnologyByIdReturnsTheTechnologyIfItExists()
        {
            // Arrange
            var entity = _fixture.Create<Technology>();
            await SaveTestData(entity).ConfigureAwait(false);
            // Act
            var response = _classUnderTest.GetTechnologyById(entity.Id);
            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(entity.Id.ToString());
            response.Name.Should().Be(entity.Name);
            response.Description.Should().Be(entity.Description);
            response.Category.Should().Be(entity.Category);
            response.Technique.Should().Be(entity.Technique);
        }

        [Test]
        public void GetAllTechnologiesReturnsEmptyArrayIfNoTechnologiesExist()
        {
            // Arrange + Act
            var response = _classUnderTest.GetAll();
            // Assert
            response.Should().BeEmpty();
        }

        [Test]
        public void GetAllTechnologiesReturnsAnArrayOfAllTechnologiesInTheTable()
        {
            // Arrange
            var entities = _fixture.CreateMany<Technology>().ToList();
            SetupTestData(entities);
            // Act
            var response = _classUnderTest.GetAll();
            // Assert
            response.Should().BeEquivalentTo(entities);
        }
    }
}
