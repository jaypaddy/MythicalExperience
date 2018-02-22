#r "Newtonsoft.Json"
#load "Subscription.csx"

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;


public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    String accessToken = data?.token;
    log.Info($"Access Token: {accessToken}");

    String accessExpiryDtTm = data?.accestokenexpirydttm;
    log.Info($"ExpiryDtTm: {accessExpiryDtTm}");

    string subscriptionsEndpoint = "https://graph.microsoft.com/v1.0/subscriptions/";
    // Build the request.
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, subscriptionsEndpoint);
    Subscription subscription = new Subscription
    {
        resource = "me/events",
        changeType = "created,updated",
        notificationUrl = "https://mythicalexperience.azurewebsites.net/api/ReceiveGraphNotification?code=h9sJ6j69Iw5geuIWhGxKSyJiXYyBrR9WkMRc0ZyEJv5tAPbIXt9ZQQ==",
        clientState = Guid.NewGuid().ToString(),
        expirationDateTime = DateTime.UtcNow + new TimeSpan(0, 0, 4230, 0) // current maximum timespan for messages
    };

    string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(subscription,
     new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
    request.Content = new StringContent(contentString, System.Text.Encoding.UTF8, "application/json");

    log.Info($"Payload:{contentString}");
    // Send the `POST subscriptions` request and parse the response.
    HttpResponseMessage response = await client.SendAsync(request);
    if (response.IsSuccessStatusCode)
    {
        string stringResult = await response.Content.ReadAsStringAsync();
        log.Info($"stringResult: {stringResult}");
        SubscriptionViewModel viewModel = new SubscriptionViewModel
        {
            Subscription = Newtonsoft.Json.JsonConvert.DeserializeObject<Subscription>(stringResult)
        };
        log.Info($"Subscription ID: {viewModel.Subscription.Id}");

        SubQueueMessage sQmsg = new SubQueueMessage("PASS", accessToken, accessExpiryDtTm, viewModel.Subscription.Id);

        return req.CreateResponse(await sQmsg.ToSubscriptionStore(log), sQmsg);
    }
    else
    {
        string errMsg = await response.Content.ReadAsStringAsync();
        log.Info($"Error : {errMsg}");
        SubQueueMessage sQmsg = new SubQueueMessage("ERROR", accessToken, accessExpiryDtTm, errMsg);
        return req.CreateResponse(await sQmsg.ToSubscriptionStore(log), sQmsg);
    }
}

public class SubQueueMessage
{
    public string error { get; set; }
    public string Token { get; set; }
    public string Id { get; set; }
    public string ExpiryDtTm { get; set; }


    public SubQueueMessage(string err, string token, string expirydttm, string id)
    {
        error = err;
        Token = token;
        ExpiryDtTm = expirydttm;
        Id = id;
    }

    public async Task<HttpStatusCode> ToSubscriptionStore(TraceWriter log)
    {
        string SubscriptionStoreEndPoint = "https://mythicalexperience.azurewebsites.net/api/SubscriptionStoreTable?code=YVjIoAx/prbQM1AWUZbxCXCaKgqfENMOQlEBFKU2XqIbuyVCMZBnyw==";
        // Build the request.
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, SubscriptionStoreEndPoint);


        string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(this,
            new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
        request.Content = new StringContent(contentString, System.Text.Encoding.UTF8, "application/json");

        // Send the `POST subscriptions` request and parse the response.
        HttpResponseMessage response = await client.SendAsync(request);

        string Msg = await response.Content.ReadAsStringAsync();
        log.Info($"StoreMsg : {Msg}");
        return (response.StatusCode);

    }
}

