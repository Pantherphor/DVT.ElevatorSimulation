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
        private readonly List<IElevator> elevators;
        private readonly int maxPassengerLimit;
        private IOverloadStrategy overloadStrategy;

        public ElevatorSystem(int numberOfElevators, int maxPassengerLimit)
        {
            this.maxPassengerLimit = maxPassengerLimit;
            elevators = new List<IElevator>();
            this.overloadStrategy = new DefaultOverloadStrategy();

            for (int i = 0; i < numberOfElevators; i++)
            {
                elevators.Add(new Elevator { Id = i + 1, CurrentFloor = 0, PassengerCount = 0, IsMoving = false });
            }
        }

        public List<IElevator> Elevators => elevators;

        public void SetOverloadStrategy(IOverloadStrategy strategy)
        {
            this.overloadStrategy = strategy;
        }

        public void CallElevator(FloorRequest request)
        {
            var nearestElevator = elevators.OrderBy(e => Math.Abs(e.CurrentFloor - request.Floor))
                                            .ThenBy(e => e.PassengerCount)
                                            .FirstOrDefault();

            if (nearestElevator != null)
            {
                nearestElevator.AddFloorRequest(new FloorRequest(request.Floor, request.PassengerCount));
                nearestElevator.PassengerCount += request.PassengerCount;

                if (nearestElevator.PassengerCount > maxPassengerLimit)
                {
                    // Handle overloading by moving excess passengers to another elevator or other logic
                    int excessPassengers = nearestElevator.PassengerCount - maxPassengerLimit;
                    nearestElevator.PassengerCount = maxPassengerLimit;
                    overloadStrategy.HandleOverload(this, nearestElevator, request.Floor, excessPassengers);
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

        public List<ElevatorStatus> GetElevatorStatus()
        {
            return elevators.Select(e => new ElevatorStatus
            {
                Id = e.Id,
                CurrentFloor = e.CurrentFloor,
                Direction = e.Direction,
                IsMoving = e.IsMoving,
                PassengerCount = e.PassengerCount
            }).ToList();
        }
    }
}
