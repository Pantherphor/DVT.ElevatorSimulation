using System;
using System.Collections.Generic;
using System.Linq;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Services;
using ElevatorSimulation.Services;
using Moq;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class ElevatorSystemTests
    {

        private readonly List<Mock<IElevator>> mockElevators;
        private readonly Mock<IOverloadStrategy> mockOverloadStrategy;
        private readonly ElevatorSystem elevatorSystem;
        private readonly Mock<IElevator> mockElevator1;
        private readonly Mock<IElevator> mockElevator2;
        private readonly Mock<IElevatorMover> mockElevatorMover1;
        private readonly Mock<IElevatorMover> mockElevatorMover2;
        private readonly List<FloorRequest> elevator1Requests;
        private readonly List<FloorRequest> elevator2Requests;

        public ElevatorSystemTests()
        {
            mockElevators = new List<Mock<IElevator>>();
            mockOverloadStrategy = new Mock<IOverloadStrategy>(); 
            
            mockElevatorMover1 = new Mock<IElevatorMover>();
            mockElevatorMover2 = new Mock<IElevatorMover>();

            elevator1Requests = new List<FloorRequest>();
            elevator2Requests = new List<FloorRequest>();

            mockElevator1 = new Mock<IElevator>();
            mockElevator1.SetupProperty(e => e.Direction, enElevatorDirection.None);
            mockElevator1.Setup(e => e.PassengerCount).Returns(0);
            mockElevator1.Setup(e => e.CurrentFloor).Returns(1);
            mockElevator1.Setup(e => e.FloorRequests).Returns(elevator1Requests);
            mockElevator1.Setup(e => e.ElevatorMover).Returns(mockElevatorMover1.Object);
            mockElevator1.Setup(e => e.IsFull(It.IsAny<int>())).Returns(false);
            mockElevator1.Setup(e => e.GetExcessPassangers(It.IsAny<int>())).Returns(0);
            mockElevator1.Setup(e => e.Id).Returns(1);
            mockElevator1.Setup(o => o.AddFloorRequest(It.IsAny<FloorRequest>()))
                .Callback<FloorRequest>(fr => elevator1Requests.Add(fr));
            mockElevator1.Setup(o => o.IsFull(2)).Returns(false);

            mockElevatorMover1.Setup(o => o.Initialize(It.IsAny<IElevator>())).Returns(mockElevatorMover1.Object);
            mockElevators.Add(mockElevator1);

            mockElevator2 = new Mock<IElevator>();
            mockElevator2.Setup(e => e.PassengerCount).Returns(0);
            mockElevator2.SetupProperty(e => e.Direction, enElevatorDirection.None);
            mockElevator2.Setup(e => e.CurrentFloor).Returns(3);
            mockElevator2.Setup(e => e.FloorRequests).Returns(elevator2Requests);
            mockElevator2.Setup(e => e.ElevatorMover).Returns(mockElevatorMover2.Object);
            mockElevator2.Setup(e => e.IsFull(It.IsAny<int>())).Returns(false);
            mockElevator2.Setup(e => e.GetExcessPassangers(It.IsAny<int>())).Returns(0);
            mockElevator2.Setup(e => e.Id).Returns(2);
            mockElevator2.Setup(o => o.AddFloorRequest(It.IsAny<FloorRequest>()))
                .Callback<FloorRequest>(fr => elevator2Requests.Add(fr));
            mockElevator2.Setup(o => o.IsFull(2)).Returns(false);

            mockElevatorMover2.Setup(o => o.Initialize(It.IsAny<IElevator>())).Returns(mockElevatorMover2.Object);
            mockElevators.Add(mockElevator2);


            var elevators = new List<IElevator> { mockElevator1.Object, mockElevator2.Object };

            elevatorSystem = new ElevatorSystem(elevators);
            elevatorSystem.SetOverloadStrategy(mockOverloadStrategy.Object);
        }

        [Fact]
        public void CallElevator_ShouldAssignRequestToNearestElevator()
        {
            // Arrange
            var request = new FloorRequest(0,5,2);
            

            // Act
            elevatorSystem.CallElevator(request);

            // Assert
            var nearestElevator = mockElevators
                                    .OrderBy(e => Math.Abs(e.Object.CurrentFloor - request.CallingFloor))  // Order by the distance to the calling floor
                                    .ThenBy(e => e.Object.PassengerCount)  // Then by the current passenger count
                                    .ThenBy(e => e.Object.Direction == enElevatorDirection.None ? 0 : 1)  // Prefer stationary elevators
                                    .ThenBy(e => (e.Object.Direction == enElevatorDirection.Up && request.CallingFloor >= e.Object.CurrentFloor) ||
                                                 (e.Object.Direction == enElevatorDirection.Down && request.CallingFloor <= e.Object.CurrentFloor) ? 0 : 1)
                                    .FirstOrDefault();
            Assert.Single(nearestElevator.Object.FloorRequests);
            Assert.Equal(request.TargetFloor, nearestElevator.Object.FloorRequests.First().TargetFloor);
            Assert.NotEqual(request.PassengerCount, nearestElevator.Object.PassengerCount); //No longer equal as this count should only be increased on callingFloor reached
        }

        [Fact]
        public void CallElevator_ShouldHandleOverload()
        {
            // Arrange
            var overloadStrategyMock = new Mock<IOverloadStrategy>();
            elevatorSystem.SetOverloadStrategy(overloadStrategyMock.Object);

            var request = new FloorRequest(0,5,10);
            mockElevators[0].Setup(e => e.IsFull(request.PassengerCount)).Returns(true);
            mockElevators[0].Setup(e => e.GetExcessPassangers(request.PassengerCount)).Returns(5);

            // Act
            elevatorSystem.CallElevator(request);

            // Assert
            var overloadedElevator = mockElevators[0].Object;
            overloadStrategyMock.Verify(s => s.HandleOverload(elevatorSystem, overloadedElevator, request.CallingFloor, request.TargetFloor, 5), Times.Once);
        }

        [Fact]
        public void MoveElevator_Should_Call_MoveToNextFloorAsync()
        {
            // Arrange
            int elevatorId = 1;
            int targetFloor = 5;

            // Act
            elevatorSystem.MoveElevator(elevatorId, targetFloor);

            // Assert
            var elevatorMock = mockElevators.First(e => e.Object.Id == elevatorId);
            elevatorMock.Verify(o => o.MoveToNextFloorAsync(), Times.Once);
        }

        [Fact]
        public void GetElevatorStatus_ShouldReturnCorrectStatus()
        {
            // Arrange
            var expectedStatus = new List<ElevatorStatus>
            {
                new ElevatorStatus { Id = 1, CurrentFloor = 1, PassengerCount = 0, Direction = enElevatorDirection.None },
                new ElevatorStatus { Id = 2, CurrentFloor = 3, PassengerCount = 0, Direction = enElevatorDirection.None }
            };

            foreach (var mockElevator in mockElevators)
            {
                mockElevator.Setup(e => e.GetElevatorStatus()).Returns(() =>
                    new ElevatorStatus
                    {
                        Id = mockElevator.Object.Id,
                        CurrentFloor = mockElevator.Object.CurrentFloor,
                        PassengerCount = mockElevator.Object.PassengerCount,
                        Direction = mockElevator.Object.Direction
                    });
            }

            // Act
            var statuses = elevatorSystem.GetElevatorStatus();

            // Assert
            Assert.Equal(2, statuses.Count());
            foreach (var status in statuses)
            {
                var expected = expectedStatus.First(s => s.Id == status.Id);
                Assert.Equal(expected.CurrentFloor, status.CurrentFloor);
                Assert.Equal(expected.PassengerCount, status.PassengerCount);
                Assert.Equal(expected.Direction, status.Direction);
            }
        }

        [Fact]
        public void CallElevator_Should_Assign_Request_To_Nearest_Elevator()
        {
            // Arrange
            var request = new FloorRequest(callingFloor: 2, targetFloor: 5, passengerCount: 3);

            // Act
            elevatorSystem.CallElevator(request);

            // Assert
            mockElevator1.Verify(e => e.AddFloorRequest(It.Is<FloorRequest>(fr => fr.TargetFloor == request.TargetFloor && fr.PassengerCount == request.PassengerCount)), Times.Once);
            Assert.NotEqual(3, mockElevator1.Object.PassengerCount); //No longer equal as this count should only be increased on callingFloor reached
        }
    }
}
