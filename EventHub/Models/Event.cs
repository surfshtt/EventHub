namespace EventHub.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Country { get; set; }
        public string Place { get; set; }
        public int NeedableAge { get; set; }
        public float[] Price { get; set; }

        public Event(int id, string name, string description, string date, string country, string place, int needableage, float[] price)
        {
            Id = id;
            Name = name;
            Description = description;
            Date = date;
            Country = country;
            Place = place;  
            NeedableAge = needableage;
            Price = price;
        }
    }
}
