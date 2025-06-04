using FluentValidation;

namespace FitFlare.Application.DTOs.Comment;

public class CommentCreateDto
{
    public required string Content { get; set; }
    public required string PostId { get; set; }
}

public class CommentCreateDtoValidator : AbstractValidator<CommentCreateDto>
{
    public CommentCreateDtoValidator()
    {
        RuleFor(m=>m.Content)
            .NotEmpty().WithMessage("Content cannot be empty")
            .MaximumLength(150).WithMessage("Comment cannot be longer than 150 characters");
        RuleFor(m => m.PostId)
            .NotEmpty().WithMessage("PostId cannot be empty");
    }
}