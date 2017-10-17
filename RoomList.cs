using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MythicalExperienceConsole
{ 
    public class Value
    {
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Address")]
        public string Address { get; set; }
    }

    public class RoomList
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string odatacontext { get; set; }

        public List<Value> value { get; set; }
    }
}
