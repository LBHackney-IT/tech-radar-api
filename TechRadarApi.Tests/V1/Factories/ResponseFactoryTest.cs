using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using NUnit.Framework;
using FluentAssertions;

namespace TechRadarApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        [Test]
        public void CanMapAnEntityToAResponseObject()
        {
            var entity = new Technology();
            var response = entity.ToResponse();

            response.Id.Should().Be(entity.Id);
            response.Name.Should().Be(entity.Name);
            response.Description.Should().Be(entity.Description);
            response.Category.Should().Be(entity.Category);
            response.Technique.Should().Be(entity.Technique);
        }
    }
}
