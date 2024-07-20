using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class ElevatorControlUseCaseTests
    {
        private readonly Mock<IElevatorSystem> mockElevatorSystem;
        private readonly Mock<Building> mockBuilding;
        private readonly ElevatorControlUseCase elevatorControlUseCase;

        public ElevatorControlUseCaseTests()
        {
            mockElevatorSystem = new Mock<IElevatorSystem>();
            mockBuilding = new Mock<Building>(mockElevatorSystem.Object);

            elevatorControlUseCase = new ElevatorControlUseCase(mockBuilding.Object);
        }

        [Fact]
        public void CallElevator_Should_Invoke_ElevatorSystem_CallElevator()
        {
            // Arrange
            var request = new FloorRequest(0, 1, 5);

            // Act
            elevatorControlUseCase.CallElevator(request);

            // Assert
            mockElevatorSystem.Verify(es => es.CallElevator(request), Times.Once);
        }

        [Fact]
        public void MoveElevator_Should_Invoke_ElevatorSystem_MoveElevator()
        {
            // Arrange
            int elevatorId = 1;
            int floor = 5;

            // Act
            elevatorControlUseCase.MoveElevator(elevatorId, floor);

            // Assert
            mockElevatorSystem.Verify(es => es.MoveElevator(elevatorId, floor), Times.Once);
        }

        [Fact]
        public void GetElevatorStatus_Should_Return_ElevatorSystem_GetElevatorStatus()
        {
            // Arrange
            var elevatorStatuses = new List<ElevatorStatus>
            {
                new ElevatorStatus { Id = 1, CurrentFloor = 3, Direction = enElevatorDirection.Up, IsMoving = true, PassengerCount = 4 },
                new ElevatorStatus { Id = 2, CurrentFloor = 5, Direction = enElevatorDirection.Down, IsMoving = false, PassengerCount = 2 }
            };
            mockElevatorSystem.Setup(es => es.GetElevatorStatus()).Returns(elevatorStatuses);

            // Act
            var result = elevatorControlUseCase.GetElevatorStatus();

            // Assert
            Assert.Equal(elevatorStatuses, result);
            mockElevatorSystem.Verify(es => es.GetElevatorStatus(), Times.Once);
        }
    }
}
