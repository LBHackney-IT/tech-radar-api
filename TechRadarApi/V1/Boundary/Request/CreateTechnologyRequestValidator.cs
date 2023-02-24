using FluentValidation;
using Hackney.Core.Validation;

namespace TechRadarApi.V1.Boundary.Request;

public class CreateTechnologyRequestValidator : AbstractValidator<CreateTechnologyRequest>
{
    public CreateTechnologyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotXssString();
        RuleFor(x => x.Description).NotEmpty().NotXssString();
        RuleFor(x => x.Category).NotEmpty().NotXssString();
        RuleFor(x => x.Technique).NotEmpty().NotXssString();
    }
}
