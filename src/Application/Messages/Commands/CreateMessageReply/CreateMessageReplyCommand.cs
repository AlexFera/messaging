using MediatR;
using Microsoft.EntityFrameworkCore;
using Messaging.Application.Common.Interfaces;
using Messaging.Domain.Entities;
using Messaging.Domain.Events;

namespace Messaging.Application.Messages.Commands.CreateMessageReply;

public class CreateMessageReplyCommand : IRequest<int>
{
    public int MessageThreadId { get; set; }

    public string Content { get; set; } = null!;
}

public class CreateMessageReplyCommandHandler : IRequestHandler<CreateMessageReplyCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateMessageReplyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateMessageReplyCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sendingUser = await _context.Users
            .AsNoTracking()
            .FirstAsync(x => x.EmailAddress == _currentUserService.UserId, cancellationToken);

        var messageBeingReplied = await _context.Messages
            .AsNoTracking()
            .FirstAsync(x => x.MessageThreadId == request.MessageThreadId, cancellationToken);

        var entity = new Message
        {
            ToAddress = messageBeingReplied.ToAddress,
            FromAddress = _currentUserService.UserId!,
            Content = request.Content,
            SendingUserId = sendingUser.Id,
            MessageThreadId = request.MessageThreadId
        };

        entity.DomainEvents.Add(new MessageCreatedEvent(entity));

        _context.Messages.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
