namespace ElevatorSimulation.Domain.Entities
{
    public class FloorRequest
    {
        private readonly int targetFloor;
        private readonly int passengerCount;
        private readonly int callingFloor;

        public FloorRequest(int callingFloor, int targetFloor, int passengerCount)
        {
            this.targetFloor = targetFloor;
            this.passengerCount = passengerCount;
            this.callingFloor = callingFloor;
        }

        public int TargetFloor => targetFloor;
        public int PassengerCount => passengerCount;

        public int CallingFloor => callingFloor;
    }
}
