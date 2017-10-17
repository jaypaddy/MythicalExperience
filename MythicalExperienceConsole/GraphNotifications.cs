using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MythicalExperienceConsole
{
    class GraphNotifications :TableEntity
    {
        public string ChangeType  { get; set; }
        public string ClientState { get; set; }

        public string Resource { get; set; }

        public DateTimeOffset SubscriptionExpirationDateTime { get; set; }

        public string SubscriptionId { get; set; }

        public string Id { get; set; }


    }
}
