namespace Otlob.Core.Contracts.VMValidations;

public class ProfileVMValidator : AbstractValidator<ProfileVM>
{
    public ProfileVMValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty()
            .Length(10, 100)
            .Matches(RegexPattern.Email)
            .WithErrorCode(ValidationErrorMessages.Email)
            .EmailAddress();

        RuleFor(p => p.FirstName)
            .Length(3, 15)
            .When(p => !string.IsNullOrEmpty(p.FirstName))
            .Matches(RegexPattern.Name);

        RuleFor(p => p.LastName)
            .Length(3, 15)
            .When(p => !string.IsNullOrEmpty(p.LastName))
            .Matches(RegexPattern.Name);

        RuleFor(p => p.PhoneNumber)
            .NotEmpty()
            .Matches(RegexPattern.UsersPhoneNumber)
            .WithMessage(ValidationErrorMessages.PhoneNumber);

        RuleFor(p => p.BirthDate)
            .LessThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-5)))
            .When(p => p.BirthDate.HasValue);

        RuleFor(p => p.Gender)
            .IsInEnum()
            .Must(p => p == Gender.Female || p == Gender.Male);
    }
}
