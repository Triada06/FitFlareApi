using FluentValidation;

namespace FitFlare.Application.DTOs.Notification;

public class CreateNotificationRequest
{
    public required string NotificationType { get; set; }
    public required string AddressedUserId { get; set; }
    public required string TriggeredUserId { get; set; }
    public string? PostId { get; set; }
    public string? PostMediaUri { get; set; }
}

public class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(m => m.NotificationType).NotEmpty().WithMessage("NotificationType is required");
        RuleFor(m => m.AddressedUserId).NotEmpty().WithMessage("AddressedUserId is required");
        RuleFor(m => m.TriggeredUserId).NotEmpty().WithMessage("TriggeredUserId is required");
    }
}