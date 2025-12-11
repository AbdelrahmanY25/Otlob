namespace Otlob.Core.Contracts.Files;

public class FileSizeValidator : AbstractValidator<IFormFile>
{
    public FileSizeValidator()
    {
        RuleFor(f => f)
            .Must(f => f.Length <= FileSettings.MaxFileSizeInBytes)
            .WithMessage($"File size must not exceed {FileSettings.MaxFileSizeInMB} MB.")
            .When(f => f is not null);
    }
}
