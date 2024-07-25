using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Enums;
using Moq;
using Xunit;
using ElevatorSimulation.InterfaceAdapters.Interfaces;
using System.Collections.Generic;
using System;

namespace ElevatorSimulation.InterfaceAdapters.Tests
{
    public class MockConsole : IConsole
    {
        private readonly Queue<string> inputQueue = new();
        private readonly List<string> outputList = new();

        public void EnqueueInput(string input) => inputQueue.Enqueue(input);
        public string GetOutput() => string.Join(Environment.NewLine, outputList);

        public void WriteLine(string message) => outputList.Add(message);
        public void Write(string message) => outputList.Add(message);
        public string ReadLine() => inputQueue.Count > 0 ? inputQueue.Dequeue() : string.Empty;
    }

    public class ConsoleControllerTests
    {
        private readonly Mock<IElevatorControlUseCase> mockElevatorControlUseCase;
        private readonly MockConsole mockConsole;
        private readonly ConsoleController consoleController;

        public ConsoleControllerTests()
        {
            mockConsole = new MockConsole();
            mockElevatorControlUseCase = new Mock<IElevatorControlUseCase>();
            consoleController = new ConsoleController(mockElevatorControlUseCase.Object, mockConsole);
        }

        [Fact]
        public void DisplayMenu_Should_Show_Options()
        {
            // Act
            consoleController.DisplayMenu();

            // Assert
            var output = mockConsole.GetOutput();
            Assert.Contains("Select an option:", output);
            Assert.Contains("1. Call Elevator", output);
            Assert.Contains("2. Move Elevator", output);
            Assert.Contains("3. Show Elevator Status", output);
            Assert.Contains("q. Quit", output);
        }

        [Fact]
        public void CallElevator_Should_CallElevator_With_Valid_Input()
        {
            // Arrange
            mockConsole.EnqueueInput("1");
            mockConsole.EnqueueInput("0");
            mockConsole.EnqueueInput("3");
            mockConsole.EnqueueInput("2");
            mockConsole.EnqueueInput("q");

            // Act
            consoleController.Run();

            // Assert
            mockElevatorControlUseCase.Verify(es => es.CallElevatorAsync(It.IsAny<FloorRequest>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void CallElevator_Should_Show_Error_On_Invalid_Floor_Input()
        {
            // Arrange
            mockConsole.EnqueueInput("1");
            mockConsole.EnqueueInput("invalid");
            mockConsole.EnqueueInput("invalid");

            // Act
            consoleController.CallElevator();

            // Assert
            var output = mockConsole.GetOutput();
            Assert.Contains("Invalid floor number.", output);
            mockElevatorControlUseCase.Verify(es => es.CallElevatorAsync(It.IsAny<FloorRequest>()), Times.Never);
        }

        [Fact]
        public void CallElevator_Should_Show_Error_On_Invalid_Passenger_Input()
        {
            // Arrange
            mockConsole.EnqueueInput("1");
            mockConsole.EnqueueInput("0");
            mockConsole.EnqueueInput("invalid");

            // Act
            consoleController.CallElevator();

            // Assert
            var output = mockConsole.GetOutput();
            Assert.Contains("Invalid number of passengers.", output);
            mockElevatorControlUseCase.Verify(es => es.CallElevatorAsync(It.IsAny<FloorRequest>()), Times.Never);
        }

        [Fact]
        public void MoveElevator_Should_MoveElevator_With_Valid_Input()
        {
            // Arrange
            mockConsole.EnqueueInput("1");
            mockConsole.EnqueueInput("5");

            // Act
            consoleController.MoveElevator();

            // Assert
            mockElevatorControlUseCase.Verify(es => es.MoveElevatorAsync(1, 5), Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void MoveElevator_Should_Show_Error_On_Invalid_ElevatorId_Input()
        {
            // Arrange
            mockConsole.EnqueueInput("invalid");

            // Act
            consoleController.MoveElevator();

            // Assert
            var output = mockConsole.GetOutput();
            Assert.Contains("Invalid elevator ID.", output);
            mockElevatorControlUseCase.Verify(es => es.MoveElevatorAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void MoveElevator_Should_Show_Error_On_Invalid_Floor_Input()
        {
            // Arrange
            mockConsole.EnqueueInput("1");
            mockConsole.EnqueueInput("invalid");

            // Act
            consoleController.MoveElevator();

            // Assert
            var output = mockConsole.GetOutput();
            Assert.Contains("Invalid floor number.", output);
            mockElevatorControlUseCase.Verify(es => es.MoveElevatorAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void ShowElevatorStatus_Should_Display_Status()
        {
            // Arrange
            var statuses = new List<ElevatorStatus>
            {
                new ElevatorStatus { Id = 1, CurrentFloor = 3, Direction = enElevatorDirection.Up, IsMoving = true, PassengerCount = 4 },
                new ElevatorStatus { Id = 2, CurrentFloor = 5, Direction = enElevatorDirection.Down, IsMoving = false, PassengerCount = 2 }
            };

            mockElevatorControlUseCase.Setup(e => e.GetElevatorStatus()).Returns(statuses);

            var movementHistory = new Dictionary<int, IList<ElevatorMovementHistory>>
            { 
            };

            mockElevatorControlUseCase.Setup(e => e.GetElevatorMovementHistory()).Returns(movementHistory);

            // Act
            consoleController.ShowElevatorStatus();

            // Assert
            var output = mockConsole.GetOutput();
            var expectedHeaderColsOutput = "| Elevator | Calling Floor | Current Floor | Target Floor | Direction | Passengers | Moving | Door Status | Timestamp |";
            Assert.Contains(expectedHeaderColsOutput, output);
            Assert.Contains("Elevator 1: Floor 3, Direction Up, Closed, Passengers Moving", output);
            Assert.Contains("Elevator 2: Floor 5, Direction Down, Closed, Passengers Stationary", output);
        }

    }
}
