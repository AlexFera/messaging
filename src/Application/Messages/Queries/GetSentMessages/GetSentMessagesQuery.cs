using System.Linq.Dynamic.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Messaging.Application.Common.Interfaces;
using Messaging.Application.Common.Mappings;
using Messaging.Application.Common.Models;

namespace Messaging.Application.Messages.Queries.GetSentMessages;

public class GetSentMessagesQuery : IRequest<PaginatedList<SentMessageDto>>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Filter { get; set; }

    public string? OrderBy { get; set; }
}

public class GetSentMessagesQueryHandler : IRequestHandler<GetSentMessagesQuery, PaginatedList<SentMessageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetSentMessagesQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<SentMessageDto>> Handle(GetSentMessagesQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = _context.Messages
            .AsNoTracking()
            .Include(message => message.SendingUser)
            .Where(message => message.SendingUser.EmailAddress!.Equals(_currentUserService.UserId));

        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(request.Filter);
        }

        if (!string.IsNullOrEmpty(request.OrderBy))
        {
            query = query.OrderBy(request.OrderBy);
        }
        else
        {
            query = query.OrderByDescending(x => x.Id);
        }

        return await query
            .ProjectTo<SentMessageDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}