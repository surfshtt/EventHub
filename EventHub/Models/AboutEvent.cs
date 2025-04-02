namespace EventHub.Models
{
    public class AboutEvent
    {
        public Event eventToShow { set; get; }
        public Event[] recEvents { set; get; }


        public AboutEvent(Event eventToShow, Event[] recEvents)
        {
            this.eventToShow = eventToShow;
            this.recEvents = recEvents;
        }
    }
}
