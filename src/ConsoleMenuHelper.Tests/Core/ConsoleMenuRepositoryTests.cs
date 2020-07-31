using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ConsoleMenuHelper.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleMenuHelper.Tests.Core
{
    [TestClass]
    public class ConsoleMenuRepositoryTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
     
        public ConsoleMenuRepositoryTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
        }


        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void  IfMenuCannotBeFoundAnArgumentExceptionIsThrown()
        {
            // Arrange
            var classUnderTest = new ConsoleMenuRepository(_mockServiceProvider.Object);
            
            // Act
            classUnderTest.LoadMenus("Menu1");

            // Assert
            Assert.Fail("There are no menus so we should get an exception.");
        }

    }
}
