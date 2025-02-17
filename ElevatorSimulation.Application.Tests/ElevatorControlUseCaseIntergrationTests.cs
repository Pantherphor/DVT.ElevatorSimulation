using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class ElevatorControlUseCaseIntergrationTests
    {
        [Theory(Timeout = 100)]
        [InlineData(1, 5)]
        [InlineData(2, 3)]
        [InlineData(1, 10)]
        public async void AddTargetFloor_Should_Add_Floor_To_Elevator_Should_Not_Move_Elevator(int elevatorId, int targetFloor)
        {
            // Arrange
            var mockElevator = new Mock<IElevator>();
            mockElevator.Setup(e => e.Id).Returns(elevatorId);
            mockElevator.Setup(e => e.IsMoving).Returns(false);
            mockElevator.Setup(e => e.IncrementPassengerCount(It.IsAny<int>()))
                .Callback<int>(passengerCount =>
                {
                    var passangerCount = mockElevator.Object.PassengerCount + passengerCount;
                    mockElevator.Setup(e => e.PassengerCount).Returns(passangerCount);
                });
            var elevators = new List<IElevator> { mockElevator.Object };

            var mockElevatorSystem = new Mock<IElevatorSystem>();
            mockElevatorSystem.Setup(es => es.Elevators).Returns(elevators);

            var building = new Building(mockElevatorSystem.Object);
            var useCase = new ElevatorControlUseCase(building);

            // Act
            await useCase.CallElevatorAsync(new FloorRequest(0, targetFloor, 2));

            // Assert
            Assert.False(mockElevator.Object.IsMoving);
        }


        [Theory(Timeout = 66000)]
        [InlineData(0, 5)]
        [InlineData(2, 3)]
        [InlineData(1, 10)]
        public async void MoveElevatorsAsync_Should_Move_Elevators_To_Next_Floor(int elevatorId, int targetFloor)
        {
            // Arrange
            var mockElevator = new Mock<IElevator>();
            mockElevator.Setup(e => e.Id).Returns(elevatorId);
            mockElevator.Setup(e => e.CurrentFloor).Returns(targetFloor);
            mockElevator.Setup(e => e.IsMoving).Returns(false);
            mockElevator.Setup(e => e.MoveToNextFloorAsync()).Returns(Task.CompletedTask);
            var elevators = new List<IElevator> { mockElevator.Object };

            var mockElevatorSystem = new Mock<IElevatorSystem>();
            mockElevatorSystem.Setup(es => es.Elevators).Returns(elevators);

            var building = new Building(mockElevatorSystem.Object);
            var useCase = new ElevatorControlUseCase(building);

            // Act
            await useCase.CallElevatorAsync(new FloorRequest(0, targetFloor, 2));
            await useCase.MoveElevatorAsync(elevatorId, targetFloor);


            // Assert
            var currentElevator = mockElevator.Object;
            Assert.Equal(elevatorId, currentElevator.Id);
            Assert.Equal(currentElevator.CurrentFloor, targetFloor);
        }
    }

}
