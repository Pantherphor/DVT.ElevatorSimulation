using ElevatorSimulation.Domain.Enums;

namespace ElevatorSimulation.Domain.Entities
{
    public class ElevatorStatus
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; }
        public int TargetFloor { get; set; }
        public enElevatorDirection Direction { get; set; }
        public bool IsMoving { get; set; }
        public int PassengerCount { get; set; }
        public int CallingFloor { get; set; }
    }
}
