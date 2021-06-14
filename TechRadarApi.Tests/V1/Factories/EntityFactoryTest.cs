using AutoFixture;
using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using TechRadarApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace TechRadarApi.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = _fixture.Build<TechnologyDbEntity>()
                .With(technology => technology.Id, Guid.NewGuid().ToString())
                .Create();
            var entity = databaseEntity.ToDomain();

            databaseEntity.Id.Should().Be(entity.Id.ToString());
            databaseEntity.Name.Should().Be(entity.Name);
            databaseEntity.Description.Should().Be(entity.Description);
            databaseEntity.Category.Should().Be(entity.Category);
            databaseEntity.Technique.Should().Be(entity.Technique);
        }

        [Test]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var entity = _fixture.Create<Technology>();
            var databaseEntity = entity.ToDatabase();

            entity.Id.Should().Be(Guid.Parse(databaseEntity.Id));
            entity.Name.Should().Be(databaseEntity.Name);
            entity.Description.Should().Be(databaseEntity.Description);
            entity.Category.Should().Be(databaseEntity.Category);
            entity.Technique.Should().Be(databaseEntity.Technique);
        }
    }
}
