using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

/*
 * Thanks go to Alois Kraus, I found his colour console template and decided to give it a go for this application
 * 
 * His blog post on the template can be found here: 
 * http://geekswithblogs.net/akraus1/archive/2010/11/11/142685.aspx
 * 
 * The Visual Studio Gallery installer can be found here:
 * http://visualstudiogallery.msdn.microsoft.com/51606523-019b-40d6-989a-88465dd6a6aa/?SRC=Home
 * 
 */


namespace Renamer
{
    class Program
    {
        static string HelpStr =
            String.Format("Renamer (c) 2012 by Chris Kieran v{0}{1}", Assembly.GetExecutingAssembly().GetName().Version, Environment.NewLine) +
            "Renames folders based on image dates within the folder" + Environment.NewLine +
            "Usage: " + Environment.NewLine +
            "Renamer [-OptionaL] -Folder <file> <other arguments>" + Environment.NewLine +
            " -Folder <folder>    Sample switch with argument." + Environment.NewLine +
            " [-OptionaL]          RenameFiles: Optional command line switch" + Environment.NewLine +
            " Examples: " + Environment.NewLine +
            " Renamer " + Environment.NewLine +
            " Renamer -f folder" + Environment.NewLine +
            " Renamer -f folder arg1 arg2 arg3" + Environment.NewLine +
            " Renamer -RenameFiles -folder folder arg1 arg2 arg3" + Environment.NewLine +
            " Renamer -RenameFiles -folder folder" + Environment.NewLine +
            Environment.NewLine;

        #region Parsed Command Line Switches
        public bool RenameFiles
        {
            get;
            set;
        }

        public string Folder
        {
            get;
            set;
        }

        public List<string> OtherArgs
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Main entry point which is directly called from main where nothing happens except exception catching.
        /// </summary>
        /// <param name="args"></param>
        public Program(string[] args)
        {
            // define parameterless command line switches. 
            // Please note: Upper case characters define the shortcut name for each switch
            var switches = new Dictionary<string, Action>
            {
                {"RenameFiles", () => RenameFiles=true }, // shortcut -rf
            };

            // define command line switches which take one parameter
            var switchWithArg = new Dictionary<string, Action<string>>
            {
                {"Folder", (arg) => Folder = arg },  // shortcut -F
            };

            // Handler for <other arguments> if present
            Action<List<string>> rest = (parameters) => OtherArgs = parameters;

            ArgParser parser = new ArgParser(switches, switchWithArg, rest);

            // check if command line is well formed
            if (!parser.ParseArgs(args))
            {
                parser.PrintHelpWithMessages(HelpStr);
                return;
            }

            if (!ValidateArgs(parser))
            {
                // Display errors but not the help screen.
                parser.PrintHelpWithMessages(null);
            }
            else
            {
                // Ok we can start
                Run();
            }
        }

        private void Run()
        {
            // Place here your actual code to execute your logic after the command line has been parsed and validated.

            Console.WriteLine("Got Optional: {0}", RenameFiles);
            Console.WriteLine("Got Folder: {0}", Folder);

            if (OtherArgs != null)
            {
                Console.WriteLine("Info: Additional Arg Count: {0}", OtherArgs.Count);
                foreach (var addtionalArg in OtherArgs)
                {
                    Console.WriteLine("Additional: {0}", addtionalArg);
                }
            }
        }


        /// <summary>
        /// Check if the passed command line arguments do contain valid data
        /// </summary>
        /// <param name="parser">used for error reporting</param>
        /// <returns>true if all parameters are valid. false otherwise.</returns>
        private bool ValidateArgs(ArgParser parser)
        {
            bool lret = true;

            /*    if (String.IsNullOrEmpty(Folder) )
                {
                    parser.AddFormat(Levels.Error, "No arg for -Folder passed.");
                    return false;
                }
             */

            return lret;
        }

        static void Main(string[] args)
        {
            using (ColourConsole console = new ColourConsole(Colorizer))
            {
                try
                {
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    Program p = new Program(args);
                }
                catch (Exception ex)
                {
                    PrintError("Error: {0}", ex);
                }
            }
        }

        /// <summary>
        /// Set console color depending on currently to be printed line.
        /// </summary>
        /// <param name="line">line to be printed</param>
        /// <returns>ConsoleColor if color needs to be set. Null otherwise.</returns>
        static ConsoleColor? Colorizer(string line)
        {
            ConsoleColor? col = null;

            if (line.StartsWith("Error"))
                col = ConsoleColor.Red;
            else if (line.StartsWith("Info"))
                col = ConsoleColor.Green;

            return col;
        }

        /// <summary>
        /// Catch uncatched exceptions thrown from other threads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            PrintError("Unhandled exception: {0}", (Exception)e.ExceptionObject);
        }

        static void PrintError(string format, Exception ex)
        {
            Trace.TraceError(format, ex);
            Console.WriteLine(format, ex.Message);
        }
    }
}
