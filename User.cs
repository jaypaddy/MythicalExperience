using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MythicalExperienceConsole
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public List<string> businessPhones { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string jobTitle { get; set; }
        public string mail { get; set; }
        public string mobilePhone { get; set; }
        public string officeLocation { get; set; }
        public string preferredLanguage { get; set; }
        public string surname { get; set; }
        public string userPrincipalName { get; set; }
    }

    public class UserList
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string odatacontext { get; set; }

        [JsonProperty(PropertyName = "@odata.nextlink")]
        public string NextPageLink { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<User> user { get; set; }

        public UserList()
        {
            user = new List<User>();     
        }
    }
}



