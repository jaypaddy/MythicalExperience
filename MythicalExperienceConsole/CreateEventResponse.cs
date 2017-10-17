using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MythicalExperienceConsole
{
    public class ResponseStatus
    {
        public string response { get; set; }
        public DateTime time { get; set; }
    }

    public class Status
    {
        public string response { get; set; }
        public DateTime time { get; set; }
    }

    public class Organizer
    {
        public EmailAddress emailAddress { get; set; }
    }

    public class CreateEventResponse
    {
        public string odatacontext { get; set; }
        public string odataetag { get; set; }
        public string id { get; set; }
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string changeKey { get; set; }
        public List<object> categories { get; set; }
        public string originalStartTimeZone { get; set; }
        public string originalEndTimeZone { get; set; }
        public string iCalUId { get; set; }
        public int reminderMinutesBeforeStart { get; set; }
        public bool isReminderOn { get; set; }
        public bool hasAttachments { get; set; }
        public string subject { get; set; }
        public string bodyPreview { get; set; }
        public string importance { get; set; }
        public string sensitivity { get; set; }
        public bool isAllDay { get; set; }
        public bool isCancelled { get; set; }
        public bool isOrganizer { get; set; }
        public bool responseRequested { get; set; }
        public object seriesMasterId { get; set; }
        public string showAs { get; set; }
        public string type { get; set; }
        public string webLink { get; set; }
        public object onlineMeetingUrl { get; set; }
        public ResponseStatus responseStatus { get; set; }
        public Body body { get; set; }
        public Start start { get; set; }
        public End end { get; set; }
        public Location location { get; set; }
        public object recurrence { get; set; }
        public List<Attendee> attendees { get; set; }
        public Organizer organizer { get; set; }
    }
}
