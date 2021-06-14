using System;
using TechRadarApi.V1.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace TechRadarApi.Tests.V1.Domain
{
    [TestFixture]
    public class TechnologyTests
    {

        [Test]
        public void TechnologiesHavePropertiesSet()
        {
            var technology = new Technology();
            var name = "C# = .NET Core";
            var description = "C# is the language that most of our projects at Hackney are written in, using the .Net framework.";
            var category = "Languages and Frameworks";
            var technique = "Adopt";
            technology.Name = name;
            technology.Description = description;
            technology.Category = category;
            technology.Technique = technique;

            technology.Name.Should().Be(name);
            technology.Description.Should().Be(description);
            technology.Category.Should().Be(category);
            technology.Technique.Should().Be(technique);
        }
    }
}
