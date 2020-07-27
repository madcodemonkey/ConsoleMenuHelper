using System;
using System.Linq;
using System.Text;

namespace ConsoleMenuHelper
{
    /// <summary>Helps with retrieving console input</summary>
    public class PromptHelper : IPromptHelper
    {
        private readonly IConsoleCommand _console;

        /// <summary>Constructor</summary>
        public PromptHelper(IConsoleCommand console)
        {
            _console = console;
        }

        /// <summary>Prompts with whatever the user passes in and requires the user to enter one of the valid answers</summary>
        /// <param name="promptMessage">An optional prompt that will be written to the screen before reading the input data.</param>
        /// <param name="ignoreCase">Indicates if the answers in the <paramref name="validAnswers"/> array are case sensitive.</param>
        /// <param name="validAnswers">The list of valid answers.</param>
        public char GetCharacter(string promptMessage, bool ignoreCase, params char[] validAnswers)
        {
            if (validAnswers == null || validAnswers.Length == 0) throw new ArgumentException("Please specify a valid answer array!");

            string validAnswerSting = BuildAnswerString(validAnswers);

            while (true)
            {
                if (string.IsNullOrWhiteSpace(promptMessage) == false)
                {
                    _console.WriteLine(promptMessage);
                }

                _console.WriteLine(validAnswerSting);
                ConsoleKeyInfo someKey = _console.ReadKey();

                // So that no text written after the user gives an answer is on the same line...
                Console.WriteLine(""); 

                foreach (var validAnswer in validAnswers)
                {
                    if (someKey.KeyChar.ToString().Equals(validAnswer.ToString(), ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                    {
                        return someKey.KeyChar;
                    }
                }
            }
        }


        /// <summary>Gets a number the specified number of times before giving up.  Returns null if no number was collected; otherwise, you get a number.</summary>
        /// <param name="promptMessage">An optional prompt that will be written to the screen before reading the input data.</param>
        /// <param name="numberOfAttempts">Number of attempts to make before returning.</param>
        public int? GetNumber(string promptMessage, int numberOfAttempts)
        {
            int attemptCount = 0;
            while (attemptCount < numberOfAttempts)
            {
                if (string.IsNullOrWhiteSpace(promptMessage) == false)
                {
                    _console.WriteLine(promptMessage);
                }

                string input = _console.ReadLine();
                
                if (int.TryParse(input, out var numberInput))
                {
                    return numberInput;
                }
                
                attemptCount++;
            }

            return null;
        }

        /// <summary>Gets a number within the specified range.</summary>
        /// <param name="promptMessage">An optional prompt that will be written to the screen before reading the input data.</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="exitText">The text that will allow the user to exit.</param>
        /// <param name="exitValue">The value to return if the user decides to exit.</param>
        public int? GetNumber(string promptMessage, int min, int max, string exitText, int? exitValue)
        {
            while (true)
            {
                if (string.IsNullOrWhiteSpace(promptMessage) == false)
                {
                    _console.WriteLine(promptMessage);
                }

                string input = _console.ReadLine();

                if (input == exitText) return exitValue;

                string message;
                if (int.TryParse(input, out var numberInput))
                {
                    if (numberInput >= min && numberInput <= max)
                    {
                        return numberInput;
                    }

                    message = $"*******Please enter a valid number between {min} and {max} -OR- type {exitText} to leave *******";
                }
                else
                {
                    message = $"*******Please enter a number -OR- type {exitText} to leave *******";
                }
                
                _console.WriteLine("  ");
                _console.WriteLine(message);
            }
        }

        /// <summary>Prompts with whatever the user passes in and requires the user to enter one of the valid answers</summary>
        /// <param name="promptMessage">An optional prompt that will be written to the screen before reading the input data.</param>
        /// <param name="ignoreCase">Indicates if the answers in the <paramref name="validAnswers"/> array are case sensitive.</param>
        /// <param name="validAnswers">The list of valid answers.</param>
        public string GetText(string promptMessage, bool ignoreCase, params string[] validAnswers)
        {
            if (validAnswers == null || validAnswers.Length == 0) throw new ArgumentException("Please specify a valid answer array!");

            string validAnswerSting = BuildAnswerString(validAnswers);

            while (true)
            {
                if (string.IsNullOrWhiteSpace(promptMessage) == false)
                {
                    _console.WriteLine(promptMessage);
                }

                _console.WriteLine(validAnswerSting);
                string result = _console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(result)) continue;

                if (validAnswers.Any(w => string.Compare(w, result, ignoreCase) == 0))
                {
                    return result;
                }
            }
        }
        
        /// <summary>Gets text from the user.</summary>
        /// <param name="promptMessage">An optional prompt that will be written to the screen before reading the input data.</param>
        /// <param name="acceptBlank">Can the user enter blank or null (hit enter)</param>
        /// <param name="trimResult">Indicates if the text should be trimmed before returning</param>
        /// <returns></returns>
        public string GetText(string promptMessage, bool acceptBlank, bool trimResult)
        {
            string input;
            do
            {
                if (string.IsNullOrWhiteSpace(promptMessage) == false)
                {
                    _console.WriteLine(promptMessage);
                }

                input = _console.ReadLine();
                if (acceptBlank)
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(input) == false)
                {
                    break;
                }
            }
            while (true);

            if (trimResult == false || input == null) return input;

            return input.Trim();
        }

        /// <summary>Prompts with whatever the user passes in and requires the user to enter y or n</summary>
        public bool GetYorN(string promptMessage)
        {
            char result = GetCharacter(promptMessage, true, 'y', 'n');
            return result == 'y' || result == 'Y';
        }

        /// <summary>Builds a string of valid answers for the user.</summary>
        /// <param name="validAnswers">Possible answers</param>
        /// <returns></returns>
        private string BuildAnswerString(string[] validAnswers)
        {
            var sb = new StringBuilder("(Enter ");

            for (var index = 0; index < validAnswers.Length; index++)
            {
                var validAnswer = validAnswers[index];

                if (index == 0)
                {
                    sb.Append(validAnswer);
                }
                else if (index == validAnswers.Length - 1)
                {
                    // Last item
                    sb.Append($" or {validAnswer}");
                }
                else
                {
                    // Somewhere in the middle
                    sb.AppendFormat(", {0}", validAnswer);
                }
            }

            sb.Append(" and hit enter)");

            return sb.ToString();
        }

        /// <summary>Builds a string of valid answers for the user.</summary>
        /// <param name="validAnswers">Possible answers</param>
        /// <returns></returns>
        private string BuildAnswerString(char[] validAnswers)
        {
            var sb = new StringBuilder("(Enter ");

            for (var index = 0; index < validAnswers.Length; index++)
            {
                var validAnswer = validAnswers[index];

                if (index == 0)
                {
                    sb.Append(validAnswer);
                }
                else if (index == validAnswers.Length - 1)
                {
                    // Last item
                    sb.Append($" or {validAnswer}");
                }
                else
                {
                    // Somewhere in the middle
                    sb.AppendFormat(", {0}", validAnswer);
                }
            }

            sb.Append(" and hit enter)");

         
            return sb.ToString();
        }
    }
}
