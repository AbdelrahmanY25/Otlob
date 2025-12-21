namespace Otlob.Core.Contracts.Files;

public class FileSignatureForFilesValidator : AbstractValidator<IFormFile>
{
    public FileSignatureForFilesValidator()
    {
        RuleFor(f => f)
            .Must(f =>
            {
                using var stream = f.OpenReadStream();
                using var reader = new BinaryReader(stream);
                
                // Read first 4 bytes for better signature detection
                var bytes = reader.ReadBytes(Math.Min(4, (int)stream.Length));
                
                if (bytes.Length < 2)
                    return false;

                var fileSignature = BitConverter.ToString(bytes);

                // Validate it's an allowed file type (jpg, png, jpeg, pdf)
                return FileSettings.AllowedFileSignatures.Any(allowed => fileSignature.StartsWith(allowed));
            })
            .WithMessage("File type is not allowed or signature is invalid.")
            .When(x => x is not null);
    }
}
