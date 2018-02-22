#r "Newtonsoft.Json"

public class Subscription
{
    public string changeType { get; set; }
    public string clientState { get; set; }
    public string notificationUrl { get; set; }
    public string resource { get; set; }
    public DateTimeOffset expirationDateTime { get; set; }
    public string Id { get; set; }
}

// The data that displays in the Subscription view.
public class SubscriptionViewModel
{
    public Subscription Subscription { get; set; }
}