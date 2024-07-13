using ElevatorSimulation.Application.Strategies;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
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

            foreach (var elevator in Elevators)
            {
                var mover = elevator.ElevatorMover;
                mover.ElevatorDoorStateChanged += HandleDoorClosed;
                mover.ElevatorStatusChanged += HandleElevatorStatusChanged;
                mover.ElevatorPassangerCountChanged += HandleElevatorPassangerCountChanged;
            }
        }

        private void HandleElevatorPassangerCountChanged(int arg1, int arg2, int arg3, int arg4)
        {
            throw new NotImplementedException();
        }

        private void HandleElevatorStatusChanged(int arg1, enElevatorDirection arg2, int arg3, int arg4, int arg5, bool arg6)
        {
            throw new NotImplementedException();
        }

        private void HandleDoorClosed(int arg1, string arg2)
        {
            throw new NotImplementedException();
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

                if (nearestElevator.IsFull(request.PassengerCount))
                {
                    // Handle overloading by moving excess passengers to another elevator or other logic
                    int excessPassengers = nearestElevator.GetExcessPassangers(request.PassengerCount);
                    overloadStrategy.HandleOverload(this, nearestElevator, request.TargetFloor, excessPassengers);
                }
            }
        }

        public void MoveElevator(int elevatorId, int floor)
        {
            var elevator = elevators.FirstOrDefault(e => e.Id == elevatorId);
            elevator?.MoveToNextFloorAsync();
        }

        public IEnumerable<ElevatorStatus> GetElevatorStatus()
        {
            return elevators.Select(o => o.GetElevatorStatus());
        }
    }
}
