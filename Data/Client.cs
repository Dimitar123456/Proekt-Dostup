using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace ASP_Ticket_Center.Data
{
    public class Client:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }     
        public DateTime RegisterDate { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
