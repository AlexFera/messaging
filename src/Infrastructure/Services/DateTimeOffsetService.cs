using Messaging.Application.Common.Interfaces;

namespace Messaging.Infrastructure.Services;

public class DateTimeOffsetService : IDateTimeOffset
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
