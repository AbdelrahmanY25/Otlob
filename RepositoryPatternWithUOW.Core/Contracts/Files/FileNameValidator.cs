namespace Otlob.Core.Contracts.Files;

public class FileNameValidator : AbstractValidator<IFormFile>
{
    public FileNameValidator()
    {
        RuleFor(f => f.FileName)
            .NotEmpty()
            .MaximumLength(FileSettings.MaxFileNameLength)
            .WithMessage($"File name must not exceed {FileSettings.MaxFileNameLength} characters.")
            .Matches(@"^[^<>:;""/\\|?*]+$")
            .WithMessage("File name contains invalid characters.")
            .When(f => f is not null);
    }
}
