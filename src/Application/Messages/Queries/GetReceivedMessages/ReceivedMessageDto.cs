using AutoMapper;
using Messaging.Application.Common.Mappings;
using Messaging.Domain.Entities;

namespace Messaging.Application.Messages.Queries.GetReceivedMessages;

public class ReceivedMessageDto : IMapFrom<Message>
{
    public int MessageThreadId { get; set; }

    public string SummaryContent { get; set; } = null!;

    public string FromAddress { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public void Mapping(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        profile.CreateMap<Message, ReceivedMessageDto>()
            .ForMember(d => d.SummaryContent, opt => opt.MapFrom(s => s.Content.Take(50)));
    }
}
