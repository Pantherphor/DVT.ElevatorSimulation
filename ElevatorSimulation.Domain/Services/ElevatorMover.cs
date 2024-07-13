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
    public interface IElevatorMover : IElevatorEvents
    {
        Task MoveToNextFloorAsync();
        IElevatorMover Initialize(IElevator elevator);
    }

    public class ElevatorMover : IElevatorMover
    {
        private IElevator elevator;

        public event Action<int, string> ElevatorDoorStateChanged;
        public event Action<int, enElevatorDirection, int, int, int, bool> ElevatorStatusChanged;
        public event Action<int, int, int, int> ElevatorPassangerCountChanged;

        public IElevatorMover Initialize(IElevator elevator)
        {
            this.elevator = elevator;
            return this;
        }

        public async Task MoveToNextFloorAsync()
        {
            var floorRequest = elevator.FloorRequests.FirstOrDefault();
            elevator.RemoveFloorRequest(floorRequest);

            if (floorRequest != null)
            {
                elevator.PassengerCount = floorRequest.PassengerCount;
                int callingFloor = floorRequest.CallingFloor;
                int targetFloor = floorRequest.TargetFloor;
                if (!elevator.IsMoving && elevator.CurrentFloor != targetFloor)
                {
                    elevator.Direction = targetFloor > elevator.CurrentFloor ? enElevatorDirection.Up : enElevatorDirection.Down;
                    elevator.IsMoving = true;
                    OnElevatorStatusChanged(callingFloor, targetFloor);

                    int step = elevator.Direction == enElevatorDirection.Up ? 1 : -1;
                    while (elevator.CurrentFloor != targetFloor)
                    {
                        await Task.Delay(1000); // Simulate time to move one floor
                        elevator.CurrentFloor += step;
                        if (elevator.CurrentFloor != targetFloor)
                        {
                            OnElevatorStatusChanged(callingFloor, targetFloor);
                        }
                    }

                    elevator.IsMoving = false;
                    OnElevatorStatusChanged(callingFloor, targetFloor);
                    await OpenAndCloseDoorAsync(floorRequest);
                    elevator.RemoveFloorRequest(floorRequest);
                    await MoveToNextFloorAsync();
                }
            }
        }

        internal async Task OpenAndCloseDoorAsync(FloorRequest floorRequest)
        {
            elevator.Direction = enElevatorDirection.None;

            OnElevatorDoorStatusChanged(enElevatorDoorState.Openning);
            await Task.Delay(2000); // Simulate door open time
            OnElevatorDoorStatusChanged(enElevatorDoorState.Opened);

            await OffLoadPassengersAsync(floorRequest);
            await LoadPassengersAsync(floorRequest);

            OnElevatorDoorStatusChanged(enElevatorDoorState.Closing);
            await Task.Delay(2000); // Simulate door close time
            OnElevatorDoorStatusChanged(enElevatorDoorState.Closed);
        }

        internal Task OffLoadPassengersAsync(FloorRequest floorRequest)
        {
            // Logic to offload passengers
            if (elevator.CurrentFloor == floorRequest.TargetFloor)
            {
                elevator.PassengerCount -= floorRequest.PassengerCount;
                OnElevatorStatusChanged(floorRequest.CallingFloor, floorRequest.TargetFloor);
                if (elevator.PassengerCount < 0)
                {
                    elevator.PassengerCount = 0;
                }
            }
            return Task.CompletedTask;
        }

        internal Task LoadPassengersAsync(FloorRequest floorRequest)
        {
            //TODO: we need to check for the next trip in the FloorRequests list
            if (elevator.CurrentFloor == floorRequest.TargetFloor)
            {
                if (elevator.IsFull(floorRequest.PassengerCount))
                {
                    //handle Excess passagers here
                    OnElevatorPassangerChanges(floorRequest);
                }
                else
                {
                    elevator.PassengerCount += floorRequest.PassengerCount;
                    OnElevatorStatusChanged(floorRequest.CallingFloor, floorRequest.TargetFloor);
                }
            }
            OnElevatorStatusChanged(floorRequest.CallingFloor, floorRequest.TargetFloor);
            return Task.CompletedTask;
        }

        private void OnElevatorPassangerChanges(FloorRequest floorRequest)
        {
            var excessPassangers = elevator.GetExcessPassangers(floorRequest.PassengerCount);
            ElevatorPassangerCountChanged?.Invoke(elevator.Id, floorRequest.CallingFloor, floorRequest.TargetFloor, excessPassangers);
        }

        private void OnElevatorDoorStatusChanged(enElevatorDoorState elevatorState)
        {
            ElevatorDoorStateChanged?.Invoke(elevator.Id, $"Elevator {elevator.Id} doors {elevatorState} at floor {elevator.CurrentFloor}");
        }

        private void OnElevatorStatusChanged(int callingFloor, int targetFloor)
        {
            ElevatorStatusChanged?.Invoke(elevator.Id, elevator.Direction, callingFloor, elevator.CurrentFloor, targetFloor, elevator.IsMoving);
        }

    }
}