using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MythicalExperienceConsole
{
    class MeetingTimeObject
    {
        public List<Attendee> attendees { get; set; }
        public LocationConstraint locationConstraint { get; set; }
        public TimeConstraint timeConstraint { get; set; }
        public string meetingDuration { get; set; }
        public string returnSuggestionReasons { get; set; }
        public string minimumAttendeePercentage { get; set; }

        public MeetingTimeObject()
        {
            attendees = new List<Attendee>();
        }
    }

    public class EmailAddress
    {
        public string name { get; set; }
        public string address { get; set; }

        public EmailAddress(string _name, string _address)
        {
            this.name = _name;
            this.address = _address;
        }
    }

    public class Attendee
    {
        public string type { get; set; }
        public EmailAddress emailAddress { get; set; }
        public Attendee (EmailAddress _emailAddress,string _type)
        {
            emailAddress = _emailAddress;
            type = _type;
        }
    }

    public class Location
    {
        public string resolveAvailability { get; set; }
        public string displayName { get; set; }

    }

    public class LocationConstraint
    {
        public string isRequired { get; set; }
        public string suggestLocation { get; set; }
        public List<Location> locations { get; set; }
    }

    public class Start
    {
        public string dateTime { get; set; }
        public string timeZone { get; set; }

        public Start(string _dateTime, string _timeZone)
        {
            dateTime = _dateTime;
            timeZone = _timeZone;
        }
    }

    public class End
    {
        public string dateTime { get; set; }
        public string timeZone { get; set; }

        public End(string _dateTime, string _timeZone)
        {
            dateTime = _dateTime;
            timeZone = _timeZone;
        }
    }

    public class Timeslot
    {
        public Start start { get; set; }
        public End end { get; set; }

        public Timeslot(string _start, string _end, string timezone)
        {
            start = new Start(_start, timezone);
            end = new End(_end, timezone);
        }
    }

    public class TimeConstraint
    {
        public string activityDomain { get; set; }
        public List<Timeslot> timeslots { get; set; }
    }

}
