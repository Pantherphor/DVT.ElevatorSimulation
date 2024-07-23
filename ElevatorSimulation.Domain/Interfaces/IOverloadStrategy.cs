using System.Threading.Tasks;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IOverloadStrategy
    {
        Task HandleOverloadAsync(IElevatorSystem elevatorSystem, IElevator nearestElevator, int callingFloor, int floor, int excessPassengers);
    }
}
