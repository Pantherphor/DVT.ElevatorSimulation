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
        private IOverloadStrategy overloadStrategy;

        public ElevatorSystem(IEnumerable<IElevator> elevators)
        {
            Elevators = elevators;
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

        public IEnumerable<IElevator> Elevators { get; set; }

        public void SetOverloadStrategy(IOverloadStrategy strategy)
        {
            this.overloadStrategy = strategy;
        }

        public void CallElevator(FloorRequest request)
        {
            var nearestElevator = Elevators
                                    .OrderBy(e => Math.Abs(e.CurrentFloor - request.CallingFloor))
                                    .ThenBy(e => e.PassengerCount)
                                    .ThenBy(e => e.Direction == enElevatorDirection.None ? 0 : 1)
                                    .ThenBy(e => (e.Direction == enElevatorDirection.Up && request.CallingFloor >= e.CurrentFloor) ||
                                                 (e.Direction == enElevatorDirection.Down && request.CallingFloor <= e.CurrentFloor) ? 0 : 1)
                                    .FirstOrDefault();

            if (nearestElevator != null)
            {
                nearestElevator.AddFloorRequest(new FloorRequest(request.CallingFloor, request.TargetFloor, request.PassengerCount));
                nearestElevator.PassengerCount += request.PassengerCount;

                if (nearestElevator.IsFull(request.PassengerCount))
                {
                    // Handle overloading by moving excess passengers to another elevator or other logic
                    int excessPassengers = nearestElevator.GetExcessPassangers(request.PassengerCount);
                    overloadStrategy.HandleOverload(this, nearestElevator, request.CallingFloor, request.TargetFloor, excessPassengers);
                }
            }
        }

        public void MoveElevator(int elevatorId, int floor)
        {
            var elevator = Elevators.FirstOrDefault(e => e.Id == elevatorId);
            elevator?.MoveToNextFloorAsync();
        }

        public IEnumerable<ElevatorStatus> GetElevatorStatus()
        {
            return Elevators.Select(o => o.GetElevatorStatus());
        }
    }
}
