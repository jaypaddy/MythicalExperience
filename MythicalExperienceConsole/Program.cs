using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace MythicalExperienceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool NotExitWhile = true;
            string givenCMD;
            string[] Avlcommands = { "getroomlist", "listrooms", "findrooms", "meetnow", "meet3d",  "createevent", "subscribe","listnotifications","refresh","exit" };
            string[] CmdStatus = { "Completed", "Completed", "**To Be Implemented**", "Completed", "Completed", "Completed", "Completed", "**To Be Implemented**", "**To Be Implemented**", "Completed" };
            List<EmailAddress> attendees;


            //Invoke Client
            MythicalExperienceClient MEClient = new MythicalExperienceClient();

            

            //Auto SignIn
            Console.WriteLine("Please Signin...");

            Console.WriteLine("Connecting to Graph...");
            MEClient.SignInToGraph().Wait();
            Console.WriteLine(MEClient.GetLastMsg());
            if (!MEClient.bGraphSignedIn)
            {
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Connecting to Outlook...");
            MEClient.SignInToOutlook().Wait();
            Console.WriteLine(MEClient.GetLastMsg());

            if (MEClient.bGraphSignedIn && MEClient.bOutlookSignedIn)
                Console.WriteLine("Connected to Graph & Outlook");
            else if (MEClient.bOutlookSignedIn)
                Console.WriteLine("Connected to Outlook...");
            else if (MEClient.bGraphSignedIn)
                Console.WriteLine("Connected to Graph");

            Console.WriteLine("Enter any of the following commands...");
            int nLoop = 0;
            foreach (var cmd in Avlcommands)
            {
                Console.WriteLine($"    {cmd}\t\t\t{CmdStatus[nLoop]}");
                nLoop++;
            }
            while (NotExitWhile)
            {
                Console.Write($"MythicalExperience:>");
                givenCMD = Console.ReadLine();
                switch (givenCMD)
                {
                    case "getroomlist":
                        if (!MEClient.bOutlookSignedIn)
                        {
                            Console.WriteLine("Not Connected to Outlook");
                            break;
                        }
                        MEClient.GetRoomLists().Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
                    case "listrooms":
                        if (!MEClient.bOutlookSignedIn)
                        {
                            Console.WriteLine("Not Connected to Outlook");
                            break;
                        }
                        MEClient.ListRooms().Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
                    case "findrooms":
                        Console.WriteLine("***Not Implemented***");
                        break;
                    case "meet3d":
                        Console.WriteLine("Looking availability to meet in the next 3 Days with:");
                        attendees = new List<EmailAddress>();
                        attendees.Add(new EmailAddress("Adele Vance", "adelev@m365x681393.onmicrosoft.com"));
                        attendees.Add(new EmailAddress("Allan Deyoung", "alland@m365x681393.onmicrosoft.com"));
                        foreach (EmailAddress attendee in attendees)
                            Console.WriteLine($"{attendee.name} - {attendee.address}");
                        MEClient.FindMeetingTimeinNext3BusinessDays(attendees).Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
                    case "meetnow":
                        Console.WriteLine("Looking availability to meet NOW Hours with:");
                        attendees = new List<EmailAddress>();
                        attendees.Add(new EmailAddress("Adele Vance", "adelev@m365x681393.onmicrosoft.com"));
                        attendees.Add(new EmailAddress("Allan Deyoung", "alland@m365x681393.onmicrosoft.com"));
                        foreach (EmailAddress attendee in attendees )
                            Console.WriteLine($"{attendee.name} - {attendee.address}");
                        MEClient.FindMeetingTimes(attendees).Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
                    case "createevent":
                        Console.WriteLine("Checking for Suggestions to pick from...");
                        string retMsg = MEClient.GetMeetingSuggestions();
                        if (retMsg == null)
                        {
                            Console.WriteLine("Execute meetnow or meet3d before creating event.");
                            break;
                        }
                        Console.WriteLine(retMsg);
                        Console.Write("Specify an Event:");
                        string selIndex = Console.ReadLine();
                        if (selIndex == null)
                        {
                            Console.WriteLine("Missing Event");
                            break;
                        }
                        
                        MEClient.CreateEvent(Convert.ToInt32(selIndex)).Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
                    case "subscribe":
                        MEClient.CreateSub().Wait();
                        Console.WriteLine(MEClient.GetLastMsg());

                        break;
                    case "listnotifications":
                        Console.WriteLine("***Not Implemented***");
                        break;
                    case "exit":
                        Console.WriteLine("Sign Out...");
                        MEClient.SignOut();
                        NotExitWhile = false;
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
                    default:
                        Console.WriteLine($"Invalid command {{givenCMD}}. Please retry");
                        break;
                }
            }

        }
    }
}
