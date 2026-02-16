using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Payments
{
    public class PaymentCreateDto
    {
        public int RegistrationId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }

}

