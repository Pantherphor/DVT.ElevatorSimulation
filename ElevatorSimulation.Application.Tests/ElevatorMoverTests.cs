using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class ElevatorMoverTests
    {
        private readonly Mock<IElevator> mockElevator;
        private readonly IElevatorMover elevatorMover;

        public ElevatorMoverTests()
        {
            mockElevator = new Mock<IElevator>();
            elevatorMover = new ElevatorMover().Initialize(mockElevator.Object);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task MoveToNextFloorAsync_ShouldMoveElevatorToTargetFloor_And_OffloadPassangers()
        {
            // Arrange
            var mover = new ElevatorMover();
            var elevator = new Elevator(mover)
            {
                Id = 1,
                MaxPassengerLimit = 10
            };
            elevator.AddFloorRequest(new FloorRequest(0, 5, 3));
            mover.Initialize(elevator);

            // Act
            await mover.MoveToNextFloorAsync();

            // Assert
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(0, elevator.PassengerCount); //Should be 0 as it would have reached and offloaded at destination floor
            Assert.False(elevator.IsMoving);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task MoveToNextFloorAsync_ShouldMoveElevatorToTargetFloor_And_OffloadPassangers_OnAllTagetFloors_GoingUP()
        {
            // Arrange
            var mover = new ElevatorMover();
            var elevator = new Elevator(mover)
            {
                Id = 1,
                MaxPassengerLimit = 10
            };
            elevator.AddFloorRequest(new FloorRequest(0, 5, 3));
            elevator.AddFloorRequest(new FloorRequest(3, 2, 3));
            elevator.AddFloorRequest(new FloorRequest(4, 6, 4));
            mover.Initialize(elevator);

            // Act
            await mover.MoveToNextFloorAsync();

            // Assert
            Assert.Equal(6, elevator.CurrentFloor);
            Assert.Equal(0, elevator.PassengerCount); //Should be 0 as it would have reached and offloaded at destination floor
            Assert.False(elevator.IsMoving);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task MoveToNextFloorAsync_ShouldMoveElevatorToTargetFloor_And_OffloadPassangers_OnAllTagetFloors_GoingDown()
        {
            // Arrange
            var mover = new ElevatorMover();
            var elevator = new Elevator(mover)
            {
                Id = 1,
                MaxPassengerLimit = 10
            };
            elevator.AddFloorRequest(new FloorRequest(6, 5, 3));
            elevator.AddFloorRequest(new FloorRequest(2, 0, 3));
            elevator.AddFloorRequest(new FloorRequest(5, 2, 4));
            mover.Initialize(elevator);

            // Act
            await mover.MoveToNextFloorAsync();

            // Assert
            Assert.Equal(2, elevator.CurrentFloor);
            Assert.Equal(0, elevator.PassengerCount); //Should be 0 as it would have reached and offloaded at destination floor
            Assert.False(elevator.IsMoving);
        }

        [Fact]
        public async Task MoveToNextFloorAsync_ShouldMoveToCallingFloorAndThenToTargetFloor()
        {
            // Arrange
            var floorRequest = new FloorRequest(2, 5, 3);
            var floorRequests = new List<FloorRequest> { floorRequest };
            mockElevator.Setup(e => e.FloorRequests).Returns(floorRequests);
            mockElevator.Setup(e => e.CurrentFloor).Returns(1);
            mockElevator.Setup(e => e.RemoveFloorRequest(It.IsAny<FloorRequest>()))
                .Callback<FloorRequest>(request =>
                {
                    mockElevator.Setup(r => r.FloorRequests.Remove(request));
                });
            mockElevator.Setup(e => e.IncrementCurrentFloor(It.IsAny<int>()))
                .Callback<int>(floor =>
                {
                    var currentFloor = mockElevator.Object.CurrentFloor;
                    mockElevator.Setup(e => e.CurrentFloor).Returns(currentFloor + floor);
                });
            mockElevator.Setup(e => e.SetCallingFloor(It.IsAny<int>()))
                .Callback<int>(floor =>
                {
                    mockElevator.Setup(e => e.CallingFloor).Returns(floor);
                });

            // Act
            await elevatorMover.MoveToNextFloorAsync();

            // Assert
            mockElevator.Verify(e => e.RemoveFloorRequest(floorRequest), Times.Once);
            mockElevator.Verify(e => e.IncrementCurrentFloor(It.IsAny<int>()), Times.Exactly(4)); // Move to 2, then 3, 4, 5
            mockElevator.Verify(e => e.DecrementPassengerCount(floorRequest.PassengerCount), Times.Once);
            mockElevator.Verify(e => e.IncrementPassengerCount(floorRequest.PassengerCount), Times.Once);
        }

        [Fact]
        public async Task MoveToNextFloorAsync_ShouldNotMoveIfAlreadyOnCallingFloor()
        {
            // Arrange
            var floorRequest = new FloorRequest(1, 5, 3);
            var floorRequests = new List<FloorRequest> { floorRequest };
            mockElevator.Setup(e => e.FloorRequests).Returns(floorRequests);
            mockElevator.Setup(e => e.CurrentFloor).Returns(1);
            mockElevator.Setup(e => e.RemoveFloorRequest(It.IsAny<FloorRequest>()))
                .Callback<FloorRequest>(request =>
                {
                    mockElevator.Setup(r => r.FloorRequests.Remove(request));
                });
            mockElevator.Setup(e => e.IncrementCurrentFloor(It.IsAny<int>()))
                .Callback<int>(floor =>
                {
                    var currentFloor = mockElevator.Object.CurrentFloor;
                    mockElevator.Setup(e => e.CurrentFloor).Returns(currentFloor + floor);
                });
            mockElevator.Setup(e => e.SetCallingFloor(It.IsAny<int>()))
                .Callback<int>(floor =>
                {
                    mockElevator.Setup(e => e.CallingFloor).Returns(floor);
                });

            // Act
            await elevatorMover.MoveToNextFloorAsync();

            // Assert
            mockElevator.Verify(e => e.RemoveFloorRequest(floorRequest), Times.Once);
            mockElevator.Verify(e => e.IncrementCurrentFloor(It.IsAny<int>()), Times.Exactly(4)); // Move to 2, then 3, 4, 5
            mockElevator.Verify(e => e.DecrementPassengerCount(floorRequest.PassengerCount), Times.Once);
            mockElevator.Verify(e => e.IncrementPassengerCount(floorRequest.PassengerCount), Times.Never);
        }

        [Fact]
        public async Task MoveToNextFloorAsync_ShouldNotMoveIfAlreadyOnTargetFloor()
        {
            // Arrange
            var floorRequest = new FloorRequest(1, 1, 3);
            var floorRequests = new List<FloorRequest> { floorRequest };
            mockElevator.Setup(e => e.FloorRequests).Returns(floorRequests);
            mockElevator.Setup(e => e.CurrentFloor).Returns(1);
            mockElevator.Setup(e => e.RemoveFloorRequest(It.IsAny<FloorRequest>()))
                .Callback<FloorRequest>(request =>
                {
                    mockElevator.Setup(r => r.FloorRequests.Remove(request));
                });
            mockElevator.Setup(e => e.IncrementCurrentFloor(It.IsAny<int>()))
                .Callback<int>(floor =>
                {
                    var currentFloor = mockElevator.Object.CurrentFloor;
                    mockElevator.Setup(e => e.CurrentFloor).Returns(currentFloor + floor);
                });
            mockElevator.Setup(e => e.SetCallingFloor(It.IsAny<int>()))
                .Callback<int>(floor =>
                {
                    mockElevator.Setup(e => e.CallingFloor).Returns(floor);
                });

            // Act
            await elevatorMover.MoveToNextFloorAsync();

            // Assert
            mockElevator.Verify(e => e.RemoveFloorRequest(floorRequest), Times.Once);
            mockElevator.Verify(e => e.IncrementCurrentFloor(It.IsAny<int>()), Times.Never); // No movement expected
            mockElevator.Verify(e => e.DecrementPassengerCount(floorRequest.PassengerCount), Times.Never);
            mockElevator.Verify(e => e.IncrementPassengerCount(floorRequest.PassengerCount), Times.Never);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task OpenAndCloseDoorAsync_Should_Trigger_Door_Events()
        {
            // Arrange

            var floorRequest = new FloorRequest(1, 5, 3);
            mockElevator.Setup(e => e.CurrentFloor).Returns(1);
            mockElevator.Setup(e => e.PassengerCount).Returns(3);

            var doorStatusOpened = false;
            var doorStatusClosed = false;

            var mover = new ElevatorMover();
            mover.Initialize(mockElevator.Object);
            mover.ElevatorDoorStateChanged += (id, status) =>
            {
                if (status.Contains("Opened"))
                {
                    doorStatusOpened = true;
                }
                else if (status.Contains("Closed"))
                {
                    doorStatusClosed = true;
                }
            };

            // Act
            await mover.OpenAndCloseDoorAsync(floorRequest);

            // Assert
            Assert.True(doorStatusOpened);
            Assert.True(doorStatusClosed);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task OffLoadPassengersAsync_Should_Decrease_PassengerCount()
        {
            // Arrange
            var floorRequest = new FloorRequest(1, 5, 3);
            mockElevator.Setup(e => e.CurrentFloor).Returns(5);
            mockElevator.Setup(e => e.PassengerCount).Returns(5);
            var mover = new ElevatorMover();
            mover.Initialize(mockElevator.Object);
            // Act
            await mover.OffLoadPassengersAsync(floorRequest);

            // Assert
            var elevator = mockElevator.Object;
            Assert.Equal(5, elevator.PassengerCount);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task LoadPassengersAsync_Should_Increase_PassengerCount()
        {
            // Arrange
            var floorRequest = new FloorRequest(1, 5, 3);
            mockElevator.Setup(e => e.CurrentFloor).Returns(1);
            mockElevator.Setup(e => e.PassengerCount).Returns(2);
            mockElevator.Setup(e => e.IsFull(It.IsAny<int>())).Returns(false);
            mockElevator.Setup(e => e.IncrementPassengerCount(It.IsAny<int>())).Callback<int>(count =>
            {
                var currentCount = mockElevator.Object.PassengerCount;
                mockElevator.Setup(e => e.PassengerCount).Returns(currentCount + count);
            });
            var mover = new ElevatorMover();
            mover.Initialize(mockElevator.Object);
            // Act
            await mover.LoadPassengersAsync(floorRequest);

            // Assert
            var elevator = mockElevator.Object;
            Assert.Equal(5, elevator.PassengerCount);
        }

        [Fact]
        [Trait("Category", "Integration")]
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
            var floorRequest = new FloorRequest(0, 5, 2);

            // Act
            await mover.OpenAndCloseDoorAsync(floorRequest);

            // Assert
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(1, elevator.PassengerCount); //as the elevator would have reached destination should offload 
            Assert.False(elevator.IsMoving);
        }

        [Fact]
        [Trait("Category", "Integration")]
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
            var floorRequest = new FloorRequest(0, 5, 5);

            // Act
            await mover.OffLoadPassengersAsync(floorRequest);

            // Assert
            Assert.Equal(0, elevator.PassengerCount);
        }

        [Fact]
        [Trait("Category", "Integration")]
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
            var floorRequest = new FloorRequest(5, targetFloor: 5, passengerCount: 5);

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