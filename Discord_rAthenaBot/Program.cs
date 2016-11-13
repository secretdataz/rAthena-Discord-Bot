using System;
using System.Linq;
using System.Diagnostics;

namespace Discord_rAthenaBot
{
    partial class rAthenaBot
    {
        static void Main(string[] args)
        {
            String thisprocessname = Process.GetCurrentProcess().ProcessName;

            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) == 1)
            {
                rAthenaBot bot = new rAthenaBot();
            }
            else
            {
                Console.WriteLine("Application already running. Please close the existing application.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
    }
}
