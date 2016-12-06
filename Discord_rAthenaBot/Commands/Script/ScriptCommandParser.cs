using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Discord_rAthenaBot.Commands.Script
{
    public class ScriptCommandParser
    {
        public List<ScriptCommand> commands = new List<ScriptCommand>();
        public bool ready { get; set; }
        private string[] content;

        public ScriptCommandParser(Uri uri)
        {
            using(WebClient wc = new WebClient())
            {
                wc.DownloadFileAsync(uri, "script_commands.txt");
                wc.DownloadFileCompleted += (o, e) => {
                    if(!e.Cancelled && e.Error == null)
                    {
                        ready = true;
                        content = File.ReadAllLines("script_commands.txt");
                    }
                };
            }
        }

        public ScriptCommandParser(string[] FileContent)
        {
            ready = true;
            content = FileContent;
        }

        public bool Parse()
        {
            bool enableParse = false;
            bool skip = false;
            ScriptCommand sc = null;
            int nullCount = 0;
            foreach(string line in content)
            {
                if (!enableParse)
                {
                    if (line.StartsWith("1.- Basic commands."))
                        enableParse = true;
                } else
                {
                    if (line.Equals("---------------------------------------"))
                    {
                        if (sc != null && !String.IsNullOrEmpty(sc.Name))
                            commands.Add(sc);
                        sc = new ScriptCommand();
                        nullCount = 0;
                        skip = false;
                    } else
                    {
                        if (sc == null || skip)
                            continue;
                        if(!string.IsNullOrWhiteSpace(line))
                            sc.Descriptions.Add(line);
                        else
                        {
                            nullCount++;
                            if (nullCount > 2)
                                skip = true;
                        }
                        if (line.StartsWith("*"))
                        {
                            Regex rgx = new Regex(@"\*(\w+)");
                            if (rgx.IsMatch(line))
                            {
                                Match match = rgx.Match(line);
                                Group g = match.Groups[1];
                                if (g.Success)
                                    sc.Name = g.Value;
                            }
                        }
                    }
                }
            }
            return commands.Count > 0;
        }
    }
}
