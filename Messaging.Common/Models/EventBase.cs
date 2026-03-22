using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Common.Models
{
    public abstract class EventBase
    {
        public Guid EventId { get; private set; } = Guid.NewGuid();
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
        public string? CorrelationId { get; set; }
    }
}
