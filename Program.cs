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
            bool NotExitWhile = true, exitAuth=false;
            string givenCMD;
            string[] command;

            //I should make AvlCommands and CmdStatus a single String Array...
            string[] Avlcommands = {       "listusers #"        ,"getroomlist"      ,"listrooms"    ,"findrooms",
                                           "meetnow"            ,"meet3d"           ,"createevent"  ,"subscribe",
                                           "people",
                                           "listemails"         ,"listnotifications","refresh"      ,"exit" };



            string[] CmdStatus = {          "Done"              ,"Done"             ,"Done"         ,"**Backlog**",
                                            "Done"              ,"Done"             ,"Done"         , "Done",
                                            "Done",
                                            "Done"              ,"Done"             , "**Backlog**" , "Done" };

            string[] B2CPolicy = { "B2C_1A_signup_signin", "B2C_1A_signup_signin", "B2C_1A_MythicalBeastSUSI", "B2C_1A_SignUpOrSignInSocialLocal"};

            List<EmailAddress> attendees;
            MythicalExperienceClient MEClient=null;
            int nRecordCount = 0;

            //Choose Your Auth Authority
            while (NotExitWhile)
            {
                Console.WriteLine("Please Choose Your Signin Authority...");
                Console.WriteLine("1. Azure AD (OAuth 2.0)");
                Console.WriteLine("2. Azure AD B2C - Local, Social, AAD");
                Console.WriteLine("3. Azure AD B2C - Local & AAD (with jll.com blacklisting)");
                Console.WriteLine("4. Azure AD B2C - AAD");
                Console.WriteLine("5. Azure AD B2C - Local & Social (jll.com blacklisting)");
                Console.WriteLine("6. Exit");
                givenCMD = Console.ReadLine();

                switch(givenCMD)
                {
                    case "1":
                        //AAD
                        MEClient = new MythicalExperienceClient("aad");
                        NotExitWhile = false;
                        break;
                    case "2":
                        MEClient = new MythicalExperienceClient(B2CPolicy[0]); //All Up
                        NotExitWhile = false;
                        break;
                    case "3":
                        MEClient = new MythicalExperienceClient(B2CPolicy[1]); //B2C_1A_SignUpOrSignInLocalAAD
                        NotExitWhile = false;
                        break;
                    case "4":
                        MEClient = new MythicalExperienceClient(B2CPolicy[2]);
                        NotExitWhile = false;
                        break;
                    case "5":
                        //AADB2C Local, AAD
                        MEClient = new MythicalExperienceClient(B2CPolicy[3]);
                        NotExitWhile = false;
                        break;
                    case "6":
                        NotExitWhile = false;
                        exitAuth = true;
                        break;
                    default:
                        break;

                }
            }
            //If Exiting Auth, then exit Program
            if (exitAuth) return;

            NotExitWhile = true;
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

            Console.WriteLine("Enter any of the following commands... to view all commands type list");
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
                //Parse the Command by space
                command = givenCMD.Split(' ');
                switch (command[0])
                {
                    case "listusers":
                        if (!MEClient.bGraphSignedIn)
                        {
                            Console.WriteLine("Not Connected to Graph");
                            break;
                        }
                        if (command.Count() > 1)
                            nRecordCount = Convert.ToInt32(command[1]);
                        else
                            nRecordCount = 0;
                        if (nRecordCount == 0) nRecordCount = 1000;
                        MEClient.ListUsers(nRecordCount).Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
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
                        //attendees.Add(new EmailAddress("Adele Vance", "adelev@m365x681393.onmicrosoft.com"));
                        //attendees.Add(new EmailAddress("Allan Deyoung", "alland@m365x681393.onmicrosoft.com"));
                        //attendees.Add(new EmailAddress("Jay Padmanabhan", "japadman@microsoft.com"));
                        foreach (EmailAddress attendee in attendees)
                            Console.WriteLine($"{attendee.name} - {attendee.address}");
                        MEClient.FindMeetingTimeinNext3BusinessDays(attendees).Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
                    case "meetnow":
                        Console.WriteLine("Please specify time period 30,60,90,120,240");
                        string mtgDuration = Console.ReadLine();
                        switch (mtgDuration)
                        {
                            case "60":
                                mtgDuration = "PT1H";
                                break;
                            case "90":
                                mtgDuration = "PT1H30M";
                                break;
                            case "120":
                                mtgDuration = "PT2H";
                                break;
                            default:
                            case "30":
                                mtgDuration = "PT30M";
                                break;
                        }
                        Console.WriteLine("Looking availability to meet NOW Hours with:");
                        attendees = new List<EmailAddress>();
                        //attendees.Add(new EmailAddress("Adele Vance", "adelev@m365x681393.onmicrosoft.com"));
                        //attendees.Add(new EmailAddress("Allan Deyoung", "alland@m365x681393.onmicrosoft.com"));
                        //attendees.Add(new EmailAddress("Rajesh Chaganti", "rajesh.chaganti@cbre.com"));
                        //attendees.Add(new EmailAddress("Jay Padmanabhan", "japadman@microsoft.com"));
                        foreach (EmailAddress attendee in attendees )
                            Console.WriteLine($"{attendee.name} - {attendee.address}");
                        MEClient.FindMeetingTimes(attendees, mtgDuration).Wait();
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
                        
                        MEClient.CreateEvent(Convert.ToInt32(selIndex)-1).Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;
                    case "subscribe":
                        MEClient.CreateSub().Wait();
                        Console.WriteLine(MEClient.GetLastMsg());

                        break;
                    case "listnotifications":
                        Console.WriteLine("***Connecting to Table Storage***");
                        TblStorageClient tsc = new TblStorageClient();
                        Console.WriteLine(tsc.GetNotifications());
                        break;
                    case "exit":
                        Console.WriteLine("Sign Out...");
                        MEClient.SignOut();
                        NotExitWhile = false;
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;

                    case "list":
                        nLoop = 0;
                        foreach (var cmd in Avlcommands)
                        {
                            Console.WriteLine($"    {cmd}\t\t\t{CmdStatus[nLoop]}");
                            nLoop++;
                        }
                        break;


                    case "listemails":
                        string now = DateTime.Now.ToString("yyyy-MM-dd");
                        Console.WriteLine($"Getting Received messages since {now}...");
                        MEClient.GetMessages(now).Wait();
                        Console.WriteLine(MEClient.GetLastMsg());
                        break;

                    case "people":
                        Console.WriteLine($"Getting Relevant People...");
                        MEClient.GetRelevantPeople().Wait();
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
