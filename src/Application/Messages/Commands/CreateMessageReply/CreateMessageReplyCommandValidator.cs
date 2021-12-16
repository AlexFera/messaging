using FluentValidation;
using Messaging.Application.Common.Interfaces;

namespace Messaging.Application.Messages.Commands.CreateMessageReply;

public class CreateMessageReplyCommandValidator : AbstractValidator<CreateMessageReplyCommand>
{
    public CreateMessageReplyCommandValidator()
    {
        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(4000).WithMessage("Content must not exceed 4000 characters.");

        RuleFor(x => x.MessageThreadId)
            .GreaterThan(0);
    }
}