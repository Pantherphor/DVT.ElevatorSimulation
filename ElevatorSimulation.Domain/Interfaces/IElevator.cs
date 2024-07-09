using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using System.Collections.Generic;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevator
    {
        int CurrentFloor { get; set; }
        enElevatorDirection Direction { get; set; }
        int Id { get; }
        bool IsMoving { get; }
        int PassengerCount { get; set; }
        enElevatorState State { get; }
        int MaxPassengerLimit { get; }

        void AddFloorRequest(FloorRequest targetFloor);
        void DisplayStatus();
        Queue<FloorRequest> FloorRequests { get; }
    }
}
