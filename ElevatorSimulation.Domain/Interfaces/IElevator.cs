using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSimulation.Domain.Interfaces
{

    public interface IElevatorOparations
    {
        IList<FloorRequest> FloorRequests { get; }

        void AddFloorRequest(FloorRequest targetFloor);
        Task MoveToNextFloorAsync();
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
