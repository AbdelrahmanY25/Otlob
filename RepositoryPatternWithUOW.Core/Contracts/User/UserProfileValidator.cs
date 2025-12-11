namespace Otlob.Core.Contracts.User;

public class UserProfileValidator : AbstractValidator<UserProfile>
{
    public UserProfileValidator()
    {
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
