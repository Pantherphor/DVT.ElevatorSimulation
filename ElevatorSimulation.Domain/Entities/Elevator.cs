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
        public enElevatorState State { get; private set; }
        public int Id { get; set; }
        public int PassengerCount { get; set; }
        public bool IsMoving { get; set; }
        public enElevatorDirection Direction { get; set; }

        public int MaxPassengerLimit { get; set; }

        public Queue<FloorRequest> FloorRequests => floorRequests;

        private readonly Queue<FloorRequest> floorRequests;

        public Elevator()
        {
            CurrentFloor = 0;
            floorRequests = new Queue<FloorRequest>();
            State = enElevatorState.Idle;
            Direction = enElevatorDirection.Stationery;
        }


        public void AddFloorRequest(FloorRequest targetFloor)
        {
            floorRequests.Enqueue(targetFloor);
        }

        public async void MoveToNextFloor()
        {
            if (floorRequests.Count > 0)
            {
                var floorRequest = floorRequests.Dequeue();
                int nextFloor = floorRequest.Floor;
                IsMoving = true;
                PassengerCount = floorRequest.PassengerCount;
                Direction = nextFloor > CurrentFloor ? enElevatorDirection.Up : enElevatorDirection.Down;
                State = enElevatorState.Moving;

                await Task.Delay(Math.Abs(nextFloor - CurrentFloor) * 1000); // Simulate time to move

                CurrentFloor = nextFloor;
                IsMoving = false;
                Direction = enElevatorDirection.Stationery;
                State = enElevatorState.Idle;
            }
            else
            {
                Direction = enElevatorDirection.Stationery;
                State = enElevatorState.Idle;
            }
        }

        public void DisplayStatus()
        {
            throw new NotImplementedException();
        }
    }
}
