using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Messaging.Application.Common.Models;
using Messaging.Application.Messages.Commands.CreateMessage;
using Messaging.Application.Messages.Commands.CreateMessageReply;
using Messaging.Application.Messages.Queries.GetMessagesInThread;
using Messaging.Application.Messages.Queries.GetReceivedMessages;
using Messaging.Application.Messages.Queries.GetSentMessages;
using Messaging.Application.Messages.Queries.GetSentMessagesAnalyticsByYear;
using Messaging.Application.Messages.Queries.GetSentMessagesBatch;

namespace WebUI.Server.Controllers;

[Authorize(policy: "api-access")]
public class MessagesController : ApiControllerBase
{
    [HttpGet("Sent")]
    public async Task<ActionResult<PaginatedList<SentMessageDto>>> GetSentMessages([FromQuery] GetSentMessagesQuery query, CancellationToken cancellationToken)
    {
        return await Mediator.Send(query, cancellationToken);
    }

    [HttpGet("Received")]
    public async Task<ActionResult<PaginatedList<ReceivedMessageDto>>> GetReceivedMessages([FromQuery] GetReceivedMessagesQuery query, CancellationToken cancellationToken)
    {
        return await Mediator.Send(query, cancellationToken);
    }

    [HttpGet("Thread")]
    public async Task<ActionResult<List<MessageDto>>> GetMessagesInThread([FromQuery] GetMessagesInThreadQuery query, CancellationToken cancellationToken)
    {
        return await Mediator.Send(query, cancellationToken);
    }

    [HttpPost("Reply")]
    public async Task<ActionResult<int>> CreateMessageReplyAsync(CreateMessageReplyCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await Mediator.Send(command, cancellationToken);
    }

    [HttpGet("SentInBatch")]
    public async Task<ActionResult<SentMessagesInBatchResponse>> GetSentMessagesInBatch([FromQuery] GetSentMessagesInBatchQuery query, CancellationToken cancellationToken)
    {
        return await Mediator.Send(query, cancellationToken);
    }

    [HttpGet("SentByYear")]
    public async Task<ActionResult<List<SentMessageByYearDto>>> GetSentMessagesByYear([FromQuery] GetSentMessagesByYearQuery query, CancellationToken cancellationToken)
    {
        return await Mediator.Send(query, cancellationToken);
    }

    [HttpPost]
    [Authorize(policy: "api-access", Roles = "Administrator")]
    public async Task<ActionResult<int>> CreateAsync(CreateMessageCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await Mediator.Send(command, cancellationToken);
    }
}
