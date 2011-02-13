using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpCommand
{
    class Program
    {
        class Parser
        {
            public string Text;
            public int Index = 0;

            public Parser(string txt)
            {
                Text = txt;
                Index = 0;
            }
            public bool IsEnd()
            {
                return Text == null || Index == -1 || Index >= Text.Length;
            }
            public bool Find(string pattern)
            {
                Index = Text.IndexOf(pattern, Index);
                return (Index != -1);
            }
            public string GetQuoted()
            {
                if (!IsEnd())
                {
                    if (Find("\""))
                    {
                        int start = ++Index;
                        if (Find("\""))
                        {
                            int end = Index++;
                            return Text.Substring(start, end - start);
                        }
                    }
                }
                return null;
            }
        };

        static List<string> GetFiles(string directory)
        {
            List<string> result = new List<string>();
            try
            {
                Parser p = new Parser(directory);
                while (!p.IsEnd())
                {
                    if (p.Find("<li><a href="))
                    {
                        string name = p.GetQuoted();
                        if (name != null)
                        {
                            result.Add(name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception = " + ex.ToString());
            }
            return result;
        }

        static string Source = null;
        static string Target = ".";

        static bool Subdirectories = false;

        static int Copy(System.Net.WebClient web, string source, string target)
        {
            int count = 0;
            string dir = "";
            try
            {
                dir = web.DownloadString(source);
            }
            catch (Exception)
            {
                System.Console.WriteLine("Failed = " + source);
                return 0;
            }
            List<string> files = GetFiles(dir);
            foreach (string f in files)
            {
                string fname = f.Replace("%20", " ");

                if (fname.StartsWith(".."))
                {
                    // Ignore
                }
                else if (fname.EndsWith("/")) // Directory?
                {
                    if (Subdirectories)
                    {
                        string src = System.IO.Path.Combine(source, fname);
                        string dst = System.IO.Path.Combine(target, fname);
                        count += Copy(web, src, dst);
                    }
                }
                else
                {
                    try
                    {
                        string web_file = System.IO.Path.Combine(source, fname);
                        string local_file = System.IO.Path.Combine(target, fname);
                        string folder = System.IO.Path.GetDirectoryName(local_file);
                        System.IO.Directory.CreateDirectory(folder);
                        string rel = web_file.Substring(Source.Length+1);
                        System.Console.WriteLine("File = " + rel);
                        web.DownloadFile(web_file, local_file);
                        count += 1;
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception = " + fname + "\n" + ex.ToString());
                    }
                }
            }
            return count;
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                if (arg[0] == '-')
                {
                    if (arg.ToUpper() == "-S")
                    {
                        Subdirectories = true;
                        System.Console.WriteLine("Subdirectories = ON");
                    }
                }
                else if (Source == null)
                {
                    Source = arg;
                    System.Console.WriteLine("Source = " + Source);
                }
                else if (Target == ".")
                {
                    Target = arg;
                    System.Console.WriteLine("Target = " + Target);
                }
            }

            if (Source == null)
            {
                System.Console.WriteLine("HttpCommand - Copyright (c) 2011 Paul Isaac");
                System.Console.WriteLine("Utility to download files from a web server.");
                System.Console.WriteLine("");
                System.Console.WriteLine(@"Useage: HttpCommand {flags} [http:\\uri] {local-target}");
                System.Console.WriteLine(@"Flags: -S = recurse subdirectories");
                System.Console.WriteLine("");
                System.Console.WriteLine(@"For HTTP resources the GET method is used.");
                System.Console.WriteLine(@"For FTP resources the RETR command is used.");
                return;
            }

            System.Console.WriteLine("Downloading...");
            System.Net.WebClient web = new System.Net.WebClient();
            int count = Copy(web, Source, Target);
            string msg = string.Format("\nDownloaded {0} files.", count);
            System.Console.WriteLine(msg);
        }
    }
}
