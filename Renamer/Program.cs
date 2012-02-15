using System;

namespace Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var arg  in args)
            {
                Console.WriteLine("arg was:" + arg);
            }
        }
    }
}
