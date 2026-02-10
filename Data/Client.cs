using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace ASP_Ticket_Center.Data
{
    public class Client:IdentityUser
    {
        public string First_name {  get; set; }
        public string Last_name { get; set; }     
        public DateTime RegisterDate {  get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
