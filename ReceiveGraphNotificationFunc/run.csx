#r "Microsoft.WindowsAzure.Storage"
#load "Notification.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;

public async static Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<SubQueueMessage> inTable, ICollector<NotificationTable> outputTable, TraceWriter log)
{
    // parse query parameter
    string valToken = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "validationToken", true) == 0)
        .Value;
    log.Info($"Validation Token: {valToken}");
    // Validate the new subscription by sending the token back to Microsoft Graph.
    // This response is required for each subscription.
    if (valToken != null)
    {
        return req.CreateResponse(HttpStatusCode.OK, $"{valToken}");
    }



    var notifications = new Dictionary<string, Notification>();
    //using (var inputStream = new System.IO.StreamReader(Request.InputStream))
    dynamic data = await req.Content.ReadAsAsync<object>();
    log.Info($"Body:{data}");
    Newtonsoft.Json.Linq.JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(Convert.ToString(data));
    if (jsonObject != null)
    {
        // Notifications are sent in a 'value' array. The array might contain multiple notifications for events that are
        // registered for the same notification endpoint, and that occur within a short timespan.
        Newtonsoft.Json.Linq.JArray value = Newtonsoft.Json.Linq.JArray.Parse(jsonObject["value"].ToString());
        foreach (var notification in value)
        {
            Notification current = Newtonsoft.Json.JsonConvert.DeserializeObject<Notification>(notification.ToString());
            notifications[current.Resource] = current;
            log.Info($"SubId:{current.SubscriptionId}");
            log.Info($"SubExpiration:{current.SubscriptionExpirationDateTime}");
            log.Info($"SubExpiration:{current.ResourceData.ODataId}");
            //Add to Notifications Table
            outputTable.Add(new NotificationTable()
            {
                PartitionKey = "Notifications",
                RowKey = Guid.NewGuid().ToString(),
                ChangeType = current.SubscriptionId,
                ClientState = current.ClientState,
                Resource = current.Resource,
                SubscriptionExpirationDateTime = current.SubscriptionExpirationDateTime,
                SubscriptionId = current.SubscriptionId,
                Id = current.ResourceData.Id,
                ODataEtag = current.ResourceData.ODataEtag,
                ODataId = current.ResourceData.ODataId,
                ODataType = current.ResourceData.ODataType,
            });
        }
    }


    //var query = from subscription in inTable select subscription;
    //foreach (SubQueueMessage subscription in query)
    //{
    //    log.Info($"SubId:{subscription.Id}");
    //}
    return req.CreateResponse(HttpStatusCode.Accepted, "");
}

public class Person : TableEntity
{
    public string Name { get; set; }
}

public class SubQueueMessage : TableEntity
{
    public string error { get; set; }
    public string Token { get; set; }
    public string Id { get; set; }
}

public class NotificationTable : TableEntity
{
    public string ChangeType { get; set; }
    public string ClientState { get; set; }
    public string Resource { get; set; }
    public DateTimeOffset SubscriptionExpirationDateTime { get; set; }
    public string SubscriptionId { get; set; }
    public string Id { get; set; }
    public string ODataEtag { get; set; }
    public string ODataId { get; set; }
    public string ODataType { get; set; }
}









