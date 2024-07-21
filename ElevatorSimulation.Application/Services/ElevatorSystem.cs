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
    public class ElevatorMovementHistory
    {
        public int ElevatorId { get; set; }
        public int CallingFloor { get; set; }
        public int CurrentFloor { get; set; }
        public int TargetFloor { get; set; }
        public enElevatorDirection Direction { get; set; }
        public int PassengerCount { get; set; }
        public bool IsMoving { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ElevatorSystem : IElevatorSystem
    {
        private IOverloadStrategy overloadStrategy;
        private readonly Dictionary<int, ElevatorStatus> elevatorStatuses = new();
        private readonly Dictionary<int, List<ElevatorMovementHistory>> elevatorHistory = new();

        public ElevatorSystem(IEnumerable<IElevator> elevators)
        {
            Elevators = elevators;
            this.overloadStrategy = new DefaultOverloadStrategy();

            foreach (var elevator in Elevators)
            {
                var mover = elevator.ElevatorMover;
                mover.ElevatorDoorStateChanged += HandleDoorClosed;
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

        private void HandleElevatorStatusChanged(int elevatorId, enElevatorDirection direction, int callingFloor, int currentFloor, int targetFloor, bool isMoving)
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
                IsMoving = isMoving
            };

            elevatorHistory[elevatorId].Add(new ElevatorMovementHistory
            {
                ElevatorId = elevator.Id,
                CallingFloor = callingFloor,
                CurrentFloor = currentFloor,
                TargetFloor = targetFloor,
                Direction = direction,
                PassengerCount = elevator.PassengerCount,
                IsMoving = isMoving,
                Timestamp = DateTime.Now
            });
        }

        public IEnumerable<ElevatorStatus> GetElevatorStatus()
        {
            DisplayElevatorHistory();
            return Elevators.Select(o => o.GetElevatorStatus());
        }

        private void DisplayElevatorHistory()
        {
            DisplayTableHeader();
            foreach (var history in elevatorHistory.Values.SelectMany(h => h))
            {
                Console.WriteLine($"| {history.ElevatorId,8} | {history.CallingFloor,13} | {history.CurrentFloor,13} | {history.TargetFloor,12} | {history.Direction,-9} | {history.PassengerCount,10} | {history.IsMoving,7} | {history.Timestamp:HH:mm:ss} |");
            }
            Console.WriteLine(ConsoleConstants.TableSeparator);
        }

        private void DisplayTableHeader()
        {
            Console.WriteLine(ConsoleConstants.TableSeparator);
            Console.WriteLine("| Elevator | Calling Floor | Current Floor | Target Floor | Direction | Passengers | Moving | Timestamp |");
            Console.WriteLine(ConsoleConstants.TableSeparator);
        }

        private void HandleDoorClosed(int elevatorId, string eventMessage)
        {
            Console.WriteLine(ConsoleConstants.DoorClosedHeader);
            Console.WriteLine($"| {ConsoleConstants.ElevatorIdMessage} {elevatorId} | {ConsoleConstants.EventMessage} {eventMessage,16} |");
            Console.WriteLine(ConsoleConstants.DoorClosedHeader);
        }

        private void HandleElevatorPassangerCountChanged(int elevatorId, int callingFloor, int targetFloor, int excessPassangers)
        {
            Console.WriteLine(ConsoleConstants.PassangerCountHeader);
            Console.WriteLine("| Elevator | Calling Floor | Target Floor | Passengers |");
            Console.WriteLine(ConsoleConstants.PassangerCountHeader);

            Console.WriteLine(ConsoleConstants.PassangerCountHeader);
            Console.WriteLine($"| {elevatorId,8} | {callingFloor,13} | {targetFloor,13} | {excessPassangers,8} |");
            Console.WriteLine(ConsoleConstants.PassangerCountHeader);
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
