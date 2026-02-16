using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int RegistrationId { get; set; }
        public Registration Registration { get; set; }

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // CreditCard, PayPal...
        public string Status { get; set; } // Pending, Paid, Failed
        public DateTime? PaidAt { get; set; }
    }
}

