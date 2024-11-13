using EventManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace EventManagementSystem.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Event>()
                .HasOne(e => e.Location)
                .WithMany(l => l.Events)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Event>()
        .HasMany(e => e.Speakers)
        .WithMany(s => s.Events)
        .UsingEntity<Dictionary<string, object>>(
            "EventSpeaker",
            j => j
                .HasOne<Speaker>()
                .WithMany()
                .HasForeignKey("SpeakerId")
                .OnDelete(DeleteBehavior.Cascade),
            j => j
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey("EventId")
                .OnDelete(DeleteBehavior.Cascade)
        );
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Event>()
        .Property(t => t.Price)
        .HasColumnType("decimal(10, 2)");

        }
    }
}
