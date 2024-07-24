using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace ElevatorSimulation.InterfaceAdapters.Tests
{
    public class ConsoleControllerIntergrationTests : IDisposable
    {
        private readonly Dictionary<int, IList<ElevatorMovementHistory>> elevatorHistory = new();

        private readonly Mock<IElevatorControlUseCase> mockElevatorControlUseCase;
        private readonly ConsoleController consoleController;
        private readonly TextReader originalInput;
        private readonly TextWriter originalOutput;

        public ConsoleControllerIntergrationTests()
        {
            mockElevatorControlUseCase = new Mock<IElevatorControlUseCase>();
            consoleController = new ConsoleController(mockElevatorControlUseCase.Object);
            originalInput = Console.In;
            originalOutput = Console.Out;
        }

        [Fact]
        [Trait("Category", "Integration")]
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
        [Trait("Category", "Integration")]
        public void CallElevator_Should_CallElevator_With_Valid_Input()
        {
            // Arrange
            var input = new StringReader("1\n0\n3\n2\nq\n");
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            // Act
            consoleController.Run();

            // Assert
            mockElevatorControlUseCase.Verify(es => es.CallElevatorAsync(It.IsAny<FloorRequest>()), Times.Once);

            // Cleanup
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void CallElevator_Should_Show_Error_On_Invalid_Floor_Input()
        {
            using var sr = new StringReader("1\ninvalid\ninvalid\n");
            using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            // Act
            consoleController.CallElevator();

            // Assert
            Assert.Contains("Invalid floor number.", sw.ToString());
            mockElevatorControlUseCase.Verify(es => es.CallElevatorAsync(new FloorRequest(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())), Times.Never);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void CallElevator_Should_Show_Error_On_Invalid_Passenger_Input()
        {
            using var sr = new StringReader("0\n5\ninvalid");
            using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            // Act
            consoleController.CallElevator();

            // Assert
            Assert.Contains("Invalid number of passengers.", sw.ToString());
            mockElevatorControlUseCase.Verify(es => es.CallElevatorAsync(new FloorRequest(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())), Times.Never);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void MoveElevator_Should_MoveElevator_With_Valid_Input()
        {
            using var sr = new StringReader("1\r\n5\n5");
            Console.SetIn(sr);

            // Act
            consoleController.MoveElevator();

            // Assert
            mockElevatorControlUseCase.Verify(es => es.MoveElevatorAsync(1, 5), Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
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
            mockElevatorControlUseCase.Verify(es => es.MoveElevatorAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Integration")]
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
            mockElevatorControlUseCase.Verify(es => es.MoveElevatorAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ShowElevatorStatus_Should_Display_Status()
        {
            var statuses = new List<ElevatorStatus>
            {
                new ElevatorStatus { Id = 1, CurrentFloor = 3, Direction = enElevatorDirection.Up, IsMoving = true, PassengerCount = 4 },
                new ElevatorStatus { Id = 2, CurrentFloor = 5, Direction = enElevatorDirection.Down, IsMoving = false, PassengerCount = 2 }
            };

            elevatorHistory[1] = new List<ElevatorMovementHistory>();

            var elevator = statuses[1];
            elevatorHistory[1].Add(new ElevatorMovementHistory
            {
                ElevatorId = elevator.Id,
                CallingFloor = elevator.CallingFloor,
                CurrentFloor = elevator.CurrentFloor,
                TargetFloor = elevator.TargetFloor,
                Direction = elevator.Direction,
                PassengerCount = elevator.PassengerCount,
                IsMoving = elevator.IsMoving,
                Timestamp = DateTime.Now
            });

            mockElevatorControlUseCase.Setup(e => e.GetElevatorStatus()).Returns(statuses);
            mockElevatorControlUseCase.Setup(e => e.GetElevatorMovementHistory()).Returns(elevatorHistory);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            consoleController.ShowElevatorStatus();

            // Assert
            var expectedHeaderColsOutput = "| Elevator | Calling Floor | Current Floor | Target Floor | Direction | Passengers | Moving | Door Status | Timestamp |";
            var expectedSummaryOutput = "Elevator 1: Floor 3, Direction Up, Closed, Passengers Moving\r\n" +
                                        "Elevator 2: Floor 5, Direction Down, Closed, Passengers Stationary\r\n";
            Assert.Contains(expectedHeaderColsOutput, sw.ToString());
            Assert.Contains(expectedSummaryOutput, sw.ToString());
        }

        public void Dispose()
        {
            // Cleanup
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }

    }

}
