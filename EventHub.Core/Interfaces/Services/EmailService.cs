using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Interfaces.Services
{
    namespace EventHub.Infrastructure.Services
    {
        public class EmailService : IEmailService
        {
            public async Task SendAsync(string to, string subject, string body)
            {
                // TODO: use SMTP or SendGrid 
                await Task.CompletedTask;
            }
        }
    }
}
