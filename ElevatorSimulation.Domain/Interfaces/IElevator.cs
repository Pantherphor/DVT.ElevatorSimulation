using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Services;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevator : IElevatorOparations
    {
        int Id { get; set; }
        int CurrentFloor { get; }
        bool IsMoving { get; set; }
        int PassengerCount { get; }
        int MaxPassengerLimit { get; }
        int CallingFloor { get; }

        enElevatorDirection Direction { get; set; }
        enElevatorDoorState DoorState { get; }

        IElevatorMover ElevatorMover { get; }

        bool IsFull(int passengerCount);
        int GetExcessPassangers(int passengerCount);
        int IncrementCurrentFloor(int step);
        void DecrementPassengerCount(int passengerCount);
        void IncrementPassengerCount(int passengerCount);
        void ResetPassengerCount();
    }
}
