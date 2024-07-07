using ElevatorSimulation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSimulation.Domain.Entities
{
    public interface IElevator
    {
        int CurrentFloor { get; set; }
        enElevatorDirection Direction { get; set; }
        int Id { get; }
        bool IsMoving { get; }
        int PassengerCount { get; }
        enElevatorState State { get; }

        void AddFloorRequest(int targetFloor);
        void DisplayStatus();
    }

    public class Elevator : IElevator
    {
        public int CurrentFloor { get; set; }
        public enElevatorState State { get; private set; }
        public int Id { get; internal set; }
        public int PassengerCount { get; internal set; }
        public bool IsMoving { get; internal set; }
        public enElevatorDirection Direction { get; set; }


        private readonly Queue<int> floorRequests;
        private Task processingTask;

        public Elevator()
        {
            CurrentFloor = 0;
            floorRequests = new Queue<int>();
            State = enElevatorState.Idle;
            Direction = enElevatorDirection.Stationery;
        }

        public void AddFloorRequest(int targetFloor)
        {
            floorRequests.Enqueue(targetFloor);
        }

        public async void MoveToNextFloor()
        {
            if (floorRequests.Count > 0)
            {
                int nextFloor = floorRequests.Dequeue();
                IsMoving = true;
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
