namespace Otlob.Core.Contracts.Files;

public class FileSignatureValidator : AbstractValidator<IFormFile>
{
    public FileSignatureValidator()
    {
        RuleFor(f => f)
            .Must(f =>
            {
                BinaryReader binaryReader = new(f.OpenReadStream());
                var bytes = binaryReader.ReadBytes(2);

                var fileSignature = BitConverter.ToString(bytes);

                return !FileSettings.BlockedSignatures.Contains(fileSignature);
            })
            .WithMessage("File type is not allowed.")
            .When(x => x is not null);
    }
}
