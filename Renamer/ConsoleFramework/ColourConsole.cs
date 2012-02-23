using System;
using System.Text;
using System.IO;

namespace Renamer.ConsoleFramework
{
    /// <summary>
    /// Colored console class which does color the console output which do contain specific keywords.
    /// The only thing you need to do is to instantiate this class and use Console.WriteLine as usual.
    /// </summary>
    class ColourConsole : TextWriter
    {
        TextWriter myOriginal = Console.Out;

        Func<string, ConsoleColor?> myColorizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColourConsole"/> class.
        /// </summary>
        /// <param name="colorizer">The colorizer function which does return the color for each line to be printed to console.</param>
        public ColourConsole(Func<string, ConsoleColor?> colorizer)
        {
            if (colorizer == null)
            {
                throw new ArgumentNullException("colorizer");
            }

            myColorizer = colorizer;

            if (!IsRedirected)
            {
                // Replace Console.Out with our own colorizing instance which will be removed on dispose
                Console.SetOut(this);
            }
        }

        public override Encoding Encoding
        {
            get { return myOriginal.Encoding; }
        }

        public override void WriteLine(string format)
        {
            // we do not need to set the color every time only for the ones which do return a color
            ConsoleColor? newColor = myColorizer(format);

            if (newColor != null)
            {
                ConsoleColor original = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = newColor.Value;
                    myOriginal.WriteLine(format);
                }
                finally
                {
                    Console.ForegroundColor = original;
                }
            }
            else
            {
                myOriginal.WriteLine(format);
            }
        }

        public override void Write(char[] buffer)
        {
            myOriginal.Write(buffer);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!IsRedirected)
            {
                Console.SetOut(myOriginal);
            }
        }

        static bool? _isRedirected;
        static bool IsRedirected
        {
            get
            {
                if (_isRedirected == null)
                    _isRedirected = IsConsoleRedirected();
                return _isRedirected.Value;
            }
        }

        static bool IsConsoleRedirected()
        {
            try
            {
                // this is the easiest way to check if sdtout is redirected. Then this property throws
                // an exception.
                bool visible = Console.CursorVisible;
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

    }
}
