using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MythicalExperienceConsole
{
    public class Messages
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string odatacontext { get; set; }

        [JsonProperty(PropertyName = "@odata.nextlink")]
        public string NextPageLink { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<Message> Msg { get; set; }
    }

    public class From
    {
        public EmailAddress emailAddress { get; set; }
    }


    public class Recipient
    {
        public EmailAddress emailAddress { get; set; }
    }

    public class Message
    {
        public string odataeTag { get; set; }
        public string id { get; set; }
        public DateTime receivedDateTime { get; set; }
        public DateTime sentDateTime { get; set; }
        public string subject { get; set; }
        public string importance { get; set; }
        public From from { get; set; }
        public List<Recipient> toRecipients { get; set; }
        public List<Recipient> ccRecipients { get; set; }
    }
}




