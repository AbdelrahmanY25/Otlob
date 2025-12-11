namespace Otlob.Core.Contracts.Files;

public class UploadImageRequestValidator : AbstractValidator<UploadImageRequest>
{
    public UploadImageRequestValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .SetValidator(new FileSizeValidator())
            .SetValidator(new FileNameValidator())
            .SetValidator(new FileSignatureValidator())
            .Must(i => FileSettings.AllowedImageExtensions.Contains(Path.GetExtension(i.FileName), StringComparer.InvariantCultureIgnoreCase))
            .WithMessage($"File extension is not allowed. Allowed extensions are: {string.Join(", ", FileSettings.AllowedImageExtensions)}")
            .When(x => x.Image is not null);
    }
}
