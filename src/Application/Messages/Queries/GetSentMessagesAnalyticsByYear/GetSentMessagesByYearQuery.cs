using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Messaging.Application.Common.Interfaces;

namespace Messaging.Application.Messages.Queries.GetSentMessagesAnalyticsByYear;

public class GetSentMessagesByYearQuery : IRequest<List<SentMessageByYearDto>>
{
}

public class GetSentMessagesInBatchQueryHandler : IRequestHandler<GetSentMessagesByYearQuery, List<SentMessageByYearDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetSentMessagesInBatchQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<SentMessageByYearDto>> Handle(GetSentMessagesByYearQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = _context.Messages
           .AsNoTracking()
           .Include(message => message.SendingUser)
           .Where(message => message.SendingUser.EmailAddress!.Equals(_currentUserService.UserId))
           .GroupBy(m => new { m.Created.Year }, (m, e) => new SentMessageByYearDto { Year = m.Year.ToString(CultureInfo.InvariantCulture), TotalCount = e.Count() });


        return await query.ToListAsync(cancellationToken);
    }
}