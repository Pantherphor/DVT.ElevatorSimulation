namespace ElevatorSimulation.Domain.Entities
{
    public class FloorRequest
    {
        private readonly int targetFloor;
        private readonly int passengerCount;

        public FloorRequest(int targetFloor, int excessPassengers)
        {
            this.targetFloor = targetFloor;
            this.passengerCount = excessPassengers;
        }

        public int TargetFloor => targetFloor;
        public int PassengerCount => passengerCount;
    }
}
