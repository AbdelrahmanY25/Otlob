namespace Otlob.Core.Contracts.Branch;

public class BranchRequestValidator : AbstractValidator<BranchRequest>
{
    public BranchRequestValidator()
    {
        RuleFor(br => br.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(RegexPattern.BranchName);

        RuleFor(br => br.Address)
            .NotEmpty()
            .MaximumLength(150)
            .Matches(RegexPattern.Address);

        RuleFor(br => br.DeliveryRadiusKm)
            .NotEmpty()
            .GreaterThan(0)
            .LessThanOrEqualTo(50);

        RuleFor(br => br.MangerName)
            .NotEmpty() 
            .MaximumLength(50)
            .Matches(RegexPattern.Name);

        RuleFor(br => br.MangerPhone)
            .NotEmpty()
            .Length(11)
            .Matches(RegexPattern.UsersPhoneNumber);

        RuleFor(br => br.LonCode)
            .NotEmpty()
            .InclusiveBetween(-180.0, 180.0);

        RuleFor(br => br.LatCode)
            .NotEmpty()
            .InclusiveBetween(-90.0, 90.0);
    }
}
