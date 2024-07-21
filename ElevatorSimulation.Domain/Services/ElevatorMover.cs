using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ElevatorSimulation.Application.Tests")]
namespace ElevatorSimulation.Domain.Services
{
    public interface IElevatorMover : IElevatorMoverEvents
    {
        Task MoveToNextFloorAsync();
        IElevatorMover Initialize(IElevator elevator);
    }

    public class ElevatorMover : IElevatorMover
    {
        private IElevator elevator;

        public event Action<int, enElevatorDirection, int, int, int, bool, enElevatorDoorState> ElevatorStatusChanged;
        public event Action<int, int, int, int> ElevatorPassangerCountChanged; //TODO: try moving this to the elevator class

        public IElevatorMover Initialize(IElevator elevator)
        {
            this.elevator = elevator;
            return this;
        }

        public async Task MoveToNextFloorAsync()
        {
            while (elevator.FloorRequests.Any())
            {
                var floorRequest = elevator.FloorRequests.FirstOrDefault();
                if (floorRequest == null) break;

                elevator.RemoveFloorRequest(floorRequest);

                int callingFloor = floorRequest.CallingFloor;
                elevator.SetCallingFloor(callingFloor);

                // Move to the calling floor first
                if (elevator.CurrentFloor != callingFloor)
                {
                    await MoveToFloorAsync(callingFloor);
                    await OpenAndCloseDoorAsync(floorRequest);
                }

                int targetFloor = floorRequest.TargetFloor;
                // Move to the target floor
                if (elevator.CurrentFloor != targetFloor)
                {
                    await MoveToFloorAsync(targetFloor);
                    await OpenAndCloseDoorAsync(floorRequest);
                }
            }
        }

        private async Task MoveToFloorAsync(int destinationFloor)
        {
            if (elevator.CurrentFloor == destinationFloor) return;

            elevator.Direction = getElevatorDirection(destinationFloor);
            elevator.IsMoving = true;
            OnElevatorStatusChanged(elevator.CallingFloor, destinationFloor, enElevatorDoorState.Closed);

            int step = elevator.Direction == enElevatorDirection.Up ? 1 : -1;
            while (elevator.CurrentFloor != destinationFloor)
            {
                await Task.Delay(1000); // Simulate time to move one floor
                elevator.IncrementCurrentFloor(step);
                OnElevatorStatusChanged(elevator.CallingFloor, destinationFloor, enElevatorDoorState.Closed);
            }

            elevator.IsMoving = false;
            OnElevatorStatusChanged(elevator.CallingFloor, destinationFloor, enElevatorDoorState.Closed);
        }

        internal async Task OpenAndCloseDoorAsync(FloorRequest floorRequest)
        {
            elevator.Direction = enElevatorDirection.None;

            OnElevatorStatusChanged(elevator.CallingFloor, floorRequest.TargetFloor, enElevatorDoorState.Openning);
            await Task.Delay(2000); // Simulate door open time
            OnElevatorStatusChanged(elevator.CallingFloor, floorRequest.TargetFloor, enElevatorDoorState.Opened);

            await OffLoadPassengersAsync(floorRequest);
            await LoadPassengersAsync(floorRequest);

            OnElevatorStatusChanged(elevator.CallingFloor, floorRequest.TargetFloor, enElevatorDoorState.Closing);
            await Task.Delay(2000); // Simulate door close time
            OnElevatorStatusChanged(elevator.CallingFloor, floorRequest.TargetFloor, enElevatorDoorState.Closed);

            elevator.Direction = getElevatorDirection(floorRequest.TargetFloor);
        }

        private enElevatorDirection getElevatorDirection(int targetFloor)
        {
            return targetFloor > elevator.CurrentFloor ? enElevatorDirection.Up : enElevatorDirection.Down;
        }

        internal Task OffLoadPassengersAsync(FloorRequest floorRequest)
        {
            // Logic to offload passengers
            if (elevator.CurrentFloor == floorRequest.TargetFloor)
            {
                elevator.DecrementPassengerCount(floorRequest.PassengerCount);
                if (elevator.PassengerCount < 0)
                {
                    elevator.ResetPassengerCount();
                }
            }
            return Task.CompletedTask;
        }

        internal Task LoadPassengersAsync(FloorRequest floorRequest)
        {
            //TODO: we need to check for the next trip in the FloorRequests list
            if (elevator.CurrentFloor == floorRequest.CallingFloor)
            {
                if (elevator.IsFull(floorRequest.PassengerCount))
                {
                    //handle Excess passagers here
                    OnElevatorPassangerChanges(floorRequest);
                }
                else
                {
                    elevator.IncrementPassengerCount(floorRequest.PassengerCount);
                    OnElevatorStatusChanged(floorRequest.CallingFloor, floorRequest.TargetFloor, enElevatorDoorState.Opened);
                }
            }
            return Task.CompletedTask;
        }

        private void OnElevatorPassangerChanges(FloorRequest floorRequest)
        {
            var excessPassangers = elevator.GetExcessPassangers(floorRequest.PassengerCount);
            ElevatorPassangerCountChanged?.Invoke(elevator.Id, floorRequest.CallingFloor, floorRequest.TargetFloor, excessPassangers);
        }

        private void OnElevatorStatusChanged(int callingFloor, int targetFloor, enElevatorDoorState elevatorState)
        {
            ElevatorStatusChanged?.Invoke(elevator.Id, elevator.Direction, callingFloor, elevator.CurrentFloor, targetFloor, elevator.IsMoving, elevatorState);
        }

    }
}