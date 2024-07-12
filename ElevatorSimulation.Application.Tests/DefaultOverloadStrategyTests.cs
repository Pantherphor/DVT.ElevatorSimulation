using ElevatorSimulation.Application.Strategies;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
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
            var elevators = new List<IElevator>
            {
                new Elevator { Id = 1, CurrentFloor = 0, PassengerCount = 8, MaxPassengerLimit = 10 },
                new Elevator { Id = 2, CurrentFloor = 0, PassengerCount = 2, MaxPassengerLimit = 10 }
            };

            mockElevatorSystem.Setup(es => es.Elevators).Returns(elevators);

            var strategy = new DefaultOverloadStrategy();
            var nearestElevator = elevators[0];
            int floor = 1;
            int excessPassengers = 5;

            // Act
            strategy.HandleOverload(mockElevatorSystem.Object, nearestElevator, floor, excessPassengers);

            // Assert
            Assert.Equal(8, nearestElevator.PassengerCount);
            Assert.Equal(7, elevators[1].PassengerCount); // 2 + 5
            Assert.Contains(floor, elevators[1].FloorRequests.Select(o => o.TargetFloor));
        }

        [Fact]
        public void HandleOverload_Should_Not_Overload_Secondary_Elevators()
        {
            // Arrange
            var mockElevatorSystem = new Mock<IElevatorSystem>();
            var elevators = new List<IElevator>
            {
                new Elevator { Id = 1, CurrentFloor = 0, PassengerCount = 10, MaxPassengerLimit = 10 },
                new Elevator { Id = 2, CurrentFloor = 0, PassengerCount = 10, MaxPassengerLimit = 10 }
            };

            mockElevatorSystem.Setup(es => es.Elevators).Returns(elevators);

            var strategy = new DefaultOverloadStrategy();
            var nearestElevator = elevators[0];
            int floor = 1;
            int excessPassengers = 5;

            // Act
            strategy.HandleOverload(mockElevatorSystem.Object, nearestElevator, floor, excessPassengers);

            // Assert
            Assert.Equal(10, nearestElevator.PassengerCount); // Nearest elevator remains at max capacity
            Assert.Equal(10, elevators[1].PassengerCount); // Secondary elevator remains at max capacity
            Assert.Equal(2, nearestElevator.FloorRequests.Count);
            Assert.Contains(5, nearestElevator.FloorRequests.Select(o => o.PassengerCount)); // Excess passengers remain
            Assert.Contains(floor, nearestElevator.FloorRequests.Select(o => o.TargetFloor)); // Floor request should be added to nearest elevator queue
        }
    }
}
