using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Messaging.Application.Common.Interfaces;

namespace Messaging.Application.Messages.Queries.GetMessagesInThread;

public class GetMessagesInThreadQuery : IRequest<List<MessageDto>>
{
    public int MessageThreadId { get; set; }
}

public class GetMessagesInThreadQueryHandler : IRequestHandler<GetMessagesInThreadQuery, List<MessageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMessagesInThreadQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesInThreadQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _context.Messages
           .AsNoTracking()
           .Include(message => message.SendingUser)
           .Where(message => message.MessageThreadId == request.MessageThreadId)
           .OrderBy(x => x.Id)
           .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
           .ToListAsync(cancellationToken);
    }
}