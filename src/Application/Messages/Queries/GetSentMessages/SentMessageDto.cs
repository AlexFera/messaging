using AutoMapper;
using Messaging.Application.Common.Mappings;
using Messaging.Domain.Entities;

namespace Messaging.Application.Messages.Queries.GetSentMessages;

public class SentMessageDto : IMapFrom<Message>
{
    public int MessageThreadId { get; set; }

    public string SummaryContent { get; set; } = null!;

    public string ToAddress { get; set; } = null!;

    public string FromAddress { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public void Mapping(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        profile.CreateMap<Message, SentMessageDto>()
            .ForMember(d => d.SummaryContent, opt => opt.MapFrom(s => s.Content.Substring(0, 50)));
    }
}
