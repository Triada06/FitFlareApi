using FluentValidation;

namespace FitFlare.Application.DTOs.Account;

public class ForgotPasswordDto
{
    public required string Email { get; set; }
}

public class EmailDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public EmailDtoValidator()
    {
        RuleFor(m=>m.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");
    }
}