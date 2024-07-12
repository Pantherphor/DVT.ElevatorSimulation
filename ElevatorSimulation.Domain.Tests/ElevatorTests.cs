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
            Assert.Equal(enElevatorDoorState.Closed, elevator.DoorState);
        }

        [Fact]
        public void AddTargetFloor_Should_Add_Target_Floor_To_Queue()
        {
            //arrange
            var elevator = new Elevator();
            
            //action
            elevator.AddFloorRequest(new FloorRequest(5, 2));

            //assert
            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Equal(enElevatorDirection.None, elevator.Direction);
            Assert.Equal(enElevatorDoorState.Closed, elevator.DoorState);
        }

        [Fact]
        public void MoveToNextFloor_Should_Move_Elevator_To_Next_Floor()
        {
            //arrange
            var elevator = new Elevator();

            //action
            elevator.AddFloorRequest(new FloorRequest(5, 2));

            //assert
            Assert.False(elevator.IsMoving);
            Assert.Equal(0, elevator.CurrentFloor);
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
            Assert.Equal(enElevatorDoorState.Closed, elevator.DoorState);
        }
    }
}
