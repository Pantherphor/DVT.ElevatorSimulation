using ElevatorSimulation.Domain.Entities;
using System.Threading.Tasks;

namespace ElevatorSimulation.Application.UseCases
{
    public class ElevatorControlUseCase
    {
        private readonly Building _building;

        public ElevatorControlUseCase(Building building)
        {
            _building = building;
        }

        public void AddTargetFloor(FloorRequest floorRequest, int elevatorId)
        {
            var elevator = _building.Elevators[elevatorId];
            elevator.AddFloorRequest(floorRequest);
        }

        public async Task MoveElevatorsAsync()
        {
            foreach (var elevator in _building.Elevators)
            {
                await elevator.MoveToNextFloorAsync();
            }
        }

        public void DisplayElevatorStatuses()
        {
            foreach (var elevator in _building.Elevators)
            {
                elevator.GetElevatorStatus();
            }
        }
    }
}
