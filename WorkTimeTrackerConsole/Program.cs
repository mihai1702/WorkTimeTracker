using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    public static uint GetIdleTime()
    {
        LASTINPUTINFO lii = new LASTINPUTINFO();
        lii.cbSize = (uint)Marshal.SizeOf(lii);

        if (GetLastInputInfo(ref lii))
        {
            uint millis = (uint)Environment.TickCount - lii.dwTime;
            return millis / 1000; 
        }
        return 0;
    }



    private static readonly string ApiBaseUrl = "http://localhost:5108/api/WorkSession/";
    private static readonly HttpClient client = new HttpClient();
    public static bool activity=false;
    public static int idling=0;
    public static string username = "";
    static async Task Main(string[] args)
    {

        Console.WriteLine("Welcome to WorkTimeTracker Console App");
        _ = Task.Run(async () =>
        {
            while (true)
            {
                if (activity==true)
                {
                    uint idleTime = GetIdleTime();
                    if (idleTime > 5 && idleTime<10 && idling==0)
                    {
                        Console.WriteLine("[Monitor] The user was inactive for more than 5 seconds");
                        idling=1;
                        
                    }
                    if(idleTime>=10)
                    {
                        Console.WriteLine("[Monitor] The user was inactive for more than 15 minutes. Pausing Session");
                        PauseSession(username);
                        idling=0;
                        break;
                    }
                }

                await Task.Delay(1000);
            }
        });

        bool sessionStarted = false;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Start Session");
            Console.WriteLine("2. Stop Session");
            Console.WriteLine("3. Pause Session");
            Console.WriteLine("4. Resume Session");
            Console.WriteLine("5. Exit");
            Console.Write("Please choose an option: ");
            
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (!sessionStarted)
                    {
                        Console.Write("Enter your username: ");
                        username = Console.ReadLine();
                        if (!string.IsNullOrEmpty(username))
                        {
                            await StartSession(username);
                        }
                        else
                        {
                            Console.WriteLine("Username is required.");
                        }
                        sessionStarted = true;
                    }
                    else
                    {
                        Console.WriteLine("Session already started!");
                    }
                    break;

                case "2":
                    if (sessionStarted)
                    {
                        await StopSession(username);
                        sessionStarted = false;
                    }
                    else
                    {
                        Console.WriteLine("No active session to stop.");
                    }
                    break;

                case "3":
                    if (sessionStarted)
                    {
                        await PauseSession(username);
                    }
                    else
                    {
                        Console.WriteLine("No active session to pause.");
                    }
                    break;

                case "4":
                    if (sessionStarted)
                    {
                        await ResumeSession(username);
                    }
                    else
                    {
                        Console.WriteLine("No active session to resume.");
                    }
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    }

    private static async Task StartSession(string username)
    {
        var content = new StringContent($"\"{username}\"", Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{ApiBaseUrl}start", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Session started for {username}.");
            activity=true;
        }
        else
        {
            Console.WriteLine($"Failed to start session for {username}. Error: {response.ReasonPhrase}");
        }
    }

    private static async Task StopSession(string username)
    {
        var content = new StringContent($"\"{username}\"", Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{ApiBaseUrl}stop", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Session stopped for {username}.");
            activity=false;
        }
        else
        {
            Console.WriteLine($"Failed to stop session for {username}. Error: {response.ReasonPhrase}");
        }
    }

    private static async Task PauseSession(string username)
    {
        var content = new StringContent($"\"{username}\"", Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{ApiBaseUrl}pause", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Session paused for {username}.");
            activity=false;
        }
        else
        {
            Console.WriteLine($"Failed to pause session for {username}. Error: {response.ReasonPhrase}");
        }
    }

    private static async Task ResumeSession(string username)
    {
        var content = new StringContent($"\"{username}\"", Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{ApiBaseUrl}resume", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Session resumed for {username}.");
            activity=true;
        }
        else
        {
            Console.WriteLine($"Failed to resume session for {username}. Error: {response.ReasonPhrase}");
        }
    }

}