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
    }
}
