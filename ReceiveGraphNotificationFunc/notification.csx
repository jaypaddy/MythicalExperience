#r "Newtonsoft.Json"
public class Notification
{

    public string ChangeType { get; set; }
    public string ClientState { get; set; }
    public string Resource { get; set; }
    public DateTimeOffset SubscriptionExpirationDateTime { get; set; }
    public string SubscriptionId { get; set; }
    public ResourceData ResourceData { get; set; }
}

public class ResourceData
{
    public string Id { get; set; }
    public string ODataEtag { get; set; }
    public string ODataId { get; set; }
    public string ODataType { get; set; }
}