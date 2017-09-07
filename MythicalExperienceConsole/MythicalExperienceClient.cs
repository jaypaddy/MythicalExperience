using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace MythicalExperienceConsole
{
    class MythicalExperienceClient
    {
        private PublicClientApplication PublicClientApp;
        private static string _mythicalEndPoint = "https://mythicalexperience.azurewebsites.net/api/GetGraph?code=eo6tnI1THhBAnmaadMkh8ngEdoqcuCBpzewOgjHrKCuY1Y3TruwpPA==";
        private static string _mythicalEventsCreateSubEndPoint = "https://mythicalexperience.azurewebsites.net/api/CreateGraphEventSubscription?code=mBqcWni6lg9Ukv3uqNZWm0BWUQGIXeMCtqZK1IwJtZeyY9RaQbgfbg==";
        private static string _mythicalFindMtgTimesEndPoint = "https://graph.microsoft.com/v1.0/me/findMeetingTimes";
        private static string _mythicalGetUserEndPoint = "https://graph.microsoft.com/v1.0/users";
        private static string _mythicalExperienceCreateEventEndPoint = "https://graph.microsoft.com/v1.0/me/events";

        private static string _outlookAccessToken="";
        private AuthenticationResult _authResult = null;
        private AuthenticationResult _outlookAuthResult = null;
        private static string ClientId = "abf2827a-f496-450f-810f-e5c236360d62";
        //private static string ClientId = "3f6adfd9-8381-498c-a5f8-440d5f974959";


        //https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-setup-aad-custom
        //Create an Azure AD app
        //      ApplicationID on JAYPADDY Azure AD Org : d564c77c-0a5e-4c68-b56b-452e5a8ccf3c
        //      JayPaddyAADKey Expires: ‎12‎/‎31‎/‎2299 : oh7ljBictuiajRxaK2IvMpBJ/iW1k39mHF0jYYBhUZs=
        //Add the Azure AD key to Azure AD B2C
        //      B2C_1A_MythicalExperience_JAYPADDY_AAD
        //Add a claims provider in your base policy
        //      

        private static string[] _Graphscopes = new string[] { "User.Read", "User.ReadBasic.All",
                                                              "Calendars.ReadWrite", "Calendars.Read.Shared", "Calendars.ReadWrite.Shared"
                                                            };
        private static string[] _outlookscopes = new string[] { "https://outlook.office.com/user.readbasic.all" };
        private Subscription _subscription = null;
        private RoomList _roomList = null;
        private RoomList _crooms = null;


        public bool bGraphSignedIn,bOutlookSignedIn;
        private string lastMsg;

        private FindTimeSuggestion lastFTSObj=null;


        public string GetLastMsg()
        {
            return lastMsg;
        }
        public MythicalExperienceClient()
        {

            PublicClientApp = new PublicClientApplication(ClientId, "https://login.microsoftonline.com/common", TokenCacheHelper.GetUserCache());


        }

        public async Task<bool> SignInToGraph()
        {
            try
            {
                _authResult = await PublicClientApp.AcquireTokenSilentAsync(_Graphscopes, PublicClientApp.Users.FirstOrDefault());


            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                lastMsg = $"MsalUiRequiredException: {ex.Message}";
                try
                {
                    _authResult = await PublicClientApp.AcquireTokenAsync(_Graphscopes);
                }
                catch (MsalException msalex)
                {
                    lastMsg = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                    bGraphSignedIn = false;
                }
            }
            catch (Exception ex)
            {
                lastMsg = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
                bGraphSignedIn = false;
            }

            if (_authResult != null)
            {
                DisplayBasicTokenInfo(_authResult);
                bGraphSignedIn = true;
            }
            else
                bGraphSignedIn = false;

            return bGraphSignedIn;
        }

        public async Task<bool>SignInToOutlook()
        {
            try
            {
                _outlookAuthResult = await PublicClientApp.AcquireTokenSilentAsync(_outlookscopes, PublicClientApp.Users.FirstOrDefault());
                bOutlookSignedIn = true;
            }
            catch (MsalUiRequiredException ex)
            {
                lastMsg = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
                try
                {
                    _outlookAuthResult = await PublicClientApp.AcquireTokenAsync(_outlookscopes);
                    bOutlookSignedIn = true;
                }
                catch (MsalException msalex)
                {
                    lastMsg = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                    bOutlookSignedIn = false;
                }
            }
            if (_outlookAuthResult != null)
            {
                _outlookAccessToken = _outlookAuthResult.AccessToken;
                DisplayBasicTokenInfo(_outlookAuthResult);
                bOutlookSignedIn = true;
            }
            else
                bOutlookSignedIn = false;
            return bOutlookSignedIn;
        }

        public bool SignOut()
        {
            if (PublicClientApp.Users.Any())
            {
                try
                {
                    string username = PublicClientApp.Users.FirstOrDefault().Name;
                    PublicClientApp.Remove(PublicClientApp.Users.FirstOrDefault());
                    lastMsg = $"Signed out user: {username}";
                    return true;
                }
                catch (MsalException ex)
                {
                    lastMsg = $"Error signing-out user: {ex.Message}";
                    return false;
                }
            }
            else
            {
                lastMsg = $"No signedin user";
                return false;
            }

        }

        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private void DisplayBasicTokenInfo(AuthenticationResult authResult)
        {
            
            if (authResult != null)
            {

                lastMsg = $"Welcome " + $"Name: {authResult.User.Name} {System.Environment.NewLine}";
                lastMsg += $"Token Expires: {authResult.ExpiresOn.ToLocalTime()} {System.Environment.NewLine}";
                lastMsg += $"Scope:{System.Environment.NewLine}";
                foreach (string scope in authResult.Scopes)
                {
                    lastMsg += $"     {scope} {System.Environment.NewLine}";
                }          
            }
        }

        public async Task<string> CreateSub()
        {
            //Collect the AuthToken and send it to Graph
            //Call the Azure Function 
            //Create a JSON with the Token
            //{ "token": "tokenvalue" , "expirydttm" : "expirydttime"}

            String payloadString = @"{'token':'" + _authResult.AccessToken + @"'" + @",";
            payloadString += @"'accestokenexpirydttm':'" + _authResult.ExpiresOn.ToUniversalTime().ToString() + @"'}";
            var payload = new StringContent(payloadString, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(_mythicalEventsCreateSubEndPoint, payload);
                var content = await response.Content.ReadAsStringAsync();
                Subscription subObj = JsonConvert.DeserializeObject<Subscription>(content);
                lastMsg = $"Status:{subObj.error}% - ID:{subObj.Id} - ExpiresAt:{subObj.ExpiryDtTm}{System.Environment.NewLine}";
                return content;
            }
            catch (Exception ex)
            {
                lastMsg = ex.ToString();
                return ex.ToString();
            }
        }

        public async Task<string> GetCalendarEvents()
        {
            //Collect the AuthToken and send it to Graph
            //Call the Azure Function 
            //Create a JSON with the Token
            //{ "name": "Azure" }

            var payload = new StringContent(@"{'token':'" + _authResult.AccessToken + @"'}", Encoding.UTF8, "application/json");

            var httpClient = new System.Net.Http.HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(_mythicalEndPoint, payload);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> GetRoomLists()
        {
            //Collect the AuthToken and send it to Graph
            //Call the Azure Function 
            //Create a JSON with the Token
            //{ "name": "Azure" }             
            try
            {
                _outlookAuthResult = await PublicClientApp.AcquireTokenSilentAsync(_outlookscopes, PublicClientApp.Users.FirstOrDefault());
            }
            catch (MsalUiRequiredException ex)
            {
                Console.WriteLine($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
                try
                {
                    _outlookAuthResult = await PublicClientApp.AcquireTokenAsync(_outlookscopes);
                }
                catch (MsalException msalex)
                {
                    Console.WriteLine($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }
            }

            try
            {
                string OutlookEndRoomListPoint = "https://outlook.office.com/api/beta/me/findroomlists";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _outlookAuthResult.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, OutlookEndRoomListPoint);
                // Send the `POST subscriptions` request and parse the response.
                HttpResponseMessage response = await client.SendAsync(request);
                string retMsg = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    RoomList rListObj = JsonConvert.DeserializeObject<RoomList>(retMsg);
                    if (rListObj != null)
                    {
                        lastMsg = $"Locations:{System.Environment.NewLine}";
                        foreach (var rlo in rListObj.value)
                        {
                            lastMsg += $"{rlo.Name} - {rlo.Address} {System.Environment.NewLine}";
                        }
                        _roomList = rListObj;
                    }

                }
                return retMsg;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> ListRooms()
        {
            //Collect the AuthToken and send it to Graph
            //Call the Azure Function 
            //Create a JSON with the Token
            //{ "name": "Azure" }    
            try
            {
                _outlookAuthResult = await PublicClientApp.AcquireTokenSilentAsync(_outlookscopes, PublicClientApp.Users.FirstOrDefault());
            }
            catch (MsalUiRequiredException ex)
            {
                Console.WriteLine($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
                try
                {
                    _outlookAuthResult = await PublicClientApp.AcquireTokenAsync(_outlookscopes);
                }
                catch (MsalException msalex)
                {
                    Console.WriteLine($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }
            }
            try
            {
                string OutlookEndRoomListPoint = "https://outlook.office.com/api/beta/me/findrooms";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _outlookAuthResult.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, OutlookEndRoomListPoint);
                // Send the `POST subscriptions` request and parse the response.
                HttpResponseMessage response = await client.SendAsync(request);
                string retMsg = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    RoomList rListObj = JsonConvert.DeserializeObject<RoomList>(retMsg);
                    if (rListObj != null)
                    {
                        lastMsg = $"Locations:{System.Environment.NewLine}";
                        foreach (var rlo in rListObj.value)
                        {
                            string gretMsg = await GraphClientGET($"{_mythicalGetUserEndPoint}/{rlo.Address}");
                            lastMsg += $"{rlo.Name} - {rlo.Address} {System.Environment.NewLine}";

                            //Check Availability for Each Room



                        }
                        _crooms = rListObj;
                    }

                }
                return retMsg;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> FindMeetingTimes(List<EmailAddress> attendees)
        {

            /*
             * adelev@m365x681393.onmicrosoft.com       Adele Vance
             * alland@m365x681393.onmicrosoft.com       Allan Deyoung
             * debrab@m365x681393.onmicrosoft.com       Debra Berger
             * adams@m365x681393.onmicrosoft.com        Conf Room Adams
             * baker@m365x681393.onmicrosoft.com        Conf Room Baker
             */

            MeetingTimeObject MTO = new MeetingTimeObject();
            foreach (EmailAddress attendee in attendees)
                MTO.attendees.Add(new Attendee(attendee,"required"));

           // MTO.locationConstraint = new LocationConstraint();
           // MTO.locationConstraint.isRequired = "true";
           // MTO.locationConstraint.suggestLocation = "true";
            MTO.meetingDuration = "PT2H";
            MTO.minimumAttendeePercentage = "100";
            MTO.returnSuggestionReasons = "true";

            FindTimeSuggestion FTSObj;
            string MTOJson = JsonConvert.SerializeObject(MTO);
            var payload = new StringContent(MTOJson, Encoding.UTF8, "application/json");
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Prefer", "outlook.timezone=\"Pacific Standard Time\"");
                // Send the `POST subscriptions` request and parse the response.
                HttpResponseMessage response = await client.PostAsync(_mythicalFindMtgTimesEndPoint, payload);
                string retMsg = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FTSObj = JsonConvert.DeserializeObject<FindTimeSuggestion>(retMsg);
                    if (FTSObj != null)
                    {
                        int ni = 0;
                        lastMsg = "";
                        FTSObj.meetingTimeSuggestions.Sort();
                        foreach (MeetingTimeSuggestions mts in FTSObj.meetingTimeSuggestions)
                        {
                            ni++;
                            lastMsg += $"{ni}. Confidence:{mts.confidence}% - {mts.suggestionReason} {System.Environment.NewLine}";
                            lastMsg += $"\tFrom:{mts.meetingTimeSlot.start.dateTime} - To:{mts.meetingTimeSlot.end.dateTime} {System.Environment.NewLine}";
                            lastMsg += $"\tLocations:{System.Environment.NewLine}";
                            foreach (location loc in mts.locations)
                            {
                                lastMsg += $"\t\t{loc.displayName}{System.Environment.NewLine}";
                            }
                        }
                        lastFTSObj = FTSObj;
                    }
                }
                return retMsg;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> FindMeetingTimeinNext3BusinessDays(List<EmailAddress> attendees)
        {

            /*
             * adelev@m365x681393.onmicrosoft.com       Adele Vance
             * alland@m365x681393.onmicrosoft.com       Allan Deyoung
             * debrab@m365x681393.onmicrosoft.com       Debra Berger
             * adams@m365x681393.onmicrosoft.com        Conf Room Adams
             * baker@m365x681393.onmicrosoft.com        Conf Room Baker
             */

            MeetingTimeObject MTO = new MeetingTimeObject();

            foreach (EmailAddress attendee in attendees)
                MTO.attendees.Add(new Attendee(attendee, "required"));
            // MTO.locationConstraint = new LocationConstraint();
            // MTO.locationConstraint.isRequired = "true";
            // MTO.locationConstraint.suggestLocation = "true";
            MTO.meetingDuration = "PT2H";
            MTO.minimumAttendeePercentage = "100";
            MTO.returnSuggestionReasons = "true";

            //Time Constraints
            MTO.timeConstraint = new TimeConstraint();
            MTO.timeConstraint.activityDomain = "unrestricted";
            MTO.timeConstraint.timeslots = new List<Timeslot>();
            DateTime startDateTime = DateTime.Now;
            DateTime endDateTime = DateTime.Now.AddDays(3);
            //"dateTime": "2017-04-17T09:00:00"
            string format = @"yyyy-MM-ddThh:mm:ss";
            MTO.timeConstraint.timeslots.Add(new Timeslot(startDateTime.ToString(format), endDateTime.ToString(format)));

            FindTimeSuggestion FTSObj;
            string MTOJson = JsonConvert.SerializeObject(MTO);
            var payload = new StringContent(MTOJson, Encoding.UTF8, "application/json");
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Send the `POST subscriptions` request and parse the response.
                HttpResponseMessage response = await client.PostAsync(_mythicalFindMtgTimesEndPoint, payload);
                string retMsg = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FTSObj = JsonConvert.DeserializeObject<FindTimeSuggestion>(retMsg);
                    if (FTSObj != null)
                    {
                        int ni = 0;
                        lastMsg = "";
                        FTSObj.meetingTimeSuggestions.Sort();
                        foreach (MeetingTimeSuggestions mts in FTSObj.meetingTimeSuggestions)
                        {
                            ni++;
                            lastMsg += $"{ni}. Confidence:{mts.confidence}% - {mts.suggestionReason} {System.Environment.NewLine}";
                            lastMsg += $"\tFrom:{mts.meetingTimeSlot.start.dateTime} - To:{mts.meetingTimeSlot.end.dateTime} {System.Environment.NewLine}";
                            lastMsg += $"\tLocations:{System.Environment.NewLine}";
                            foreach (location loc in mts.locations)
                            {
                                lastMsg += $"\t\t:{loc.displayName}{System.Environment.NewLine}";
                            }
                        }
                        lastFTSObj = FTSObj;
                    }
                }
                return retMsg;
            }
            catch (Exception ex)
            {
                lastMsg = ex.ToString();
                return ex.ToString();
            }
        }

        public string GetMeetingSuggestions()
        {
            int ni = 0;
            if (lastFTSObj == null) return null;
            lastMsg = "";
            foreach (MeetingTimeSuggestions mts in lastFTSObj.meetingTimeSuggestions)
            {
                ni++;
                lastMsg += $"{ni}. Confidence:{mts.confidence}% - {mts.suggestionReason} {System.Environment.NewLine}";
                lastMsg += $"\tFrom:{mts.meetingTimeSlot.start.dateTime} - To:{mts.meetingTimeSlot.end.dateTime} {System.Environment.NewLine}";
                lastMsg += $"\tLocations:{System.Environment.NewLine}";
                foreach (location loc in mts.locations)
                {
                    lastMsg += $"\t\t{loc.displayName}{System.Environment.NewLine}";
                }
            }
            return lastMsg;

        }

        public async Task<string> CreateEvent(int nSelFTS)
        {
            //Assuming lastFTSObj is populated and the user picked an item from the List of FindTimeSuggestions
            //nSelFTS is the index of the FTS Object that was selected from the List

            string retMsg = "";
            if (nSelFTS > lastFTSObj.meetingTimeSuggestions.Count() || nSelFTS < 0)
            {
                retMsg = $"Selected Index {nSelFTS} is OutofBounds";
                lastMsg = retMsg;
                return retMsg;
            }

            
            MeetingTimeSuggestions mts = lastFTSObj.meetingTimeSuggestions[nSelFTS];
            //Generate a Event Object from the selection
            Event calevent = new Event();

            //Loop through the Attendees for the Meeting
            calevent.subject = $"Generated by MythicalExperienceConsole On {DateTime.Now.ToString()}";
            calevent.body = new Body();
            calevent.body.content = $"Welcome to MythicalExperienceConsole............... ";
            calevent.body.contentType = $"HTML";
            calevent.start = new Start(mts.meetingTimeSlot.start.dateTime, "Pacific Standard Time");
            calevent.end = new End(mts.meetingTimeSlot.end.dateTime, "Pacific Standard Time");
            calevent.attendees = new List<Attendee>();
            foreach (AttendeeAvailability attAvail in mts.attendeeAvailability)
            {
                calevent.attendees.Add(attAvail.attendee);
            }
            //Add Conference Room as a Resource in the Attendee List
            //We pick the first conference room
            //Convert from location to Attendee
            Attendee confRoom = new Attendee(new EmailAddress(mts.locations[0].displayName, mts.locations[0].locationEmailAddress), "Resource");
            calevent.attendees.Add(confRoom);
            calevent.location = new EventLocation(mts.locations[0].displayName);

            //Generate JSON for CalEvent;
            string calEventJSON = JsonConvert.SerializeObject(calevent);
            var payload = new StringContent(calEventJSON, Encoding.UTF8, "application/json");



            retMsg = await GraphClientPOST(_mythicalExperienceCreateEventEndPoint, payload);
            lastMsg = retMsg;
            return retMsg;

        }

        private async Task<string> GraphClientPOST(string endPoint, StringContent payload)
        {
            string retMsg;

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Prefer", "outlook.timezone=\"Pacific Standard Time\"");
                // Send the `POST subscriptions` request and parse the response.
                HttpResponseMessage response = await client.PostAsync(endPoint, payload);
                retMsg = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                retMsg = ex.ToString();
            }
            return retMsg;
        }

        private async Task<string> GraphClientGET(string endPoint)
        {
            string retMsg;
            try
            {
                _authResult = await PublicClientApp.AcquireTokenSilentAsync(_Graphscopes, PublicClientApp.Users.FirstOrDefault());


            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                lastMsg = $"MsalUiRequiredException: {ex.Message}";
                try
                {
                    _authResult = await PublicClientApp.AcquireTokenAsync(_Graphscopes);
                }
                catch (MsalException msalex)
                {
                    lastMsg = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                    bGraphSignedIn = false;
                }
            }

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endPoint);
                HttpResponseMessage response = await client.SendAsync(request);
                retMsg = await response.Content.ReadAsStringAsync();
                retMsg = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                retMsg = ex.ToString();
            }
            return retMsg;
        }

        private async Task<bool> RefreshGraphToken()
        {
            try
            {
                _authResult = await PublicClientApp.AcquireTokenSilentAsync(_Graphscopes, PublicClientApp.Users.FirstOrDefault());
                bGraphSignedIn = true;
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                lastMsg = $"MsalUiRequiredException: {ex.Message}";
                try
                {
                    _authResult = await PublicClientApp.AcquireTokenAsync(_Graphscopes);
                    bGraphSignedIn = true;
                }
                catch (MsalException msalex)
                {
                    lastMsg = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                    bGraphSignedIn = false;
                }
            }

            return bGraphSignedIn;

        }


    }
}
