using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

namespace MythicalExperienceConsole
{
    class TblStorageClient
    {
        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;

        public TblStorageClient()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("SubNotificationTblConnString"));
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public string GetNotifications()
        {
            string retStr = "";
            // Create the CloudTable object that represents the "notifications" table.
            CloudTable table = tableClient.GetTableReference("notifications");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<GraphNotifications> query = new TableQuery<GraphNotifications>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Notifications"));

            // Print the fields for each customer.
            int ni = 1;
            foreach (GraphNotifications entity in table.ExecuteQuery(query))
            {
                retStr += $"{ni}. SubExpiryDtTm:{entity.SubscriptionExpirationDateTime.ToString()} - {entity.SubscriptionId} {System.Environment.NewLine}";
                ni++;
            }

            return retStr;
        }
    }
}
