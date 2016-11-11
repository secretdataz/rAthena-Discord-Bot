using System;

namespace Discord_rAthenaBot
{
    partial class rAthenaBot
    {
        static void Main(string[] args)
        {
            Console.Title = Settings.Default.ConsoleTitle;
            rAthenaBot bot = new rAthenaBot();
        }
    }
}
