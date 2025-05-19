using Microsoft.EntityFrameworkCore;
using EventHub.Models;

namespace EventHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserTicket> UserTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserTicket>()
                .HasOne(ut => ut.User)
                .WithMany()
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTicket>()
                .HasOne(ut => ut.Event)
                .WithMany()
                .HasForeignKey(ut => ut.EventId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 