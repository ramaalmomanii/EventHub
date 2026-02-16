using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs
{
    public class AuditLogReadDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

