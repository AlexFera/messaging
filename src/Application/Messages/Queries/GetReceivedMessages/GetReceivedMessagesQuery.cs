using MediatR;
using Microsoft.EntityFrameworkCore;
using Messaging.Application.Common.Interfaces;
using Messaging.Application.Common.Mappings;
using Messaging.Application.Common.Models;

namespace Messaging.Application.Messages.Queries.GetReceivedMessages;

public class GetReceivedMessagesQuery : IRequest<PaginatedList<ReceivedMessageDto>>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

public class GetReceivedMessagesQueryHandler : IRequestHandler<GetReceivedMessagesQuery, PaginatedList<ReceivedMessageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetReceivedMessagesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<ReceivedMessageDto>> Handle(GetReceivedMessagesQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var receivedMessages = (from mtp in _context.MessageThreadParticipants.AsNoTracking()
                                join u in _context.Users on mtp.UserId equals u.Id
                                join m in _context.Messages on mtp.MessageThreadId equals m.MessageThreadId
                                where u.EmailAddress!.Equals(_currentUserService.UserId) && !m.SendingUserId.Equals(u.Id)
                                orderby m.Id descending
                                select new ReceivedMessageDto { SummaryContent = m.Content.Substring(0, 50), Created = m.Created, FromAddress = m.FromAddress, MessageThreadId = m.MessageThreadId });

        return await receivedMessages.PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}