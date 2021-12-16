using FluentValidation;

namespace Messaging.Application.Messages.Commands.CreateMessage;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(v => v.ToEmailAddress)
            .NotEmpty().WithMessage("To Email Address is required.")
            .MaximumLength(200).WithMessage("To Email Address must not exceed 200 characters.")
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("To Email Address must be a valid email address");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(4000).WithMessage("Content must not exceed 4000 characters.");
    }
}