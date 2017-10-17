using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MythicalExperienceConsole
{
    public class RelevantPeople
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string datacontext { get; set; }

        [JsonProperty(PropertyName = "@odata.nextlink")]
        public string datanextLink { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<Person> persons { get; set; }
    }


    public class ScoredEmailAddress
    {
        public string address { get; set; }
        public string relevanceScore { get; set; }
    }

    public class Person
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public List<ScoredEmailAddress> scoredEmailAddresses { get; set; }
    }


}
