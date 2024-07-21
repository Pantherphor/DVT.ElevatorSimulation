using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorSimulation.Application.UseCases
{
    public interface IElevatorControlUseCase
    {
        Task CallElevatorAsync(FloorRequest request);
        Task MoveElevatorAsync(int elevatorId, int floor);
        IEnumerable<ElevatorStatus> GetElevatorStatus();
    }

    public class ElevatorControlUseCase : IElevatorControlUseCase
    {
        private readonly Building building;
        private readonly IElevatorSystem elevatorSystem;

        public ElevatorControlUseCase(Building building)
        {
            this.building = building;
            elevatorSystem = this.building.ElevatorSystem;
        }

        public async Task CallElevatorAsync(FloorRequest request)
        {
            await elevatorSystem.CallElevatorAsync(request);
        }

        public async Task MoveElevatorAsync(int elevatorId, int floor)
        {
            await elevatorSystem.MoveElevatorAsync(elevatorId, floor);
        }

        public IEnumerable<ElevatorStatus> GetElevatorStatus()
        {
            return elevatorSystem.GetElevatorStatus();
        }
    }
}
