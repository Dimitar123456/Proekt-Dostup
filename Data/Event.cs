namespace ASP_Ticket_Center.Data
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public int CategoryId { get; set;}
        public Category Categories { get; set;}
        public string Location {  get; set;}
        public string Description { get; set;}
        public string ImageURL { get; set;}
        public string Organizer { get; set;}
        public int Capacity {  get; set;}
        public DateTime Date { get; set;}
        public decimal MaxPrice {  get; set;}
        public decimal MinPrice { get; set;}
        public bool Status {  get; set;}
        public DateTime Last_Update {  get; set;}
        public ICollection<Ticket> Tickets { get; set;}
       
    }
}
