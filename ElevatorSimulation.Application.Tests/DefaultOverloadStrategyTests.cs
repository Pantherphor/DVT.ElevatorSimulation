using ElevatorSimulation.Application.Strategies;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class DefaultOverloadStrategyTests
    {
        [Fact]
        public async void HandleOverload_Should_DistributePassengers_To_OtherElevators()
        {
            // Arrange
            var mockElevatorSystem = new Mock<IElevatorSystem>();
            var mockElevatorMover = new Mock<IElevatorMover>();
            var elevators = new List<IElevator>
            {
                new Elevator(mockElevatorMover.Object) { Id = 1, CurrentFloor = 0, PassengerCount = 8, MaxPassengerLimit = 10 },
                new Elevator(mockElevatorMover.Object) { Id = 2, CurrentFloor = 0, PassengerCount = 2, MaxPassengerLimit = 10 }
            };

            mockElevatorSystem.Setup(es => es.Elevators).Returns(elevators);

            var strategy = new DefaultOverloadStrategy();
            var nearestElevator = elevators[0];
            int targetFloor = 1;
            int callingFloor = 0;
            int excessPassengers = 5;

            // Act
            await strategy.HandleOverloadAsync(mockElevatorSystem.Object, nearestElevator, callingFloor, targetFloor, excessPassengers);

            // Assert
            Assert.Equal(8, nearestElevator.PassengerCount);
            Assert.Equal(2, elevators[1].PassengerCount); // elevator 2, should now have 2 passangers as set up above 
            Assert.Contains(targetFloor, elevators[1].FloorRequests.Select(o => o.TargetFloor));
        }

        [Fact]
        public async void HandleOverload_Should_Not_Overload_Secondary_Elevators()
        {
            // Arrange
            var mockElevatorSystem = new Mock<IElevatorSystem>();
            var mockElevatorMover = new Mock<IElevatorMover>();

            var mockElevatorMover1 = new Mock<IElevatorMover>();
            var mockElevatorMover2 = new Mock<IElevatorMover>();
            var mockElevators = new List<Mock<IElevator>>();
            var mockElevator1 = new Mock<IElevator>();
            var elevator1Requests = new List<FloorRequest>();
            var elevator2Requests = new List<FloorRequest>();
            
            mockElevator1 = new Mock<IElevator>();
            mockElevator1.SetupProperty(e => e.Direction, enElevatorDirection.None);
            mockElevator1.Setup(e => e.PassengerCount).Returns(10);
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

            var mockElevator2 = new Mock<IElevator>();
            mockElevator2.Setup(e => e.PassengerCount).Returns(10);
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

            mockElevatorSystem.Setup(es => es.Elevators).Returns(mockElevators.Select(o => o.Object));

            var strategy = new DefaultOverloadStrategy();
            var nearestElevator = mockElevator1.Object;
            var secodaryElevator = mockElevator2.Object;
            int targetFloor = 1;
            int callingFloor = 0;
            int excessPassengers = 5;

            // Act
            await strategy.HandleOverloadAsync(mockElevatorSystem.Object, nearestElevator, callingFloor, targetFloor, excessPassengers);

            // Assert
            Assert.Equal(10, nearestElevator.PassengerCount); // Nearest elevator remains at max capacity
            Assert.Equal(10, secodaryElevator.PassengerCount); // Secondary elevator remains at max capacity
            Assert.Equal(1, nearestElevator.FloorRequests.Count); //With the new changes, the elevators should shad the load, by moving to the next available lift
            Assert.Contains(5, nearestElevator.FloorRequests.Select(o => o.PassengerCount)); // Excess passengers remain
            Assert.Contains(targetFloor, nearestElevator.FloorRequests.Select(o => o.TargetFloor)); // Floor request should be added to nearest elevator queue
        }
    }
}
