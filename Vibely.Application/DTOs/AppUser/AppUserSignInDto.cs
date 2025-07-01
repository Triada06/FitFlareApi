using FluentValidation;

namespace FitFlare.Application.DTOs.AppUser;

public class AppUserSignInDto
{
    public required string EmailOrUserName { get; set; }
    public required string PassWord { get; set; }
}

public class AppUserSignInDtoValidator : AbstractValidator<AppUserSignInDto>
{
    public AppUserSignInDtoValidator()
    {
        RuleFor(m=>m.EmailOrUserName)
            .NotNull().WithMessage("Password can't be null")
            .NotEmpty().WithMessage("Password format is invalid.");
        RuleFor(model => model.PassWord)
            .NotNull().WithMessage("Password can't be null")
            .NotEmpty().WithMessage("Password is required.");
    }
}