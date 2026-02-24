using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_Ticket_Center.Data
{
    public class ApplicationDbContext : IdentityDbContext<Client>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
       public DbSet<Event> Events { get; set; }
       public DbSet<Category> Categories { get; set; }
       public DbSet<Ticket> Tickets { get; set; }
       public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ticket → Category (no cascade)
            builder.Entity<Ticket>()
                .HasOne(t => t.Categories)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // ❌ no cascade

            // Ticket → Event (cascade)
            builder.Entity<Ticket>()
                .HasOne(t => t.Events)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Event → Category (cascade optional)
            builder.Entity<Event>()
                .HasOne(e => e.Categories)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reservation → Ticket
            builder.Entity<Reservation>()
                .HasOne(r => r.Tickets)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reservation → Client
            builder.Entity<Reservation>()
                .HasOne(r => r.Clients)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
