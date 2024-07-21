using ElevatorSimulation.Domain.Enums;
using System;

namespace ElevatorSimulation.Domain.Entities
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
        public enElevatorDoorState DoorState { get; set; }
    }
}
