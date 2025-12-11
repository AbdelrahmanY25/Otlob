namespace Otlob.Core.Contracts.Files;

public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest>
{
    public UploadFileRequestValidator()
    {
        RuleFor(x => x.File)
           .NotNull()
           .SetValidator(new FileSizeValidator())
           .SetValidator(new FileNameValidator())
           .SetValidator(new FileSignatureValidator())
           .Must(i => FileSettings.AllowedFileExtensions.Contains(Path.GetExtension(i.FileName), StringComparer.InvariantCultureIgnoreCase))
           .WithMessage($"File extension is not allowed. Allowed extensions are: {string.Join(", ", FileSettings.AllowedFileExtensions)}")
           .When(x => x.File is not null);
    }
}
