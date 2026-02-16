using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Payments
{
    public class PaymentReadDto
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
