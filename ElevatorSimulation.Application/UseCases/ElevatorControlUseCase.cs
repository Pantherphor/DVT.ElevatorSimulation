using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using System.Collections.Generic;

namespace ElevatorSimulation.Application.UseCases
{
    public interface IElevatorControlUseCase
    {
        void CallElevator(FloorRequest request);
        void MoveElevator(int elevatorId, int floor);
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

        public void CallElevator(FloorRequest request)
        {
            elevatorSystem.CallElevator(request);
        }

        public void MoveElevator(int elevatorId, int floor)
        {
            elevatorSystem.MoveElevator(elevatorId, floor);
        }

        public IEnumerable<ElevatorStatus> GetElevatorStatus()
        {
            return elevatorSystem.GetElevatorStatus();
        }
    }
}
