using System;
using System.Collections.Generic;
using System.Text;
using ConsoleMenuHelper.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleMenuHelper.Tests.Helpers
{
    [TestClass]
    public class PromptHelper_GetYorN_Tests
    {
        private readonly Mock<IConsoleCommand> _mockConsole;
        public PromptHelper_GetYorN_Tests()
        {
            _mockConsole = new Mock<IConsoleCommand>();
        }

        [TestMethod]
        public void ContinuesToPromptTillItGetsTheCorrectAnswer()
        {
            // Arrange
            _mockConsole.SetupSequence(s => s.ReadLine())
                .Returns("jack").Returns((string)null).Returns("").Returns(" ").Returns("z").Returns("james").Returns("Y");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            var actualResult = cut.GetYorN("Shall I proceed?");
            
            // Assert
            Assert.IsTrue(actualResult);
        }

        [DataTestMethod]
        [DataRow(null, 0, "No prompt specified so nothing should be called!")]
        [DataRow("", 0, "No prompt specified so nothing should be called!")]
        [DataRow(" ", 0, "No prompt specified so nothing should be called!")]
        [DataRow("?", 1, "Called once because we return a valid answer ('n')")]
        [DataRow("What's your name?", 1, "Called once because we return a valid answer ('n')")]
        public void UsesThePromptIfGivenOne(string promptMessage, int numberOfTimesPrompted, string message)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadLine()).Returns("n");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            cut.GetYorN(promptMessage);
            
            // Assert
            _mockConsole.Verify(v => v.WriteLine(promptMessage), Times.Exactly(numberOfTimesPrompted), message);
        }  
    }
}
