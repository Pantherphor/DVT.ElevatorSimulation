using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using System.Collections.Generic;

namespace ElevatorSimulation.Domain.Interfaces
{

    public interface IElevatorOparations
    {
        Queue<FloorRequest> FloorRequests { get; }

        void AddFloorRequest(FloorRequest targetFloor);
        void MoveToNextFloor();
        ElevatorStatus GetElevatorStatus();
    }

    public interface IElevator : IElevatorOparations
    {
        int Id { get; set; }
        int CurrentFloor { get; set; }
        bool IsMoving { get; }
        int PassengerCount { get; set; }
        int MaxPassengerLimit { get; }

        enElevatorDirection Direction { get; set; }
        enElevatorDoorState DoorState { get; }

        bool IsFull();
        int GetExcessPassangers();
    }
}
