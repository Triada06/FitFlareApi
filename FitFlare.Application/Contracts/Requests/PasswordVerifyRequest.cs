using FluentValidation;

namespace FitFlare.Application.Contracts.Requests;

public class PasswordVerifyRequest
{
    public required string Password { get; set; }
    /* public required string Purpose { get; set; } */
}

public class PasswordVerifyRequestValidator : AbstractValidator<PasswordVerifyRequest>
{
    public PasswordVerifyRequestValidator()
    {
        RuleFor(m => m.Password)
            .NotEmpty().WithMessage("Password is required");
        /*RuleFor(m => m.Purpose)
            .NotEmpty().WithMessage("Purpose is required")
            .Must(p => p is "changePassword" or "deleteAccount")
            .WithMessage("Invalid purpose");*/
    }
}