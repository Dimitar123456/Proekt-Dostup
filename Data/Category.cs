namespace ASP_Ticket_Center.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
