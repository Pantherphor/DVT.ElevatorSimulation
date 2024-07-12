using ElevatorSimulation.Application.Strategies;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorSimulation.Services
{
    public class ElevatorSystem : IElevatorSystem
    {
        private readonly IEnumerable<IElevator> elevators;
        private IOverloadStrategy overloadStrategy;

        public ElevatorSystem(IEnumerable<IElevator> elevators)
        {
            this.elevators = elevators;
            this.overloadStrategy = new DefaultOverloadStrategy();
        }

        public IEnumerable<IElevator> Elevators => elevators;

        public void SetOverloadStrategy(IOverloadStrategy strategy)
        {
            this.overloadStrategy = strategy;
        }

        public void CallElevator(FloorRequest request)
        {
            var nearestElevator = elevators.OrderBy(e => Math.Abs(e.CurrentFloor - request.TargetFloor))
                                            .ThenBy(e => e.PassengerCount)
                                            .FirstOrDefault();

            if (nearestElevator != null)
            {
                nearestElevator.AddFloorRequest(new FloorRequest(request.TargetFloor, request.PassengerCount));
                nearestElevator.PassengerCount += request.PassengerCount;

                if (nearestElevator.IsFull())
                {
                    // Handle overloading by moving excess passengers to another elevator or other logic
                    int excessPassengers = nearestElevator.GetExcessPassangers();
                    overloadStrategy.HandleOverload(this, nearestElevator, request.TargetFloor, excessPassengers);
                }
            }
        }

        public void MoveElevator(int elevatorId, int floor)
        {
            var elevator = elevators.FirstOrDefault(e => e.Id == elevatorId);

            if (elevator != null)
            {
                elevator.AddFloorRequest(new FloorRequest(floor, 0)); // Assuming 0 passengers for a move request
            }
        }

        public IEnumerable<ElevatorStatus> GetElevatorStatus()
        {
            return elevators.Select(o => o.GetElevatorStatus());
        }
    }
}
