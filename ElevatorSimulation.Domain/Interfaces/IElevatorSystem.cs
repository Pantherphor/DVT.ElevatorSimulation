using ElevatorSimulation.Domain.Entities;
using System.Collections.Generic;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevatorSystem
    {
        IEnumerable<IElevator> Elevators { get; set; }

        void CallElevator(FloorRequest request);
        void MoveElevator(int elevatorId, int floor);
        IEnumerable<ElevatorStatus> GetElevatorStatus();
    }
}
