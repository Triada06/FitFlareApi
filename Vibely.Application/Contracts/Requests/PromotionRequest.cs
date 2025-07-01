using FluentValidation;

namespace FitFlare.Application.Contracts.Requests;

public class PromotionRequest
{
    public required string Email { get; set; }
}

public class PromotionRequestValidator : AbstractValidator<PromotionRequest>
{
    public PromotionRequestValidator()
    {
        RuleFor(m=>m.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is required");
    }
}