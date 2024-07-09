using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Linq;

namespace ElevatorSimulation.Application.Strategies
{
    public class DefaultOverloadStrategy : IOverloadStrategy
    {
        public void HandleOverload(IElevatorSystem elevatorSystem, IElevator nearestElevator, int floor, int excessPassengers)
        {
            nearestElevator.AddFloorRequest(new FloorRequest(floor, excessPassengers));

            var availableElevator = elevatorSystem.Elevators
                .Where(e => e.Id != nearestElevator.Id && e.PassengerCount < e.MaxPassengerLimit)
                .OrderBy(e => Math.Abs(e.CurrentFloor - nearestElevator.CurrentFloor))
                .FirstOrDefault();

            if (availableElevator != null)
            {
                int passengersToTransfer = Math.Min(excessPassengers, availableElevator.MaxPassengerLimit - availableElevator.PassengerCount);
                availableElevator.PassengerCount += passengersToTransfer;
                excessPassengers -= passengersToTransfer;
                availableElevator.AddFloorRequest(new FloorRequest(floor, passengersToTransfer));
            }

            if (excessPassengers > 0)
            {
                // If there are still excess passengers and no available elevator can take them,
                // they should remain with the nearest elevator and added to this queue.
                nearestElevator.AddFloorRequest(new FloorRequest(floor, excessPassengers));
            }
        }
    }
}
