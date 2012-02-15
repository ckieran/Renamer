using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renamer
{
    /// <summary>
    /// Severity levels for each parser message. Used by <see cref="ArgParser.AddFormat"/>
    /// </summary>
    public enum Levels
    {
        Info,
        Warning,
        Error
    }

    public class ArgParser
    {
        /// Dictionary of command line switches. The action is called when the switch was found in the command line.
        Dictionary<string, Action> mySwitches;

        /// Dictionary of command line switches which expect one parameter. The action is called when the switch was found in the command line
        Dictionary<string, Action<string>> mySwitchesWithArg;

        // If at the end of the command line parameters are left this delegate is called.
        Action<List<string>> myOtherArgs;

        // List of info, warning, error messages that happened while parsing and verification of the command line switches
        // They are printed via the PrintHelpWithMessages method.
        List<Message> myMessages = new List<Message>();

        // Supported command line switch tags 
        char[] myDelimiters = new char[] { '/', '-' };

        /// <summary>
        /// Construct a command line argument parser which can parse command line switches of the form /xxx -xxx.
        /// </summary>
        /// <param name="switches">Key is the command line switch. Value is an action that normally sets some boolean flag to true.</param>
        /// <exception cref="ArgumentNullExcepton">switches must not be null.</exception>
        public ArgParser(Dictionary<string, Action> switches)
            : this(switches, null, null)
        {
        }

        /// <summary>
        /// Construct a command line argument parser which can parse command line switches of the form /xxx -xxx and
        /// switches with one parameter like -x argument.
        /// </summary>
        /// <param name="switches">Key is the command line switch. Value is an action that normally sets some boolean flag to true.</param>
        /// <param name="switcheswithArg">Key is the command line switch. Value is an action that normally sets the passed parameter for this switch to a member variable. E.g. test.exe /files test.txt will call the delegate with the name files in the dictionary with test.txt as parameter. Can be null.</param>
        /// <exception cref="ArgumentNullExcepton">switches must not be null.</exception>
        public ArgParser(Dictionary<string, Action> switches, Dictionary<string, Action<string>> switcheswithArg)
            : this(switches, switcheswithArg, null)
        {
        }

        /// <summary>
        /// Construct a command line argument parser which can parse command line switches of the form /xxx -xxx and
        /// switches with one parameter like -x argument.
        /// </summary>
        /// <param name="switches">Key is the command line switch. Value is an action that normally sets some boolean flag to true.</param>
        /// <param name="switcheswithArg">Key is the command line switch. Value is an action that normally sets the passed parameter for this switch to a member variable. E.g. test.exe /files test.txt will call the delegate with the name files in the dictionary with test.txt as parameter. Can be null.</param>
        /// <param name="otherArgs">Callback which is called with a list of the command line arguments which do not belong to a specific command line parameter. E.g. test.exe aaa bbb ccc will call this method with an array with aaa,bbb,ccc. Can be null.</param>
        /// <exception cref="ArgumentNullExcepton">switches must not be null.</exception>
        public ArgParser(Dictionary<string, Action> switches, Dictionary<string, Action<string>> switcheswithArg, Action<List<string>> otherArgs)
        {
            if (switches == null)
                throw new ArgumentNullException("switches");

            mySwitches = CreateDictWithShortCuts(switches);
            myOtherArgs = otherArgs;

            if (switcheswithArg == null)
            {
                mySwitchesWithArg = new Dictionary<string, Action<string>>();
            }
            else
            {
                mySwitchesWithArg = CreateDictWithShortCuts(switcheswithArg);

                // check for duplicate keys 
                foreach (string key in mySwitches.Keys)
                {
                    if (mySwitchesWithArg.ContainsKey(key))
                    {
                        throw new ArgumentException(
                            String.Format("The command line switch -{0} occurs in both switches dictionaries. Please make it unambiguous.", key));
                    }
                }
            }
        }

        /// <summary>
        /// Parse command line and call the corresponding actions passed from the ctor for each
        /// found command line argument. 
        /// </summary>
        /// <param name="args">Command line array.</param>
        /// <returns>true when all command line switched could be parsed. false otherwise. In this case you should call
        /// PrintHelpWithMessages to print your help string. Then all validation errors will be printed as well.</returns>
        public bool ParseArgs(string[] args)
        {
            if (args.Length == 0)
            {
                AddFormat(Levels.Error, "No arguments specified.");
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                // get command line parameter for current argument if there is one
                string parameter = (i + 1 < args.Length) ? args[i + 1] : null;
                parameter = IsSwitch(parameter) ? null : parameter;

                if (parameter != null) // Advance counter to next command line argument which does not belong to the current one
                {
                    i++;
                }

                if (IsSwitch(arg))
                {
                    string strippedArg = arg.Substring(1).ToLower(); // command line switches are not case sensitive

                    if (true == mySwitches.OnValue(strippedArg)) // Set Flag for simple command line switch
                    {
                        if (parameter != null)
                        {
                            if (myOtherArgs == null)
                            {
                                AddFormat(Levels.Error, "Superflous argument ({0}) for command line switch {1}", parameter, arg);
                            }
                            else // Other arguments present then process it if this was the last command line argument
                            {
                                if (CallOtherArgs(args, i))  // all arguments are processed when this returns true
                                {
                                    break;
                                }
                            }
                        }
                        continue;
                    }

                    // Set for given flag the passed parameter for this flag
                    bool? ret = mySwitchesWithArg.OnValueAndParameterNotNull(strippedArg, parameter);

                    // not found then it must be an unknown command line switch
                    if (null == ret)
                    {
                        AddFormat(Levels.Error, "Unknown command line switch {0}", arg);
                    }
                    else if (false == ret)  // Found but argument was missing
                    {
                        AddFormat(Levels.Error, "Missing data for command line switch {0}", arg);
                    }
                    else // when command was ok perhaps we have some arguments left if this was the last argument
                    {
                        if (CallOtherArgs(args, i + 1)) // all arguments are processed when this returns true
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (CallOtherArgs(args, (parameter == null) ? i : i - 1))
                    {
                        break; // all arguments are processed when this returns true
                    }
                    else
                    {
                        AddFormat(Levels.Error, "Not a command line switch: {0}", arg);
                        if (parameter != null)
                        {
                            AddFormat(Levels.Error, "Not a command line switch: {0}", parameter);
                        }
                    }
                }
            }

            return myMessages.Count == 0;
        }

        /// <summary>
        /// Check if first character is one of the allowed command line tags.
        /// </summary>
        /// <param name="arg">command line argument to check. Can be null.</param>
        /// <returns>true if it is an command line switch, false otherwise.</returns>
        bool IsSwitch(string arg)
        {
            if (String.IsNullOrEmpty(arg))
                return false;

            return Array.Exists(myDelimiters, (char c) => arg[0] == c);
        }

        /// <summary>
        /// Add a message to the list of messages which is displayed after parsing and parameter validation.
        /// </summary>
        /// <param name="level">Message type</param>
        /// <param name="format">Message format string</param>
        /// <param name="args">Optional message parameters</param>
        public void AddFormat(Levels level, string format, params object[] args)
        {
            myMessages.Add(new Message { Level = level, Text = String.Format(format, args) });
        }

        /// <summary>
        /// Call the delegate for the "other" arguments when no more command line switches are present.
        /// </summary>
        /// <param name="args">The command line argument array</param>
        /// <param name="start">The start index from where the search starts.</param>
        /// <returns>true if the delegate was called with the arguments. false otherwise.</returns>
        bool CallOtherArgs(string[] args, int start)
        {
            List<string> ret = new List<string>();
            for (int i = start; i < args.Length; i++)
            {
                string curr = args[i];
                if (IsSwitch(curr)) // when a switch is found this is not the last argument. Do not process it
                {
                    ret = null;
                    break;
                }
                else
                {
                    ret.Add(curr);
                }
            }

            if (myOtherArgs != null && ret != null)
            {
                myOtherArgs(ret);
                return true;
            }
            else
            {
                return false;
            }
        }

        private Dictionary<string, T> CreateDictWithShortCuts<T>(Dictionary<string, T> switches)
        {
            Dictionary<string, T> ret = new Dictionary<string, T>();
            // Generate shortcut names from upper case letters of command line arguments
            foreach (var kvp in switches)
            {
                ret.Add(kvp.Key.ToLower(), kvp.Value);

                string shortCut = new string(
                                   (from c in kvp.Key
                                    where Char.IsUpper(c)
                                    select c).ToArray()).ToLower();
                if (shortCut != "")
                {
                    if (ret.ContainsKey(shortCut))
                    {
                        throw new ArgumentException(
                            String.Format("The generated shortcut \"-{0}\" from \"-{1}\" collides with another command line switch.", shortCut, kvp.Key));
                    }
                    ret.Add(shortCut, kvp.Value);
                }
            }

            return ret;
        }

        /// <summary>
        /// Print help and colored error and warning messages to the console.
        /// </summary>
        /// <param name="helpString">Help string which is printed first. Can be null.</param>
        public void PrintHelpWithMessages(string helpString)
        {
            if (helpString != null)
            {
                Console.WriteLine(helpString);
            }

            foreach (var message in myMessages)
            {
                string text = message.Text;
                if (message.Level == Levels.Warning)
                {
                    text = "Warning: " + text;
                }
                else if (message.Level == Levels.Error)
                {
                    text = "Error: " + text;
                }

                Console.WriteLine(text);
            }
        }

        struct Message
        {
            public Levels Level;
            public string Text;
        }

    }
}
