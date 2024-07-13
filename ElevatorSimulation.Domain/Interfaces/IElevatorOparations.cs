using ElevatorSimulation.Domain.Entities;
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
        void RemoveFloorRequest(FloorRequest floorRequest);
    }
}
