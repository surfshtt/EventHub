namespace EventHub.Models
{
    public class Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Country { get; set; }
        public string Place { get; set; }
        public int NeedableAge { get; set; }
        public float MinPrice { get; set; }

        public Event(string name, string description, string date, string country, string place, int needableage, float minPrice)
        {
            Name = name;
            Description = description;
            Date = date;
            Country = country;
            Place = place;  
            NeedableAge = needableage;
            MinPrice = minPrice;
        }
    }
}
