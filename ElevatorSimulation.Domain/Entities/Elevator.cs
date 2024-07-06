using ElevatorSimulation.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ElevatorSimulation.Domain.Entities
{
    public class Elevator
    {
        public int CurrentFloor { get; private set; }
        public Queue<int> TargetFloors { get; private set; }
        public enElevatorState State { get; private set; }

        public Elevator()
        {
            CurrentFloor = 0;
            TargetFloors = new Queue<int>();
            State = enElevatorState.Idle;
        }

        public void AddTargetFloor(int targetFloor)
        {
            TargetFloors.Enqueue(targetFloor);
        }

        public void MoveToNextFloor()
        {
            if (TargetFloors.Count > 0)
            {
                CurrentFloor = TargetFloors.Dequeue();
                State = enElevatorState.Moving;
            }
            else
            {
                State = enElevatorState.Idle;
            }
        }

        public void DisplayStatus()
        {
            throw new NotImplementedException();
        }
    }
}
