using Microsoft.Extensions.Logging;

namespace Infrastructure.Common.Events;

internal class EventInfoId
{
    internal static readonly EventId _requestModemError = new EventId(100, "RequestModemError");
}
