namespace ConsoleMenuHelper
{
    /// <summary>Helps with retrieving console input</summary>
    public interface IPromptHelper
    {
        /// <summary>Gets a number the specified number of times before giving up.  Returns null if no number was collected; otherwise, you get a number.</summary>
        /// <param name="promptMessage">An optional prompt that will be written to the screen before reading the input data.</param>
        /// <param name="numberOfAttempts">Number of attempts to make before returning.</param>
        int? GetNumber(string promptMessage, int numberOfAttempts);
        
        /// <summary>Gets a number within the specified range.</summary>
        /// <param name="promptMessage">An optional prompt that will be written to the screen before reading the input data.</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="exitText">The text that will allow the user to exit.</param>
        /// <param name="exitValue">The value to return if the user decides to exit.</param>
        int? GetNumber(string promptMessage, int min, int max, string exitText, int? exitValue);

        /// <summary>Prompts with whatever the user passes in and requires the user to enter one of the valid answers</summary>
        /// <param name="promptMessage">An optional prompt that will be written to the screen before reading the input data.</param>
        /// <param name="ignoreCase">Indicates if the answers in the <paramref name="validAnswers"/> array are case sensitive.</param>
        /// <param name="validAnswers">The list of valid answers.</param>
        string GetText(string promptMessage, bool ignoreCase, params string[] validAnswers);

        /// <summary>Gets text from the user.</summary>
        /// <param name="promptMessage">The question to prompt with</param>
        /// <param name="acceptBlank">Can the user enter blank or null (hit enter)</param>
        /// <param name="trimResult">Indicates if the text should be trimmed before returning</param>
        /// <returns></returns>
        string GetText(string promptMessage, bool acceptBlank, bool trimResult);

        /// <summary>Prompts with whatever the user passes in and requires the user to enter y or n</summary>
        bool GetYorN(string promptMessage);
    }
}