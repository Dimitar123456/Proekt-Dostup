namespace ASP_Ticket_Center.Data
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ClientId {  get; set; }
        public int TicketId {  get; set; }
        public int Quantity {  get; set; }
        public DateTime RegisterDate { get; set; }

        public Client Clients { get; set; }
        public Ticket Tickets { get; set; }
    }
}
