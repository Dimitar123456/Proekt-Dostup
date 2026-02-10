using System.Reflection.Metadata;

namespace ASP_Ticket_Center.Data
{
    public class Ticket
    {
        public int Id { get; set; }
        public int CategoryId {  get; set; }
        public Category Categories{  get; set; }
        public int EventId {  get; set; }
        public Event Events { get;set; }
        public string QRCode {  get; set; }
        public DateTime RegisterDate {  get; set; }
        public string Seat { get; set; }
        public string Line { get; set; }


        
        

    }
}
