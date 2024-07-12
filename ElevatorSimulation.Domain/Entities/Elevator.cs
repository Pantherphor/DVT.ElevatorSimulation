using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSimulation.Domain.Entities
{
    public class Elevator : IElevator
    {
        public int CurrentFloor { get; set; }
        public int Id { get; set; }
        public int PassengerCount { get; set; }
        public bool IsMoving { get; set; }
        public int MaxPassengerLimit { get; set; }

        public enElevatorDoorState DoorState { get; private set; }
        public enElevatorDirection Direction { get; set; }


        public Queue<FloorRequest> FloorRequests { get; }

        public Elevator()
        {
            CurrentFloor = 0;
            FloorRequests = new Queue<FloorRequest>();
            DoorState = enElevatorDoorState.Closed;
            Direction = enElevatorDirection.None;
        }


        public void AddFloorRequest(FloorRequest targetFloor)
        {
            FloorRequests.Enqueue(targetFloor);
        }

        public async void MoveToNextFloor()
        {
            if (FloorRequests.Count > 0)
            {
                var floorRequest = FloorRequests.Dequeue();
                int tagerFloor = floorRequest.TargetFloor;
                IsMoving = true;
                PassengerCount = floorRequest.PassengerCount;
                Direction = tagerFloor > CurrentFloor ? enElevatorDirection.Up : enElevatorDirection.Down;
                DoorState = enElevatorDoorState.Closed;

                await Task.Delay(Math.Abs(tagerFloor - CurrentFloor) * 1000); // Simulate time to move

                CurrentFloor = tagerFloor;
                IsMoving = false;
                Direction = enElevatorDirection.None;
                DoorState = enElevatorDoorState.Closed;
            }
            else
            {
                Direction = enElevatorDirection.None;
                DoorState = enElevatorDoorState.Closed;
            }
        }

        public ElevatorStatus GetElevatorStatus()
        {
            return new ElevatorStatus
            {
                Id = Id,
                CurrentFloor = CurrentFloor,
                Direction = Direction,
                IsMoving = IsMoving,
                PassengerCount = PassengerCount
            };
        }

        public bool IsFull()
        {
            return PassengerCount > MaxPassengerLimit;
        }

        public int GetExcessPassangers()
        {
            return PassengerCount - MaxPassengerLimit;
        }
    }
}
