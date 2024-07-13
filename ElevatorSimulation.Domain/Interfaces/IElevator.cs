using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Services;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevator : IElevatorOparations
    {
        int Id { get; set; }
        int CurrentFloor { get; set; }
        bool IsMoving { get; set; }
        int PassengerCount { get; set; }
        int MaxPassengerLimit { get; }

        enElevatorDirection Direction { get; set; }
        enElevatorDoorState DoorState { get; }

        IElevatorMover ElevatorMover { get; }

        bool IsFull(int passengerCount);
        int GetExcessPassangers(int passengerCount);
    }
}
