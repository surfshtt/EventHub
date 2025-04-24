namespace EventHub.Models
{
    public class AboutEvent
    {
        public Event? Event { get; set; }
        public List<Event> RelatedEvents { get; set; } = new List<Event>();
    }
}
