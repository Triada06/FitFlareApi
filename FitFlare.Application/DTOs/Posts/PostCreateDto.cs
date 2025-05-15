using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.DTOs.Posts;

public class PostCreateDto
{
    public string? Description { get; set; }
    public required IFormFile Media { get; set; }
}

public class PostDtoValidator : AbstractValidator<PostCreateDto>
{
    public PostDtoValidator()
    {
        RuleFor(m => m.Description)
            .NotEmpty().WithMessage("Description is required")
            .NotNull().WithMessage("Description is required")
            .MaximumLength(150).WithMessage("Description must not exceed 150 characters");
        RuleFor(m => m.Media)
            .NotEmpty().WithMessage("Media is required")
            .NotNull().WithMessage("Media is required");
    }
}