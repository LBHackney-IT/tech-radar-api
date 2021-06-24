using TechRadarApi.V1.Domain;
using TechRadarApi.V1.Factories;
using Xunit;
using FluentAssertions;

namespace TechRadarApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        [Fact]
        public void CanMapAnEntityToAResponseObject()
        {
            // Arrange
            var entity = new Technology();

            // Act
            var response = entity.ToResponse();

            // Assert
            response.Id.Should().Be(entity.Id);
            response.Name.Should().Be(entity.Name);
            response.Description.Should().Be(entity.Description);
            response.Category.Should().Be(entity.Category);
            response.Technique.Should().Be(entity.Technique);
        }
    }
}
