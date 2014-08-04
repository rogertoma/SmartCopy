using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SmartCopy
{
    class Program
    {
        static bool verboseLogging = false;

        static void Main(string[] args)
        {
            args = makeAllLowerCase(args);
            string source;
            string destination;
            bool overwrite = false;

            if (args.Contains("-h"))
            {
                Console.WriteLine("SmartCopy.exe Help");
                Console.WriteLine("-source where to copy from");
                Console.WriteLine("-destination where to copy file to");
                Console.WriteLine("-verbose [optional default false] will log all files and directories copied");
                Console.WriteLine("-overwrite [optional default false] will determine whether desintation files are replaced");
                return;
            }

            source = GetAttribValue("-source", true, args);
            if (String.IsNullOrEmpty(source))
            {
                Console.WriteLine("Need a source location to perform copy, use -source flag");
                return;
            }

            destination = GetAttribValue("-destination", true, args);
            if (String.IsNullOrEmpty(destination))
            {
                Console.WriteLine("Need a source location to perform copy, use -destination flag");
                return;
            }

            string temp = GetAttribValue("-verbose", false, args);
            if (!string.IsNullOrEmpty(temp))
            {
                if (temp.Substring(0, 1).ToLower() == "t")
                {
                    verboseLogging = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("WARNING: verbose flag value was not recognized.  Ignoring input");
                    Console.ResetColor();
                }
            }

            temp = GetAttribValue("-overwrite", false, args);
            if (!string.IsNullOrEmpty(temp))
            {
                if (temp.Substring(0, 1).ToLower() == "t")
                {
                    overwrite = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("WARNING: overwrite flag value was not recognized.  Ignoring input");
                    Console.ResetColor();
                }
            }

            DirectoryCopy(source, destination, overwrite);
        }

        static void DirectoryCopy(string source, string destination, bool overwrite)
        {
            bool copySubDirs = true;
            DirectoryInfo from = new DirectoryInfo(source);
            DirectoryInfo[] dirs = from.GetDirectories();

            Log(String.Format("Copying directory {0}", source));

            if (!from.Exists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Source directory does not exist");
                Console.ResetColor();
                return;
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destination))
            {
                Log(String.Format("Destination directory not found creating it: {0}", destination));
                Directory.CreateDirectory(destination);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = from.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destination, file.Name);
                if (overwrite)
                {
                    Log(string.Format("File copied {0}", file.Name));
                    file.CopyTo(temppath, true);
                }
                else
                {
                    if (!File.Exists(temppath))
                    {
                        Log(string.Format("File copied {0}", file.Name));
                        file.CopyTo(temppath, true);
                    }
                }
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destination, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, overwrite);
                }
            }

        }

        static void Log(string logString)
        {
            if (verboseLogging)
            {
                Console.WriteLine(logString);
            }
        }

        static string GetAttribValue(string field, bool mandatory, string[] args)
        {
            string value = "";

            if (args.Contains(field))
            {
                int valuePosition = args.ToList().IndexOf(field) + 1;

                if (args.Count() > valuePosition)
                {
                    value = args[valuePosition];
                }
            }

            if (mandatory && String.IsNullOrEmpty(value))
            {
                Console.WriteLine("The field {0} is mandatory ", field);
                Console.Write("{0} : ", field);
                return Console.ReadLine();
            }
            else
            {
                return value;
            }
        }

        static string[] makeAllLowerCase(string[] args)
        {
            string[] lowerAgs = new string[args.Count()];

            for (int i = 0; i < args.Count(); i++)
            {
                lowerAgs[i] = args[i].ToLower();
            }

            return lowerAgs;
        }
    }
}
