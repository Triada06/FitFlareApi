using FluentValidation;

namespace FitFlare.Application.DTOs.Ban;

public class BanCreateDto
{
    public required string UserId { get; set; }
    public required string Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class BanCreateDtoValidator : AbstractValidator<BanCreateDto>
{
    public BanCreateDtoValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(50);
        RuleFor(x => x.ExpiresAt)
            .Must(date => date == null || date > DateTime.UtcNow)
            .WithMessage("Expiration time must be in the future.");
    }
}