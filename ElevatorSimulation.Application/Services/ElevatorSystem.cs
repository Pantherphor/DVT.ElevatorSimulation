using ElevatorSimulation.Application.Services;
using ElevatorSimulation.Application.Strategies;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSimulation.Services
{
    public class ElevatorSystem : IElevatorSystem
    {
        private IOverloadStrategy overloadStrategy;
        private readonly Dictionary<int, ElevatorStatus> elevatorStatuses = new();
        private readonly Dictionary<int, IList<ElevatorMovementHistory>> elevatorHistory = new();

        public ElevatorSystem(IEnumerable<IElevator> elevators)
        {
            Elevators = elevators;
            this.overloadStrategy = new DefaultOverloadStrategy();

            foreach (var elevator in Elevators)
            {
                var mover = elevator.ElevatorMover;
                mover.ElevatorStatusChanged += HandleElevatorStatusChanged;

                elevatorStatuses[elevator.Id] = new ElevatorStatus
                {
                    Id = elevator.Id,
                    CallingFloor = elevator.CallingFloor,
                    CurrentFloor = elevator.CurrentFloor,
                    Direction = elevator.Direction,
                    PassengerCount = elevator.PassengerCount,
                    IsMoving = elevator.IsMoving
                };

                elevatorHistory[elevator.Id] = new List<ElevatorMovementHistory>();
            }
        }

        private void HandleElevatorStatusChanged(int elevatorId, enElevatorDirection direction, int callingFloor, int currentFloor, int targetFloor, bool isMoving, enElevatorDoorState doorState)
        {
            var elevator = Elevators.First(e => e.Id == elevatorId);
            elevatorStatuses[elevatorId] = new ElevatorStatus
            {
                Id = elevator.Id,
                CallingFloor = callingFloor,
                CurrentFloor = currentFloor,
                TargetFloor = targetFloor,
                Direction = direction,
                PassengerCount = elevator.PassengerCount,
                IsMoving = isMoving,
                DoorState = doorState
            };

            elevatorHistory[elevatorId].Add(new ElevatorMovementHistory
            {
                ElevatorId = elevator.Id,
                CallingFloor = callingFloor,
                CurrentFloor = currentFloor,
                TargetFloor = targetFloor,
                Direction = direction,
                DoorState = doorState,
                PassengerCount = elevator.PassengerCount,
                IsMoving = isMoving,
                Timestamp = DateTime.Now
            });
        }

        public IEnumerable<ElevatorStatus> GetElevatorStatus()
        {
            return Elevators.Select(o => o.GetElevatorStatus());
        }

        public Dictionary<int, IList<ElevatorMovementHistory>> GetElevatorMovementHistory()
        {
            return elevatorHistory;
        }

        public IEnumerable<IElevator> Elevators { get; set; }

        public void SetOverloadStrategy(IOverloadStrategy strategy)
        {
            this.overloadStrategy = strategy;
        }

        public async Task CallElevatorAsync(FloorRequest request)
        {
            var nearestElevator = Elevators
                                    .Where(e => !e.IsMoving || !e.FloorRequests.Any())
                                    .OrderBy(e => Math.Abs(e.CurrentFloor - request.CallingFloor))
                                    .ThenBy(e => e.PassengerCount)
                                    .ThenBy(e => e.Direction == enElevatorDirection.None ? 0 : 1)
                                    .ThenBy(e => (e.Direction == enElevatorDirection.Up && request.CallingFloor >= e.CurrentFloor) ||
                                                    (e.Direction == enElevatorDirection.Down && request.CallingFloor <= e.CurrentFloor) ? 0 : 1)
                                    .FirstOrDefault();

            if (nearestElevator == null)
            {
                // All elevators are busy, pick the one with the least pending requests
                nearestElevator = Elevators
                                    .OrderBy(e => e.FloorRequests.Count)
                                    .ThenBy(e => Math.Abs(e.CurrentFloor - request.CallingFloor))
                                    .FirstOrDefault();
            }

            if (nearestElevator != null)
            {
                if (nearestElevator.IsFull(request.PassengerCount))
                {
                    int excessPassengers = nearestElevator.GetExcessPassangers(request.PassengerCount);
                    var floorRequest = new FloorRequest(
                        request.CallingFloor, 
                        request.TargetFloor, 
                        (request.PassengerCount - excessPassengers)
                        );

                    nearestElevator.AddFloorRequest(floorRequest);
                    overloadStrategy.HandleOverload(this, nearestElevator, request.CallingFloor, request.TargetFloor, excessPassengers);
                }
                else
                {
                    nearestElevator.AddFloorRequest(new FloorRequest(request.CallingFloor, request.TargetFloor, request.PassengerCount));
                }

                await MoveElevatorAsync(nearestElevator.Id, nearestElevator.CallingFloor);
            }
        }

        public async Task MoveElevatorAsync(int elevatorId, int floor)
        {
            var moveTasks = Elevators.Select(elevator => elevator.MoveToNextFloorAsync()).ToArray();
            await Task.WhenAll(moveTasks);
        }
    }

}
