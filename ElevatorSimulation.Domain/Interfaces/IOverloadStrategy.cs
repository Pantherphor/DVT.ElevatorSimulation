namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IOverloadStrategy
    {
        void HandleOverload(IElevatorSystem elevatorSystem, IElevator nearestElevator, int floor, int excessPassengers);
    }
}
