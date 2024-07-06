using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using Xunit;

namespace ElevatorSimulation.Domain.Tests
{
    public class ElevatorTests
    {
        [Fact]
        public void Elevator_Should_Initialize_With_Correct_Values()
        {
            //arrange
            var elevator = new Elevator();

            //assert
            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Empty(elevator.TargetFloors);
            Assert.Equal(enElevatorState.Idle, elevator.State);
        }

        [Fact]
        public void AddTargetFloor_Should_Add_Target_Floor_To_Queue()
        {
            //arrange
            var elevator = new Elevator();
            
            //action
            elevator.AddTargetFloor(5);
            
            //assert
            Assert.Contains(5, elevator.TargetFloors);
        }

        [Fact]
        public void MoveToNextFloor_Should_Move_Elevator_To_Next_Floor()
        {
            //arrange
            var elevator = new Elevator();

            //action
            elevator.AddTargetFloor(5);
            elevator.MoveToNextFloor();
            
            //assert
            Assert.Equal(5, elevator.CurrentFloor);
        }

        [Fact]
        public void MoveToNextFloor_Should_Move_Elevator_To_Next_Floor_Idle()
        {
            //arrange
            var elevator = new Elevator();

            //action
            elevator.MoveToNextFloor();

            //assert
            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Equal(enElevatorState.Idle, elevator.State);
        }
    }
}
