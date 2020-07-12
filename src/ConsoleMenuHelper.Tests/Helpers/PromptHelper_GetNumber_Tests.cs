using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleMenuHelper.Tests
{
    [TestClass]
    public class PromptHelper_GetNumber_Tests
    {
        private readonly Mock<IConsoleCommand> _mockConsole;
        public PromptHelper_GetNumber_Tests()
        {
            _mockConsole = new Mock<IConsoleCommand>();
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow(" ", null)]
        [DataRow("ABC", null)]
        [DataRow("1", 1)]
        [DataRow("13", 13)]
        [DataRow("-4", -4)]
        public void Overload1_ReturnsCorrectValue(string input, int? expectedOutput)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadLine()).Returns(input);

            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            int? actualOutput = cut.GetNumber(null, 1);


            // Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void Overload1_WhenOrderedToPromptTwiceItDoes()
        {
            // Arrange
            _mockConsole.SetupSequence(s => s.ReadLine())
                .Returns("Not_a_number").Returns("still_not_a_number");

            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            int? actualOutput = cut.GetNumber(null, 2);


            // Assert
            _mockConsole.Verify(v => v.ReadLine(), Times.Exactly(2));
        }
        
        [DataTestMethod]
        [DataRow(null, 0)]
        [DataRow("", 0)]
        [DataRow(" ", 0)]
        [DataRow("?", 1)]
        [DataRow("What's your favorite number?", 1)]
        public void Overload1_UsesThePromptIfGivenOne(string promptMessage, int usageCount)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadLine()).Returns("5");

            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            cut.GetNumber(promptMessage, 1);


            // Assert
            _mockConsole.Verify(v => v.WriteLine(promptMessage), Times.Exactly(usageCount));
        }


        [DataTestMethod]
        [DataRow(null, -100, "No input results in default")]
        [DataRow("", -100, "No input results in default")]
        [DataRow(" ", -100, "No input results in default")]
        [DataRow("ABC", -100, "No number results in default")]
        [DataRow("1", 1, "Should work it's within the range of -20 to 20!")]
        [DataRow("13", 13, "Should work it's within the range of -20 to 20!")]
        [DataRow("20", 20, "Should work it's within the range of -20 to 20!")]
        [DataRow("21", -100, "Should NOT work it's NOT within the range of -20 to 20!")]
        [DataRow("-4", -4, "Should work it's within the range of -20 to 20!")]
        [DataRow("-20", -20, "Should work it's within the range of -20 to 20!")]
        [DataRow("-21", -100, "Should NOT work it's NOT within the range of -20 to 20!")]
        public void Overload2_ReturnsCorrectValue(string input, int? expectedOutput, string message)
        {
            // Arrange
            _mockConsole.SetupSequence(s => s.ReadLine())
                .Returns(input).Returns("exit");

            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            int? actualOutput = cut.GetNumber(null, -20, 20, "exit", -100);


            // Assert
            Assert.AreEqual(expectedOutput, actualOutput, message);
        }


           
        [DataTestMethod]
        [DataRow(null, 0, "No prompt specified so nothing should be called!")]
        [DataRow("", 0, "No prompt specified so nothing should be called!")]
        [DataRow(" ", 0, "No prompt specified so nothing should be called!")]
        [DataRow("?", 1, "Called once because the user gives a valid number (1)")]
        [DataRow("What's your name?", 1, "Called once because the user gives a valid number (1)")]
        public void Overload2_UsesThePromptIfGivenOne(string promptMessage, int usageCount, string message)
        {
            // Arrange
            _mockConsole.Setup(s => s.ReadLine()).Returns("1");
            
            var cut = new PromptHelper(_mockConsole.Object);

            // Act
            cut.GetNumber(promptMessage, 1, 2, "exit", 0);
            
            // Assert
            _mockConsole.Verify(v => v.WriteLine(promptMessage), Times.Exactly(usageCount), message);
        }

    }
}
