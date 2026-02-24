using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ASP_Ticket_Center.Data
{
    public class Ticket
    {
        public int Id { get; set; }
        public int CategoryId {  get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Categories{  get; set; }
        public int EventId {  get; set; }
        [ForeignKey(nameof(EventId))]
        public Event Events { get;set; }
        public string QRCode {  get; set; }
        public DateTime RegisterDate {  get; set; }
        public string Seat { get; set; }
        public string Line { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
        
        

    }
}
