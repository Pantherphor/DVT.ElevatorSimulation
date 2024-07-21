using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Services;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorSimulation.Domain.Tests
{
    public class ElevatorTests
    {
        [Fact]
        public void Elevator_Should_Initialize_With_Correct_Values()
        {
            //arrange
            var mockElevatorMover = new Mock<IElevatorMover>();
            var elevator = new Elevator(mockElevatorMover.Object);

            //assert
            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Equal(enElevatorDoorState.Closed, elevator.DoorState);
        }

        [Fact]
        public void AddTargetFloor_Should_Add_Target_Floor_To_Queue()
        {
            //arrange
            var mockElevatorMover = new Mock<IElevatorMover>();
            var elevator = new Elevator(mockElevatorMover.Object);
            
            //action
            elevator.AddFloorRequest(new FloorRequest(0, 5, 2));

            //assert
            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Equal(enElevatorDirection.None, elevator.Direction);
            Assert.Equal(enElevatorDoorState.Closed, elevator.DoorState);
        }
        
        [Fact]
        public async void MoveToNextFloor_Should_Move_Elevator_To_Next_Floor_No_Request_Should_Not_Fail()
        {
            //arrange
            var mockElevatorMover = new Mock<IElevatorMover>();
            mockElevatorMover.Setup(o => o.Initialize(It.IsAny<IElevator>()))
                     .Returns(mockElevatorMover.Object);
            var elevator = new Elevator(mockElevatorMover.Object);

            //action
            await elevator.MoveToNextFloorAsync();

            //assert
            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Equal(enElevatorDoorState.Closed, elevator.DoorState);
        }

        [Fact]
        public void MoveToNextFloor_Should_Not_Move_Elevator_To_Next_Floor()
        {
            //arrange
            var mockElevatorMover = new Mock<IElevatorMover>();
            var elevator = new Elevator(mockElevatorMover.Object);

            //action
            elevator.AddFloorRequest(new FloorRequest(0, 5, 2));

            //assert
            Assert.False(elevator.IsMoving);
            Assert.Equal(0, elevator.CurrentFloor);
        }

        [Fact]
        public async void MoveToNextFloor_Should_Call_Move_Elevator_To_Next_Floor()
        {
            //arrange
            var mockElevatorMover = new Mock<IElevatorMover>();
            mockElevatorMover.Setup(o => o.Initialize(It.IsAny<IElevator>()))
                     .Returns(mockElevatorMover.Object);
            mockElevatorMover.Setup(o => o.MoveToNextFloorAsync())
                .Returns(Task.CompletedTask);

            var elevator = new Elevator(mockElevatorMover.Object);
            elevator.AddFloorRequest(new FloorRequest(0, 5, 2));
            elevator.AddFloorRequest(new FloorRequest(0, 3, 3));
            elevator.AddFloorRequest(new FloorRequest(0, 8, 4));

            //action
            await elevator.MoveToNextFloorAsync(); //TODO: move this method into it own object to test better and clean code as more is expected to happen here

            //assert
            mockElevatorMover.Verify(o => o.Initialize(elevator), Times.Once);
            mockElevatorMover.Verify(o => o.MoveToNextFloorAsync(), Times.Once);
        }

    }
}
