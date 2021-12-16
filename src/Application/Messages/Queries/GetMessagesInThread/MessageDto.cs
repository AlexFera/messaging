using AutoMapper;
using Messaging.Application.Common.Mappings;
using Messaging.Domain.Entities;

namespace Messaging.Application.Messages.Queries.GetMessagesInThread;

public class MessageDto : IMapFrom<Message>
{
    public string Content { get; set; } = null!;

    public string SendingUserEmail { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public void Mapping(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        profile.CreateMap<Message, MessageDto>()
            .ForMember(d => d.SendingUserEmail, opt => opt.MapFrom(s => s.SendingUser.EmailAddress));
    }
}
