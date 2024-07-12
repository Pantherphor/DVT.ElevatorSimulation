﻿using System;
using System.Collections.Generic;
using System.Linq;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Services;
using Moq;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class ElevatorSystemTests
    {
        private ElevatorSystem _elevatorSystem;
        private List<Mock<IElevator>> _mockElevators;

        public ElevatorSystemTests()
        {
            _mockElevators = new List<Mock<IElevator>>();

            for (int i = 0; i < 3; i++)
            {
                var mockElevator = new Mock<IElevator>();
                mockElevator.SetupAllProperties();
                mockElevator.Setup(e => e.AddFloorRequest(It.IsAny<FloorRequest>()))
                    .Callback<FloorRequest>(request =>
                {
                    mockElevator.Object.FloorRequests.Enqueue(request);
                });
                mockElevator.Setup(e => e.FloorRequests).Returns(new Queue<FloorRequest>());
                mockElevator.Object.CurrentFloor = 0;
                mockElevator.Object.PassengerCount = 0;
                mockElevator.Object.Id = i + 1;
                _mockElevators.Add(mockElevator);
            }

            _elevatorSystem = new ElevatorSystem(_mockElevators.Select(e => e.Object));
        }

        [Fact]
        public void CallElevator_ShouldAssignRequestToNearestElevator()
        {
            // Arrange
            var request = new FloorRequest(5,2);

            // Act
            _elevatorSystem.CallElevator(request);

            // Assert
            var nearestElevator = _mockElevators.OrderBy(e => Math.Abs(e.Object.CurrentFloor - request.TargetFloor)).First().Object;
            Assert.Single(nearestElevator.FloorRequests);
            Assert.Equal(request.TargetFloor, nearestElevator.FloorRequests.First().TargetFloor);
            Assert.Equal(request.PassengerCount, nearestElevator.PassengerCount);
        }

        [Fact]
        public void CallElevator_ShouldHandleOverload()
        {
            // Arrange
            var overloadStrategyMock = new Mock<IOverloadStrategy>();
            _elevatorSystem.SetOverloadStrategy(overloadStrategyMock.Object);

            var request = new FloorRequest(5,10);
            _mockElevators[0].Setup(e => e.IsFull()).Returns(true);
            _mockElevators[0].Setup(e => e.GetExcessPassangers()).Returns(5);

            // Act
            _elevatorSystem.CallElevator(request);

            // Assert
            var overloadedElevator = _mockElevators[0].Object;
            overloadStrategyMock.Verify(s => s.HandleOverload(_elevatorSystem, overloadedElevator, request.TargetFloor, 5), Times.Once);
        }

        [Fact]
        public void MoveElevator_ShouldAddMoveRequest()
        {
            // Arrange
            int elevatorId = 1;
            int targetFloor = 5;

            // Act
            _elevatorSystem.MoveElevator(elevatorId, targetFloor);

            // Assert
            var elevator = _mockElevators.First(e => e.Object.Id == elevatorId).Object;
            Assert.Single(elevator.FloorRequests);
            Assert.Equal(targetFloor, elevator.FloorRequests.First().TargetFloor);
        }

        [Fact]
        public void GetElevatorStatus_ShouldReturnCorrectStatus()
        {
            // Arrange
            var expectedStatus = new List<ElevatorStatus>
            {
                new ElevatorStatus { Id = 1, CurrentFloor = 0, PassengerCount = 0, Direction = enElevatorDirection.Up },
                new ElevatorStatus { Id = 2, CurrentFloor = 0, PassengerCount = 0, Direction = enElevatorDirection.Up },
                new ElevatorStatus { Id = 3, CurrentFloor = 0, PassengerCount = 0, Direction = enElevatorDirection.Up }
            };

            foreach (var mockElevator in _mockElevators)
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
            var statuses = _elevatorSystem.GetElevatorStatus();

            // Assert
            Assert.Equal(3, statuses.Count());
            foreach (var status in statuses)
            {
                var expected = expectedStatus.First(s => s.Id == status.Id);
                Assert.Equal(expected.CurrentFloor, status.CurrentFloor);
                Assert.Equal(expected.PassengerCount, status.PassengerCount);
                Assert.Equal(expected.Direction, status.Direction);
            }
        }
    }
}
