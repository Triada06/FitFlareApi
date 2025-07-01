using FluentValidation;

namespace FitFlare.Application.Contracts.Requests;

public class PasswordChangeRequest
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}

public class PasswordChangeRequestValidator : AbstractValidator<PasswordChangeRequest>
{
    public PasswordChangeRequestValidator()
    {
        RuleFor(m=>m.OldPassword).NotEmpty().WithMessage("Password is required");
        RuleFor(model => model.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
    }
}