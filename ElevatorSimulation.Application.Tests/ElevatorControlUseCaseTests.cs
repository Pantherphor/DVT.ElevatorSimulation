using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorSimulation.Application.Tests
{
    public class ElevatorControlUseCaseTests
    {
        [Theory(Timeout = 100)]
        [InlineData(0, 5)]
        [InlineData(2, 3)]
        [InlineData(1, 10)]
        public void AddTargetFloor_Should_Add_Floor_To_Elevator(int elevetorId, int targetFloor)
        {
            //arrange
            var building = new Building(0, 10, 3);
            var useCase = new ElevatorControlUseCase(building);

            //action
            useCase.AddTargetFloor(elevetorId, targetFloor);

            //assert
            Assert.Contains(targetFloor, building.Elevators[elevetorId].TargetFloors);
        }

        [Fact(Timeout = 100)]
        public async Task MoveElevatorsAsync_Should_Move_Elevators_To_Next_Floor()
        {
            //arrange
            var building = new Building(0, 10, 3);
            var useCase = new ElevatorControlUseCase(building);

            //action
            useCase.AddTargetFloor(0, 5);
            await useCase.MoveElevatorsAsync();

            //assert
            Assert.Equal(5, building.Elevators[0].CurrentFloor);
        }

    }
}
