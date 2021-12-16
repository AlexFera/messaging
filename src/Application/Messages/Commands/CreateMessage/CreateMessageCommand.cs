using MediatR;
using Messaging.Application.Common.Interfaces;
using Messaging.Domain.Entities;
using Messaging.Domain.Events;

namespace Messaging.Application.Messages.Commands.CreateMessage;

public class CreateMessageCommand : IRequest<int>
{
    public string ToEmailAddress { get; set; } = null!;

    public string Content { get; set; } = null!;
}

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateMessageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sendingUser = _context.Users.FirstOrDefault(x => x.EmailAddress == _currentUserService.UserId);
        if (sendingUser == null)
        {
            sendingUser = new User { EmailAddress = _currentUserService.UserId };
            _context.Users.Add(sendingUser);
        }

        var receivingUser = _context.Users.FirstOrDefault(x => x.EmailAddress == request.ToEmailAddress);
        if (receivingUser == null)
        {
            receivingUser = new User { EmailAddress = request.ToEmailAddress };
            _context.Users.Add(receivingUser);
        }

        var messageThread = new MessageThread();
        _context.MessageThreads.Add(messageThread);

        await _context.SaveChangesAsync(cancellationToken);

        _context.MessageThreadParticipants.Add(new MessageThreadParticipant { MessageThreadId = messageThread.Id, UserId = sendingUser.Id });
        _context.MessageThreadParticipants.Add(new MessageThreadParticipant { MessageThreadId = messageThread.Id, UserId = receivingUser.Id });

        var entity = new Message
        {
            ToAddress = request.ToEmailAddress,
            FromAddress = _currentUserService.UserId!,
            Content = request.Content,
            SendingUserId = sendingUser.Id,
            MessageThreadId = messageThread.Id
        };

        entity.DomainEvents.Add(new MessageCreatedEvent(entity));

        _context.Messages.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

