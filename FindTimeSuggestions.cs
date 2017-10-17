using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MythicalExperienceConsole
{

    public class FindTimeSuggestion
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string odatacontext { get; set; }

        [JsonProperty(PropertyName = "emptySuggestionsReason")]
        public string emptySuggestionsReason { get; set; }

        [JsonProperty(PropertyName = "meetingTimeSuggestions")]
        public List<MeetingTimeSuggestions> meetingTimeSuggestions { get; set; }
    }

    public class MeetingTimeSuggestions : IEquatable<MeetingTimeSuggestions>, IComparable<MeetingTimeSuggestions>

    {
        [JsonProperty(PropertyName = "confidence")]
        public double confidence { get; set; }

        [JsonProperty(PropertyName = "organizerAvailability")]
        public string organizerAvailability { get; set; }

        [JsonProperty(PropertyName = "suggestionReason")]
        public string suggestionReason { get; set; }

        [JsonProperty(PropertyName = "meetingTimeSlot")]
        public MeetingTimeSlot meetingTimeSlot { get; set; }

        [JsonProperty(PropertyName = "attendeeAvailability")]
        public List<AttendeeAvailability> attendeeAvailability { get; set; }

        [JsonProperty(PropertyName = "locations")]
        public List<location> locations { get; set; }

        int IComparable<MeetingTimeSuggestions>.CompareTo(MeetingTimeSuggestions compareMTS)
        {
            if (compareMTS == null)
                return 1;

            else
                return this.meetingTimeSlot.start.dateTime.CompareTo(compareMTS.meetingTimeSlot.start.dateTime);
        }

        public override int GetHashCode()
        {
            return meetingTimeSlot.start.dateTime.GetHashCode();
        }
        public bool Equals(MeetingTimeSuggestions other)
        {
            if (other == null) return false;
            return (this.meetingTimeSlot.start.dateTime.Equals(other.meetingTimeSlot.start.dateTime));
        }

        bool IEquatable<MeetingTimeSuggestions>.Equals(MeetingTimeSuggestions other)
        {
            if (other == null) return false;
            if (other == null) return false;
            else return Equals(other);
        }
    }

    public class MeetingTimeSlot
    {
        [JsonProperty(PropertyName = "start")]
        public Start start { get; set; }

        [JsonProperty(PropertyName = "end")]
        public End end { get; set; }
    }

    public class AttendeeAvailability
    {
        [JsonProperty(PropertyName = "availability")]
        public string availability { get; set; }

        [JsonProperty(PropertyName = "attendee")]
        public Attendee attendee { get; set; }
    }

    public class Address
    {
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string countryOrRegion { get; set; }
        public string postalCode { get; set; }
    }

    public class location
    {
        public string displayName { get; set; }
        public string locationEmailAddress { get; set; }
        public Address address { get; set; }
    }

}