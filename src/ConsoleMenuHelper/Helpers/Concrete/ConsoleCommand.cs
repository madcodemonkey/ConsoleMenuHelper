using System;

namespace ConsoleMenuHelper
{
    /// <summary>A wrapper around console commands.</summary>
    public class ConsoleCommand : IConsoleCommand
    {
        /// <summary>Clears the screen</summary>
        public void Clear()
        {
            Console.Clear();
        }

        /// <summary>Obtains then next key that is pressed.</summary>
        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        /// <summary>Reads a line using the Console's ReadLine method.</summary>
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        /// <summary>Writes text using the Console's Write method</summary>
        /// <param name="line">Text to write</param>
        public void Write(string line)
        {
            Console.Write(line);
        }

        /// <summary>Writes text using the Console's WriteLine method</summary>
        /// <param name="line">Text to write</param>
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}