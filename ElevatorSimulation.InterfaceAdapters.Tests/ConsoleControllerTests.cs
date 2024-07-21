using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ElevatorSimulation.InterfaceAdapters.Tests
{
    public class ConsoleControllerTests : IDisposable
    {
        private readonly Mock<IElevatorControlUseCase> mockElevatorControlUseCase;
        private readonly ConsoleController controller;
        private readonly TextReader originalInput;
        private readonly TextWriter originalOutput;

        public ConsoleControllerTests()
        {
            mockElevatorControlUseCase = new Mock<IElevatorControlUseCase>();
            var history = new Dictionary<int, IList<ElevatorMovementHistory>>
            {

            };

            mockElevatorControlUseCase.Setup(e => e.GetElevatorMovementHistory()).Returns(history);
            controller = new ConsoleController(mockElevatorControlUseCase.Object);
            originalInput = Console.In;
            originalOutput = Console.Out;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void CallElevator_Should_Call_ElevatorControlUseCase_With_Correct_Parameters()
        {
            // Arrange
            var input = "1\n5\n3\n";
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            // Act
            controller.CallElevator();

            // Assert
            mockElevatorControlUseCase.Verify(e => e.CallElevatorAsync(It.Is<FloorRequest>(r => r.CallingFloor == 1 && r.TargetFloor == 5 && r.PassengerCount == 3)), Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void MoveElevator_Should_Move_ElevatorControlUseCase_With_Correct_Parameters()
        {
            // Arrange
            var input = "1\n5\n";
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            // Act
            controller.MoveElevator();

            // Assert
            mockElevatorControlUseCase.Verify(e => e.MoveElevatorAsync(1, 5), Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ShowElevatorStatus_Should_Display_Correct_Status()
        {
            // Arrange
            var statuses = new List<ElevatorStatus>
            {
                new ElevatorStatus { Id = 1, CurrentFloor = 3, Direction = enElevatorDirection.Up, IsMoving = true, PassengerCount = 2 }
            };

            var history = new Dictionary<int, IList<ElevatorMovementHistory>>
            {
                
            };

            mockElevatorControlUseCase.Setup(e => e.GetElevatorStatus()).Returns(statuses);
            mockElevatorControlUseCase.Setup(e => e.GetElevatorMovementHistory()).Returns(history);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            controller.ShowElevatorStatus();

            // Assert
            var output = stringWriter.ToString().Trim();
            Assert.Contains("Elevator 1: Floor 3, Direction Up, Moving, Passengers 2", output);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Run_Should_Handle_Invalid_Option()
        {
            // Arrange
            var input = "invalid\nq\n";
            var stringReader = new StringReader(input);
            var stringWriter = new StringWriter();

            Console.SetIn(stringReader);
            Console.SetOut(stringWriter);

            // Act
            controller.Run();

            // Assert
            var output = stringWriter.ToString();
            Assert.Contains("Invalid option. Please try again.", output);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Run_Should_Call_Elevator()
        {
            // Arrange
            var input = "1\n1\n5\n3\nq\n";
            var stringReader = new StringReader(input);
            var stringWriter = new StringWriter();

            Console.SetIn(stringReader);
            Console.SetOut(stringWriter);

            // Act
            controller.Run();

            // Assert
            mockElevatorControlUseCase.Verify(e => e.CallElevatorAsync(It.IsAny<FloorRequest>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Run_Should_Move_Elevator()
        {
            // Arrange
            var input = "2\n1\n5\nq\n";
            var stringReader = new StringReader(input);
            var stringWriter = new StringWriter();

            Console.SetIn(stringReader);
            Console.SetOut(stringWriter);

            // Act
            controller.Run();

            // Assert
            mockElevatorControlUseCase.Verify(e => e.MoveElevatorAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Run_Should_Show_Elevator_Status()
        {
            // Arrange
            var input = "3\nq\n";
            var stringReader = new StringReader(input);
            var stringWriter = new StringWriter();

            Console.SetIn(stringReader);
            Console.SetOut(stringWriter);


            // Act
            controller.Run();

            // Assert
            mockElevatorControlUseCase.Verify(e => e.GetElevatorStatus(), Times.Once);
        }

        public void Dispose()
        {
            // Cleanup
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }

    }
}
