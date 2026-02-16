using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; } // Unique
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin, Organizer, Attendee
        public string Status { get; set; } // Active, Inactive
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public DateTime? UpdatedAt { get; set; }


        // Add these properties for password reset functionality
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        // Add these properties for email verification functionality
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }

        // Add these properties for refresh token functionality
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }

        // Relations
        public ICollection<EEvent> Events { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<EEvent> OrganizedEvents { get; set; }
        public ICollection<Registration> Registrations { get; set; }
        public ICollection<Testimonial> Testimonials { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }
    }
}

