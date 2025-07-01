using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.DTOs.Story;

public class StoryCreateDto
{
    public IFormFile Media { get; set; }
}

public class StoryCreateDtoValidator : AbstractValidator<StoryCreateDto>
{
    private const long MaxFileSize = 15 * 1024 * 1024;

    public StoryCreateDtoValidator()
    {
        RuleFor(m => m.Media)
            .NotNull().WithMessage("Media is required")
            .NotEmpty().WithMessage("Media is required")
            .Must(file => file.Length <= MaxFileSize)
            .WithMessage("Media size must not exceed 15 MB");
    }
}