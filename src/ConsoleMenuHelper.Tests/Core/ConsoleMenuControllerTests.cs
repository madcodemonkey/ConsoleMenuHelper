using System.Collections.Generic;
using System.Threading.Tasks;
using ConsoleMenuHelper.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleMenuHelper.Tests.Core
{
    [TestClass]
    public class ConsoleMenuControllerTests
    {
        private readonly Mock<IConsoleMenuRepository> _mockConsoleMenuRepository;
        private readonly Mock<IConsoleCommand> _mockConsoleCommand;
        private readonly Mock<IPromptHelper> _mockPromptHelper;

        public ConsoleMenuControllerTests()
        {
            _mockConsoleCommand = new Mock<IConsoleCommand>();
            _mockConsoleMenuRepository = new Mock<IConsoleMenuRepository>();
            _mockPromptHelper = new Mock<IPromptHelper>();
        }
        
        [TestMethod]
        public async Task  Title_IfSpecifiedItIsWrittenToTheScreen()
        {
            // Arrange
            var classUnderTest = new ConsoleMenuController(_mockConsoleMenuRepository.Object, _mockConsoleCommand.Object, _mockPromptHelper.Object);
            string titleOfMenu1 = "Title of menu 1";

            BuildThreeLevelMenu(classUnderTest, string.Empty, string.Empty);

            _mockPromptHelper.Setup(s => s.GetNumber(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(0); //Users exits menu1 and the app too

            // Act
            await classUnderTest.DisplayMenuAsync("Menu1", titleOfMenu1);

            // Assert
            _mockConsoleCommand.Verify(v => v.WriteLine(titleOfMenu1), Times.Once);
        }
        
        [TestMethod]
        public async Task Breadcrumb_IfIsSpecifiedItIsWrittenToTheScreen()
        {
            // Arrange
            string titleOfMenu1 = "Title of menu 1";
            string titleOfMenu2 = "Title of menu 2";
            string titleOfMenu3 = "Title of menu 3";

            var classUnderTest = new ConsoleMenuController(_mockConsoleMenuRepository.Object, _mockConsoleCommand.Object, _mockPromptHelper.Object);

            BuildThreeLevelMenu(classUnderTest, titleOfMenu2, titleOfMenu3);

            _mockPromptHelper.SetupSequence(s => s.GetNumber(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(3)  // User presses 3 and goes to menu2
                .Returns(3)  // User presses 3 and goes to menu3
                .Returns(0)  // Users exits menu3 and returns to menu2
                .Returns(0)  // Users exits menu2 and returns to menu1
                .Returns(0); // Users exits menu1 and the app too

            // Act
            await classUnderTest.DisplayMenuAsync("Menu1", titleOfMenu1, BreadCrumbType.Concatenate);

            // Assert
            _mockConsoleCommand.Verify(v => v.WriteLine(titleOfMenu1), Times.Exactly(2));
            _mockConsoleCommand.Verify(v => v.WriteLine($"{titleOfMenu1} > {titleOfMenu2}"), Times.Exactly(2));
            _mockConsoleCommand.Verify(v => v.WriteLine($"{titleOfMenu1} > {titleOfMenu2} > {titleOfMenu3}"), Times.Once);
        }

        [DataTestMethod]
        [DataRow("T1", "T2", "T3",      
            "T1", "T1 > T2", "T1 > T2 > T3", 
            "Developer specified a title for all three menus")]
        [DataRow("", "T2", "T3",         
            "", "T2", "T2 > T3", 
            "Developer did NOT specified a title for menu 1.  Results in some breadcrumb trails")]
        [DataRow("T1", "", "T3",      
            "T1", "", "T3", 
            "Developer did NOT specified a title for menu 2.  Results in no breadcrumb trails because none have parents with titles")]
        [DataRow("T1", "T2", "",      
            "T1", "T1 > T2", "", 
            "Developer did NOT specified a title for menu 3.  Results in some breadcrumb trails")]
        public async Task Breadcrumb_ParentDoesNotHaveTitleItIsExcludedFromTheTrail(
            string titleOfMenu1, string titleOfMenu2, string titleOfMenu3,
            string expectMenu1, string expectMenu2, string expectMenu3, 
            string message)
        {
            // Arrange
            var classUnderTest = new ConsoleMenuController(_mockConsoleMenuRepository.Object, _mockConsoleCommand.Object, _mockPromptHelper.Object);

            BuildThreeLevelMenu(classUnderTest, titleOfMenu2, titleOfMenu3);

            _mockPromptHelper.SetupSequence(s => s.GetNumber(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(3)  // User presses 3 and goes to menu2
                .Returns(3)  // User presses 3 and goes to menu3
                .Returns(0)  // Users exits menu3 and returns to menu2
                .Returns(0)  // Users exits menu2 and returns to menu1
                .Returns(0); // Users exits menu1 and the app too

            // Act
            await classUnderTest.DisplayMenuAsync("Menu1", titleOfMenu1, BreadCrumbType.Concatenate);

            // Assert
            if (string.IsNullOrWhiteSpace(expectMenu1) == false)
                _mockConsoleCommand.Verify(v => v.WriteLine(expectMenu1), Times.Exactly(2), message);
            if (string.IsNullOrWhiteSpace(expectMenu2) == false)
                _mockConsoleCommand.Verify(v => v.WriteLine(expectMenu2), Times.Exactly(2), message);
            if (string.IsNullOrWhiteSpace(expectMenu3) == false)
                _mockConsoleCommand.Verify(v => v.WriteLine(expectMenu3), Times.Once, message);
        }

        private void BuildThreeLevelMenu(ConsoleMenuController classUnderTest, string titleOfMenu2, string titleOfMenu3 )
        {
            
            // MENU 1
            var menu1List = new List<ConsoleMenuItemWrapper>
            {
                new ConsoleMenuItemWrapper {Item = new TestMenuItem("Test 1"), ItemNumber = 1, TheType = typeof(TestMenuItem)},
                new ConsoleMenuItemWrapper {Item = new TestMenuItem("Test 2"), ItemNumber = 2, TheType = typeof(TestMenuItem)},
                new ConsoleMenuItemWrapper {Item = new TestSubMenuItem("Menu2", "Test 2", titleOfMenu2, BreadCrumbType.Concatenate, classUnderTest), ItemNumber = 3, TheType = typeof(TestSubMenuItem)}, 
                new ConsoleMenuItemWrapper {Item = new ExitConsoleMenuItem(), ItemNumber = 0, TheType = typeof(ExitConsoleMenuItem)}
            };
            _mockConsoleMenuRepository.Setup(s => s.LoadMenus("Menu1")).Returns(menu1List);
            _mockConsoleMenuRepository.Setup(s => s.CreateMenuItems(menu1List)).Returns(menu1List);

            // MENU 2
            var menu2List = new List<ConsoleMenuItemWrapper>
            {
                new ConsoleMenuItemWrapper {Item = new TestMenuItem("Test 4"), ItemNumber = 1, TheType = typeof(TestMenuItem)}, 
                new ConsoleMenuItemWrapper {Item = new TestMenuItem("Test 5"), ItemNumber = 2, TheType = typeof(TestMenuItem)}, 
                new ConsoleMenuItemWrapper {Item = new TestSubMenuItem("Menu3", "Test 4", titleOfMenu3, BreadCrumbType.Concatenate, classUnderTest), ItemNumber = 3, TheType = typeof(TestSubMenuItem)}, 
                new ConsoleMenuItemWrapper {Item = new ExitConsoleMenuItem(), ItemNumber = 0, TheType = typeof(ExitConsoleMenuItem)}
            };
            _mockConsoleMenuRepository.Setup(s => s.LoadMenus("Menu2")).Returns(menu2List);
            _mockConsoleMenuRepository.Setup(s => s.CreateMenuItems(menu2List)).Returns(menu2List);

            // MENU 3
            var menu3List = new List<ConsoleMenuItemWrapper>
            {
                new ConsoleMenuItemWrapper {Item = new TestMenuItem("Test 7"), ItemNumber = 1, TheType = typeof(TestMenuItem)}, 
                new ConsoleMenuItemWrapper {Item = new TestMenuItem("Test 8"), ItemNumber = 2, TheType = typeof(TestMenuItem)}, 
                new ConsoleMenuItemWrapper {Item = new TestMenuItem("Test 9"), ItemNumber = 3, TheType = typeof(TestSubMenuItem)}, 
                new ConsoleMenuItemWrapper {Item = new ExitConsoleMenuItem(), ItemNumber = 0, TheType = typeof(ExitConsoleMenuItem)}
            };
            _mockConsoleMenuRepository.Setup(s => s.LoadMenus("Menu3")).Returns(menu3List);
            _mockConsoleMenuRepository.Setup(s => s.CreateMenuItems(menu3List)).Returns(menu3List);

        }
    }

    [ConsoleMenuItem("Hello")]
    internal class TestMenuItem : IConsoleMenuItem
    {
        public TestMenuItem(){}

        public TestMenuItem(string text)
        {
            ItemText = text;
        }
        public Task<ConsoleMenuItemResponse> WorkAsync()
        {
            return Task.FromResult(new ConsoleMenuItemResponse(false, true));
        }

        public string ItemText { get; set; } = "Test";

        public string AttributeData { get; set; }
    }

    [ConsoleMenuItem("Hello")]
    internal class TestSubMenuItem : IConsoleMenuItem
    {
        private readonly string _menu;
        private readonly string _title;
        private readonly BreadCrumbType _breadCrumbType;
        private readonly IConsoleMenuController _controller;
        public TestSubMenuItem(){}

        public TestSubMenuItem(string menu, string text, string title, BreadCrumbType breadCrumbType, IConsoleMenuController controller)
        {
            _menu = menu;
            _title = title;
            _breadCrumbType = breadCrumbType;
            _controller = controller;
            ItemText = text;
        }
        public async Task<ConsoleMenuItemResponse> WorkAsync()
        {
            await _controller.DisplayMenuAsync(_menu, _title, _breadCrumbType);
            return new ConsoleMenuItemResponse(false, true);
        }

        public string ItemText { get; set; } = "Test";

        public string AttributeData { get; set; }
    }
}
