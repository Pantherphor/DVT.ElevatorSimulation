using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSimulation.Application.Strategies
{
    public class DefaultOverloadStrategy : IOverloadStrategy
    {
        public async Task HandleOverloadAsync(IElevatorSystem elevatorSystem, IElevator nearestElevator, int callingFloor, int targetFloor, int excessPassengers)
        {
            var availableElevator = elevatorSystem.Elevators
                .Where(e => e.Id != nearestElevator.Id && e.PassengerCount < e.MaxPassengerLimit)
                .OrderBy(e => Math.Abs(e.CurrentFloor - nearestElevator.CurrentFloor))
                .FirstOrDefault();

            if (availableElevator != null)
            {
                int passengersToTransfer = Math.Min(excessPassengers, availableElevator.MaxPassengerLimit - availableElevator.PassengerCount);
                excessPassengers -= passengersToTransfer;
                availableElevator.AddFloorRequest(new FloorRequest(callingFloor, targetFloor, passengersToTransfer));
            }

            if (excessPassengers > 0)
            {
                // If there are still excess passengers and no available elevator can take them,
                // they should remain with the nearest elevator and added to this queue.
                //HandleOverload(elevatorSystem, nearestElevator, callingFloor, targetFloor, excessPassengers);
                nearestElevator.AddFloorRequest(new FloorRequest(callingFloor, targetFloor, excessPassengers));
            }
            await Task.CompletedTask;
        }
    }
}
