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
        public void MoveToNextFloor_Should_Move_Elevator_To_Next_Floor_No_Request_Should_Not_Fail()
        {
            //arrange
            var elevator = new Elevator();

            //action
            elevator.MoveToNextFloorAsync();

            //assert
            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Equal(enElevatorDoorState.Closed, elevator.DoorState);
        }

        [Fact]
        public void MoveToNextFloor_Should_Not_Move_Elevator_To_Next_Floor()
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
        public async void MoveToNextFloor_Should_Move_Elevator_To_Next_Floor_First_Request_Floor()
        {
            //arrange
            var elevator = new Elevator();
            elevator.AddFloorRequest(new FloorRequest(5, 2));
            elevator.AddFloorRequest(new FloorRequest(3, 3));
            elevator.AddFloorRequest(new FloorRequest(8, 4));

            //action
            await elevator.MoveToNextFloorAsync();

            //assert
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(2, elevator.FloorRequests.Count);
            Assert.Equal(0, elevator.PassengerCount);
            Assert.Equal(enElevatorDoorState.Closed, elevator.DoorState);
        }
    }
}
