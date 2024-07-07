using ElevatorSimulation.Domain.Entities;

namespace ElevatorSimulation.InterfaceAdapters.Tests
{
    using ElevatorSimulation.Domain.Enums;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    public class ConsoleControllerTests
    {
        private readonly Mock<IElevatorSystem> mockElevatorSystem;
        private readonly ConsoleController consoleController;

        public ConsoleControllerTests()
        {
            mockElevatorSystem = new Mock<IElevatorSystem>();
            consoleController = new ConsoleController(mockElevatorSystem.Object);
        }

        [Fact]
        public void DisplayMenu_Should_Show_Options()
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            ConsoleController.DisplayMenu();

            // Assert
            var expectedOutput = "Select an option:\r\n" +
                                 "1. Call Elevator\r\n" +
                                 "2. Move Elevator\r\n" +
                                 "3. Show Elevator Status\r\n" +
                                 "q. Quit\r\n";
            Assert.Equal(expectedOutput, sw.ToString());
        }

        [Fact]
        public void CallElevator_Should_CallElevator_With_Valid_Input()
        {
            // Arrange
            var input = new StringReader("1\n3\n2\nq\n");
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            // Act
            consoleController.Run();

            // Assert
            mockElevatorSystem.Verify(es => es.CallElevator(3, 2), Times.Once);

            // Cleanup
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void CallElevator_Should_Show_Error_On_Invalid_Floor_Input()
        {
            using var sr = new StringReader("invalid\n");
            using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            // Act
            consoleController.CallElevator();

            // Assert
            Assert.Contains("Invalid floor number.", sw.ToString());
            mockElevatorSystem.Verify(es => es.CallElevator(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CallElevator_Should_Show_Error_On_Invalid_Passenger_Input()
        {
            using var sr = new StringReader("5\ninvalid");
            using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            // Act
            consoleController.CallElevator();

            // Assert
            Assert.Contains("Invalid number of passengers.", sw.ToString());
            mockElevatorSystem.Verify(es => es.CallElevator(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void MoveElevator_Should_MoveElevator_With_Valid_Input()
        {
            using var sr = new StringReader("1\r\n5");
            Console.SetIn(sr);

            // Act
            consoleController.MoveElevator();

            // Assert
            mockElevatorSystem.Verify(es => es.MoveElevator(1, 5), Times.Once);
        }

        [Fact]
        public void MoveElevator_Should_Show_Error_On_Invalid_ElevatorId_Input()
        {
            using var sr = new StringReader("invalid\n");
            using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            // Act
            consoleController.MoveElevator();

            // Assert
            Assert.Contains("Invalid elevator ID.", sw.ToString());
            mockElevatorSystem.Verify(es => es.MoveElevator(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void MoveElevator_Should_Show_Error_On_Invalid_Floor_Input()
        {
            using var sr = new StringReader("1\ninvalid");
            using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            // Act
            consoleController.MoveElevator();

            // Assert
            Assert.Contains("Invalid floor number.", sw.ToString());
            mockElevatorSystem.Verify(es => es.MoveElevator(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void ShowElevatorStatus_Should_Display_Status()
        {
            var statuses = new List<ElevatorStatus>
            {
                new ElevatorStatus { Id = 1, CurrentFloor = 3, Direction = enElevatorDirection.Up, IsMoving = true, PassengerCount = 4 },
                new ElevatorStatus { Id = 2, CurrentFloor = 5, Direction = enElevatorDirection.Down, IsMoving = false, PassengerCount = 2 }
            };

            mockElevatorSystem.Setup(es => es.GetElevatorStatus()).Returns(statuses);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            consoleController.ShowElevatorStatus();

            // Assert
            var expectedOutput = "Elevator 1: Floor 3, Direction Up, Moving, Passengers 4\r\n" +
                                 "Elevator 2: Floor 5, Direction Down, Stationary, Passengers 2\r\n";
            Assert.Equal(expectedOutput, sw.ToString());
        }
    }

}
