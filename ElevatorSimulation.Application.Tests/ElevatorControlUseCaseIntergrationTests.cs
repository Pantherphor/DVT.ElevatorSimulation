using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class ElevatorControlUseCaseIntergrationTests
    {
        [Theory(Timeout = 100)]
        [InlineData(0, 5)]
        [InlineData(2, 3)]
        [InlineData(1, 10)]
        public void AddTargetFloor_Should_Add_Floor_To_Elevator_Should_Not_Move_Elevater(int elevetorId, int targetFloor)
        {
            //arrange
            var building = new Building(0, 10, 3);
            var useCase = new ElevatorControlUseCase(building);

            //action
            useCase.AddTargetFloor(new FloorRequest(0, targetFloor, 2), elevetorId);

            //assert
            Assert.False(building.Elevators[elevetorId].IsMoving);
        }

        [Theory(Timeout = 66000)]
        [InlineData(0, 5)]
        [InlineData(2, 3)]
        [InlineData(1, 10)]
        public async Task MoveElevatorsAsync_Should_Move_Elevators_To_Next_Floor(int elevatorId, int targetFloor)
        {
            //arrange
            var building = new Building(0, 10, 3);
            var useCase = new ElevatorControlUseCase(building);

            //action
            useCase.AddTargetFloor(new FloorRequest(0, targetFloor, 2), elevatorId);
            await useCase.MoveElevatorsAsync();

            await Task.Delay(15500); // Wait for the elevator to process

            //assert
            Assert.Equal(targetFloor, building.Elevators[elevatorId].CurrentFloor);
        }

    }
}
