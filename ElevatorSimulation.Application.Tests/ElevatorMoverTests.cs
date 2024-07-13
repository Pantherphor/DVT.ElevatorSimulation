using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class ElevatorMoverTests
    {
        [Fact]
        public async Task MoveToNextFloorAsync_ShouldMoveElevatorToTargetFloor_And_OffloadPassangers()
        {
            // Arrange
            var mover = new ElevatorMover();
            var elevator = new Elevator(mover)
            {
                Id = 1,
                CurrentFloor = 0,
                PassengerCount = 8,
                MaxPassengerLimit = 10
            };
            elevator.AddFloorRequest(new FloorRequest(5, 3));
            mover.Initialize(elevator);

            // Act
            await mover.MoveToNextFloorAsync();

            // Assert
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(3, elevator.PassengerCount);
            Assert.False(elevator.IsMoving);
        }

        [Fact]
        public async Task OpenAndCloseDoorAsync_Should_OpenAndCloseDoorAsync()
        {
            // Arrange
            var mover = new ElevatorMover();
            var elevator = new Elevator(mover)
            {
                Id = 1,
                CurrentFloor = 5,
                PassengerCount = 3,
                MaxPassengerLimit = 10
            };
            var floorRequest = new FloorRequest(5, 2);

            // Act
            await mover.OpenAndCloseDoorAsync(floorRequest);

            // Assert
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(3, elevator.PassengerCount);
            Assert.False(elevator.IsMoving);
        }

        [Fact]
        public async Task OffLoadPassengersAsync_ShouldNotAllowNegativePassengerCount()
        {
            // Arrange
            var mover = new ElevatorMover();
            var elevator = new Elevator(mover)
            {
                Id = 1,
                CurrentFloor = 5,
                PassengerCount = 3,
                MaxPassengerLimit = 10
            };
            var floorRequest = new FloorRequest(5, 5);

            // Act
            await mover.OffLoadPassengersAsync(floorRequest);

            // Assert
            Assert.Equal(0, elevator.PassengerCount);
        }

        [Fact]
        public async Task LoadPassengersAsync_ShouldNotExceedMaxPassengerLimit()
        {
            // Arrange
            var mover = new ElevatorMover();
            var elevator = new Elevator(mover)
            {
                Id = 1,
                CurrentFloor = 5,
                PassengerCount = 8,
                MaxPassengerLimit = 10
            };
            var floorRequest = new FloorRequest(targetFloor: 5, passengerCount: 5);

            // Mock the event handler
            var mockEventHandler = new Mock<Action<int, int, int, int>>();
            mover.ElevatorPassangerCountChanged += mockEventHandler.Object;


            // Act
            await mover.LoadPassengersAsync(floorRequest);

            // Assert
            mockEventHandler.Verify(o => o(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}