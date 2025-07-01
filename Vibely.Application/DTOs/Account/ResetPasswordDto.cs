using FluentValidation;

namespace FitFlare.Application.DTOs.Account;

public class ResetPasswordDto
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(m => m.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");
        RuleFor(m => m.Token).NotEmpty().WithMessage("Token is required");
        RuleFor(m => m.NewPassword).NotEmpty().WithMessage("NewPassword is required");
        RuleFor(m => m.ConfirmPassword).NotEmpty().WithMessage("ConfirmPassword is required");
    }
}