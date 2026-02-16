using Azure;
using EventHub.Core.Entities;
using iText.Kernel.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace EventHub.Infrastructure.Data

{
    
    public class EventHubDbContext : DbContext
    {
        public EventHubDbContext(DbContextOptions<EventHubDbContext> options)
       : base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<EEvent> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entity in entries)
            {
                if (entity.Entity is IHasUpdated trackable)
                {
                    trackable.Updated = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Email Unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Category - Event (1 - M)
            modelBuilder.Entity<EEvent>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Event (1 - M) as Organizer
            modelBuilder.Entity<EEvent>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event - Registration (1 - M)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Registration (1 - M) as Attendee
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Attendee)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.AttendeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Registration - Payment (1 - 1)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Registration)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RegistrationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Registration - Ticket (1 - 1)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Registration)
                .WithOne(r => r.Ticket)
                .HasForeignKey<Ticket>(t => t.RegistrationId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Ticket (1 - M)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Attendee)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event - Ticket (1 - M)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Testimonial (1 - M) as Attendee
            modelBuilder.Entity<Testimonial>()
                .HasOne(t => t.Attendee)
                .WithMany(u => u.Testimonials)
                .HasForeignKey(t => t.AttendeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event - Testimonial (1 - M)
            modelBuilder.Entity<Testimonial>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Testimonials)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - AuditLog (1 - M)
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}