﻿using System;
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
        public void ReadLineImplementation_ContinuesToPromptTillItGetsTheCorrectAnswer()
        {
            // Arrange
            _mockConsole.SetupSequence(s => s.ReadLine())
                .Returns(" ").Returns("z").Returns("s").Returns("Y");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            var actualResult = cut.GetYorN("Shall I proceed?", true);
            
            // Assert
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void  ReadKeyImplementation_ContinuesToPromptTillItGetsTheCorrectAnswer()
        {
            // Arrange
            _mockConsole.SetupSequence(s => s.ReadKey())
                .Returns(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false))
                .Returns(new ConsoleKeyInfo('z', ConsoleKey.Z, false, false, false))
                .Returns(new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false))
                .Returns(new ConsoleKeyInfo('Y', ConsoleKey.Y, true, false, false));
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            var actualResult = cut.GetYorN("Shall I proceed?", false);
            
            // Assert
            Assert.IsTrue(actualResult);
        }

        [DataTestMethod]
        [DataRow(null, 0, "No prompt specified so nothing should be called!")]
        [DataRow("", 0, "No prompt specified so nothing should be called!")]
        [DataRow(" ", 0, "No prompt specified so nothing should be called!")]
        [DataRow("?", 1, "Called once because we return a valid answer ('n')")]
        [DataRow("What's your name?", 1, "Called once because we return a valid answer ('n')")]
        public void ReadKeyImplementation_UsesThePromptIfGivenOne(string promptMessage, int numberOfTimesPrompted, string message)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadKey()).Returns(new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false));
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            cut.GetYorN(promptMessage, false);
            
            // Assert
            _mockConsole.Verify(v => v.WriteLine(promptMessage), Times.Exactly(numberOfTimesPrompted), message);
        }  

        
        [DataTestMethod]
        [DataRow(null, 0, "No prompt specified so nothing should be called!")]
        [DataRow("", 0, "No prompt specified so nothing should be called!")]
        [DataRow(" ", 0, "No prompt specified so nothing should be called!")]
        [DataRow("?", 1, "Called once because we return a valid answer ('n')")]
        [DataRow("What's your name?", 1, "Called once because we return a valid answer ('n')")]
        public void ReadLineImplementation_UsesThePromptIfGivenOne(string promptMessage, int numberOfTimesPrompted, string message)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadLine()).Returns("n");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            cut.GetYorN(promptMessage, true);
            
            // Assert
            _mockConsole.Verify(v => v.WriteLine(promptMessage), Times.Exactly(numberOfTimesPrompted), message);
        }  
    }
}
