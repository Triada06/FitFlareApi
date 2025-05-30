using FluentValidation;

namespace FitFlare.Application.DTOs.Posts;

public class PostUpdateDto
{
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}

public class PostUpdateDtoValidator : AbstractValidator<PostUpdateDto>
{
    public PostUpdateDtoValidator()
    {
        RuleFor(m => m.Description)
            .MaximumLength(150).WithMessage("Description must not exceed 150 characters");
    }
}