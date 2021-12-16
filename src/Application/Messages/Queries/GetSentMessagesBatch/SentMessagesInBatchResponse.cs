namespace Messaging.Application.Messages.Queries.GetSentMessagesBatch;

public class SentMessagesInBatchResponse
{
    public int ModifiedCount { get; set; }

    public List<SentMessageBatchDto> SentMessages { get; set; } = new List<SentMessageBatchDto>();
}
