using FluentValidation;
using urlshortner.DTO;

public class CreateUrlRequestValidator : AbstractValidator<CreateUrlRequest>
{
    public CreateUrlRequestValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid URL");
    }
}