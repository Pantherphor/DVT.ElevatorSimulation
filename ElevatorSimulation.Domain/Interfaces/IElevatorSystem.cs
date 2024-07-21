using ElevatorSimulation.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevatorSystem
    {
        IEnumerable<IElevator> Elevators { get; set; }

        Task CallElevator(FloorRequest request);
        Task MoveElevator(int elevatorId, int floor);
        IEnumerable<ElevatorStatus> GetElevatorStatus();
    }
}
