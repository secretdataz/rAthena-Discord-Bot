using System;
using System.Linq;
using System.Diagnostics;

namespace Discord_rAthenaBot
{
	partial class rAthenaBot
	{
		static void Main(string[] args)
		{
			// Mono code
			if (Type.GetType("Mono.Runtime") != null)
			{
				// Just run it without any checks because Mono complains
				new rAthenaBot();
			}
			else // Windows code
			{
				String thisprocessname = Process.GetCurrentProcess().ProcessName;

				if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) == 1)
				{
					new rAthenaBot();
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Application already running. Please close the existing application.");
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("Press any key to exit...");
					Console.ReadKey();
					Environment.Exit(1);
				}
			}
        }
    }
}
