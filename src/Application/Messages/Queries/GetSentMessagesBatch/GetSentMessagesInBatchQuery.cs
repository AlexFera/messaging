using System.Linq.Dynamic.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Messaging.Application.Common.Interfaces;

namespace Messaging.Application.Messages.Queries.GetSentMessagesBatch;

public class GetSentMessagesInBatchQuery : IRequest<SentMessagesInBatchResponse>
{
    public int MaxCount { get; set; } = 1000;

    public long ModifiedSinceTicks { get; set; }
}

public class GetSentMessagesInBatchQueryHandler : IRequestHandler<GetSentMessagesInBatchQuery, SentMessagesInBatchResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetSentMessagesInBatchQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<SentMessagesInBatchResponse> Handle(GetSentMessagesInBatchQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var modifiedSentMessages = _context.Messages
            .AsNoTracking()
            .Include(message => message.SendingUser)
            .Where(message => message.SendingUser.EmailAddress!.Equals(_currentUserService.UserId))
            .Where(message => message.Id > request.ModifiedSinceTicks);

        var sentMessagesInBatchResponse = new SentMessagesInBatchResponse
        {
            ModifiedCount = await modifiedSentMessages.CountAsync(cancellationToken),
            SentMessages = await modifiedSentMessages.Take(request.MaxCount).ProjectTo<SentMessageBatchDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken)
        };

        return sentMessagesInBatchResponse;
    }
}