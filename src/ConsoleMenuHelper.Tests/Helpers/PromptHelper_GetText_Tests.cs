using System;
using System.Collections.Generic;
using System.Text;
using ConsoleMenuHelper.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleMenuHelper.Tests.Helpers
{
    [TestClass]
    public class PromptHelper_GetText_Tests
    {
        private readonly Mock<IConsoleCommand> _mockConsole;
        public PromptHelper_GetText_Tests()
        {
            _mockConsole = new Mock<IConsoleCommand>();
        }

        [TestMethod]
        public void Overload1_ContinuesToPromptTillItGetsTheCorrectAnswer()
        {
            // Arrange
            _mockConsole.SetupSequence(s => s.ReadLine())
                .Returns("jack").Returns((string)null).Returns("").Returns(" ").Returns("z").Returns("james").Returns("c");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            var actualText = cut.GetText(null, true, "a", "b", "c");
            
            // Assert
            Assert.AreEqual("c", actualText);
        }

        [DataTestMethod]
        [DataRow(null, 0, "No prompt specified so nothing should be called!")]
        [DataRow("", 0, "No prompt specified so nothing should be called!")]
        [DataRow(" ", 0, "No prompt specified so nothing should be called!")]
        [DataRow("?", 1, "Called once because we return a valid answer ('n')")]
        [DataRow("What's your name?", 1, "Called once because we return a valid answer ('n')")]
        public void Overload1_UsesThePromptIfGivenOne(string promptMessage, int numberOfTimesPrompted, string message)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadLine()).Returns("n");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            cut.GetText(promptMessage, true, "y", "n");
            
            // Assert
            _mockConsole.Verify(v => v.WriteLine(promptMessage), Times.Exactly(numberOfTimesPrompted), message);
        }
        
        [DataTestMethod]
        [DataRow("james", false, true, "james")]
        [DataRow(" james ", false, true, "james")]
        [DataRow(" james ", false, false, " james ")]
        [DataRow("", true, true, "")]
        [DataRow(null, true, true, null)]
        public void Overload2_ReturnsTheCorrectText(string input, bool acceptBlank, bool trimResult, string expectedText)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadLine()).Returns(input);
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            var actualText = cut.GetText(null, acceptBlank, trimResult);
            
            // Assert
            Assert.AreEqual(expectedText, actualText);
        }


        [TestMethod]
        public void Overload2_ContinuesToPromptIfTextCannotBeBlank()
        {
            // Arrange
            _mockConsole.SetupSequence(s => s.ReadLine())
                .Returns("").Returns((string)null).Returns("").Returns(" ").Returns(" JaMes ");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            var actualText = cut.GetText(null, false, true);
            
            // Assert
            Assert.AreEqual("JaMes", actualText);
        }


        [DataTestMethod]
        [DataRow(null, 0, "No prompt specified so nothing should be called!")]
        [DataRow("", 0, "No prompt specified so nothing should be called!")]
        [DataRow(" ", 0, "No prompt specified so nothing should be called!")]
        [DataRow("?", 1, "Called once because we return a valid answer ('n')")]
        [DataRow("What's your name?", 1, "Called once because we return a valid answer ('n')")]
        public void Overload2_UsesThePromptIfGivenOne(string promptMessage, int numberOfTimesPrompted, string message)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadLine()).Returns("n");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            cut.GetText(promptMessage, true, true);
            
            // Assert
            _mockConsole.Verify(v => v.WriteLine(promptMessage), Times.Exactly(numberOfTimesPrompted), message);
        }
    }
}
