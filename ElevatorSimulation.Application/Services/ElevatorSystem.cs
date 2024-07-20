﻿using ElevatorSimulation.Application.Strategies;
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
        private readonly Dictionary<int, ElevatorStatus> elevatorStatuses = new();

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

                elevatorStatuses[elevator.Id] = new ElevatorStatus
                {
                    Id = elevator.Id,
                    CallingFloor = elevator.CallingFloor,
                    CurrentFloor = elevator.CurrentFloor,
                    Direction = elevator.Direction,
                    PassengerCount = elevator.PassengerCount,
                    IsMoving = elevator.IsMoving
                };
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
            DrawTable();
        }

        private void DrawTable()
        {
            //Console.Clear();
            DisplayTableHeader();
            foreach (var status in elevatorStatuses.Values)
            {
                Console.WriteLine($"| {status.Id,8} | {status.CallingFloor,13} | {status.CurrentFloor,13} | {status.TargetFloor,12} | {status.Direction,-9} | {status.PassengerCount,10} | {status.IsMoving,8} |");
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------");
        }

        private void DisplayTableHeader()
        {
            Console.WriteLine("------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Elevator | Calling Floor | Current Floor | Target Floor | Direction | Passengers | IsMoving |");
            Console.WriteLine("------------------------------------------------------------------------------------------------");
        }

        private void HandleDoorClosed(int elevatorId, string eventMessage)
        {
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine($"| Elevator ID: {elevatorId} | Message: {eventMessage,16} |");
            Console.WriteLine("----------------------------------------------------------------");
        }

        private void HandleElevatorPassangerCountChanged(int elevatorId, int callingFloor, int targetFloor, int excessPassangers)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("| Elevator | Calling Floor | Target Floor | Passengers |");
            Console.WriteLine("----------------------------------------------------------");

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"| {elevatorId,8} | {callingFloor,13} | {targetFloor,13} | {excessPassangers,8} |");
            Console.WriteLine("----------------------------------------------------------");
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
                //nearestElevator.PassengerCount += request.PassengerCount;

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
            elevator?.MoveToNextFloorAsync(); //TODO: need to pass the elevatorId and targetFloor to enable the central controller to override,
                                              //with this we will later need to have the override strategy that will redirect requests to other
                                              //elevators available at the time.
        }

        public IEnumerable<ElevatorStatus> GetElevatorStatus()
        {
            return Elevators.Select(o => o.GetElevatorStatus());
        }
    }
}
