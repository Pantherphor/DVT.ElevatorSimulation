using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Services;
using System;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevatorEvents
    {
        event Action<FloorRequest> ElevatorRequestOverloadChanged;
    }

    public interface IElevator : IElevatorOparations, IElevatorEvents
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
        internal void IncrementPassengerCount(int passengerCount);
        void ResetPassengerCount();
        void SetCallingFloor(int callingFloor);
    }
}
