using AutoMapper;
using Messaging.Application.Common.Mappings;
using Messaging.Domain.Entities;

namespace Messaging.Application.Messages.Queries.GetSentMessagesBatch;

public class SentMessageBatchDto : IMapFrom<Message>
{
    public int Id { get; set; }

    public int MessageThreadId { get; set; }

    public string SummaryContent { get; set; } = null!;

    public string ToAddress { get; set; } = null!;

    public string FromAddress { get; set; } = null!;

    public string Created { get; set; } = null!;

    public void Mapping(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        profile.CreateMap<Message, SentMessageBatchDto>()
            .ForMember(d => d.SummaryContent, opt => opt.MapFrom(s => s.Content.Substring(0, 50)))
            .ForMember(d => d.Created, opt => opt.MapFrom(s => s.Created.ToLocalTime()));
    }
}

