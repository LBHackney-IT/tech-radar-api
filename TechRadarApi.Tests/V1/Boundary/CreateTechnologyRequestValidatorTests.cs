using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using TechRadarApi.V1.Boundary.Request;
using Xunit;

namespace TechRadarApi.Tests.V1.Boundary;

public class CreateTechnologyRequestValidatorTests
{
    private readonly CreateTechnologyRequestValidator _classUnderTest;
    private readonly Fixture _fixture;

    public CreateTechnologyRequestValidatorTests()
    {
        _classUnderTest = new CreateTechnologyRequestValidator();
        _fixture = new Fixture();
    }

    [Fact]
    public void ValidRequestDoesNotError()
    {
        var request = _fixture.Create<CreateTechnologyRequest>();
        var result = _classUnderTest.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("<string with tags in it>")]
    public void RequestErrorsIfNameIsInvalid(string value)
    {
        var request = _fixture.Create<CreateTechnologyRequest>();
        request.Name = value;
        var result = _classUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("<string with tags in it>")]
    public void RequestErrorsIfDescriptionIsInvalid(string value)
    {
        var request = _fixture.Create<CreateTechnologyRequest>();
        request.Description = value;
        var result = _classUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("<string with tags in it>")]
    public void RequestErrorsIfCategoryIsInvalid(string value)
    {
        var request = _fixture.Create<CreateTechnologyRequest>();
        request.Category = value;
        var result = _classUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Theory]
    [InlineData("")]
    [InlineData("<string with tags in it>")]
    public void RequestErrorsIfTechniqueIsInvalid(string value)
    {
        var request = _fixture.Create<CreateTechnologyRequest>();
        request.Technique = value;
        var result = _classUnderTest.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Technique);
    }
}
