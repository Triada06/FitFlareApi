using FluentValidation;

namespace FitFlare.Application.DTOs.Ban;

public class BanUpdateDto
{
    public required string Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
public class BanUpdateDtoValidator : AbstractValidator<BanUpdateDto>
{
    public BanUpdateDtoValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(50);
        RuleFor(x => x.ExpiresAt)
            .Must(date => date == null || date > DateTime.UtcNow)
            .WithMessage("Expiration time must be in the future.");
    }
}