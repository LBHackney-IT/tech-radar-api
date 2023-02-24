using AutoFixture;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using FluentAssertions;
using Xunit;
using System;

namespace TechRadarApi.Tests.V1.Factories
{
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanMapADatabaseEntityToADomainObject()
        {
            // Arrange
            var databaseEntity = _fixture.Create<TechnologyDbEntity>();
            // Act    
            var entity = databaseEntity.ToDomain();

            //Assert
            entity.Id.Should().Be(databaseEntity.Id);
            entity.Name.Should().Be(databaseEntity.Name);
            entity.Description.Should().Be(databaseEntity.Description);
            entity.Category.Should().Be(databaseEntity.Category);
            entity.Technique.Should().Be(databaseEntity.Technique);
        }

        [Fact]
        public void CanMapADomainEntityToADatabaseObject()
        {
            // Arrange
            var entity = _fixture.Create<Technology>();

            // Act
            var databaseEntity = entity.ToDatabase();

            // Assert
            databaseEntity.Id.Should().Be(entity.Id.ToString());
            databaseEntity.Name.Should().Be(entity.Name);
            databaseEntity.Description.Should().Be(entity.Description);
            databaseEntity.Category.Should().Be(entity.Category);
            databaseEntity.Technique.Should().Be(entity.Technique);
        }
    }
}
