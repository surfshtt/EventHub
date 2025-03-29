namespace EventHub.Models
{
    public class Event
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Event(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
