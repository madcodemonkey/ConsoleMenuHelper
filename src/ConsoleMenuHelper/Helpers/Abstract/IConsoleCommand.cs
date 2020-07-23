using System;

namespace ConsoleMenuHelper
{
    /// <summary>A wrapper around console commands.</summary>
    public interface IConsoleCommand
    {
        /// <summary>Clears the screen</summary>
        void Clear();

        /// <summary>Obtains then next key that is pressed.</summary>
        ConsoleKeyInfo ReadKey();

        /// <summary>Reads a line using the Console's ReadLine method.</summary>
        string ReadLine();

        /// <summary>Writes text using the Console's Write method</summary>
        /// <param name="line">Text to write</param>
        void Write(string line);

        /// <summary>Writes text using the Console's WriteLine method</summary>
        /// <param name="line">Text to write</param>
        void WriteLine(string line);
    }
}