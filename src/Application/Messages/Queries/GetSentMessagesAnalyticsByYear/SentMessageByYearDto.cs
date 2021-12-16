namespace Messaging.Application.Messages.Queries.GetSentMessagesAnalyticsByYear;

public class SentMessageByYearDto
{
    public string Year { get; set; } = null!;

    public int TotalCount { get; set; }
}
