using ElevatorSimulation.Application.Strategies;
using ElevatorSimulation.Domain.Entities;
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
        public void HandleOverload_Should_DistributePassengers_To_OtherElevators()
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
            strategy.HandleOverload(mockElevatorSystem.Object, nearestElevator, callingFloor, targetFloor, excessPassengers);

            // Assert
            Assert.Equal(8, nearestElevator.PassengerCount);
            Assert.Equal(2, elevators[1].PassengerCount); // elevator 2, should now have 2 passangers as set up above 
            Assert.Contains(targetFloor, elevators[1].FloorRequests.Select(o => o.TargetFloor));
        }

        [Fact]
        public void HandleOverload_Should_Not_Overload_Secondary_Elevators()
        {
            // Arrange
            var mockElevatorSystem = new Mock<IElevatorSystem>();
            var mockElevatorMover = new Mock<IElevatorMover>();
            var elevators = new List<IElevator>
            {
                new Elevator(mockElevatorMover.Object) { Id = 1, CurrentFloor = 0, PassengerCount = 10, MaxPassengerLimit = 10 },
                new Elevator(mockElevatorMover.Object) { Id = 2, CurrentFloor = 0, PassengerCount = 10, MaxPassengerLimit = 10 }
            };

            mockElevatorSystem.Setup(es => es.Elevators).Returns(elevators);

            var strategy = new DefaultOverloadStrategy();
            var nearestElevator = elevators[0];
            int targetFloor = 1;
            int callingFloor = 0;
            int excessPassengers = 5;

            // Act
            strategy.HandleOverload(mockElevatorSystem.Object, nearestElevator, callingFloor, targetFloor, excessPassengers);

            // Assert
            Assert.Equal(10, nearestElevator.PassengerCount); // Nearest elevator remains at max capacity
            Assert.Equal(10, elevators[1].PassengerCount); // Secondary elevator remains at max capacity
            Assert.Equal(1, nearestElevator.FloorRequests.Count); //With the new changes, the elevators should shad the load, by moving to the next available lift
            Assert.Contains(5, nearestElevator.FloorRequests.Select(o => o.PassengerCount)); // Excess passengers remain
            Assert.Contains(targetFloor, nearestElevator.FloorRequests.Select(o => o.TargetFloor)); // Floor request should be added to nearest elevator queue
        }
    }
}
