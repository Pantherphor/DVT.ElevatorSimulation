using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly22")]
namespace ElevatorSimulation.Domain.Services
{
    public class Elevator : IElevator
    {
        public int Id { get; set; }
        public int CurrentFloor { get; internal set; }
        public int PassengerCount { get; internal set; }
        public int CallingFloor { get; internal set; }

        public bool IsMoving { get; set; }
        public int MaxPassengerLimit { get; set; }
        public IList<FloorRequest> FloorRequests { get; }
        
        public enElevatorDoorState DoorState { get; private set; }
        public enElevatorDirection Direction { get; set; }

        private readonly IElevatorMover elevatorMover;

        public event Action<FloorRequest> ElevatorRequestOverloadChanged;

        public Elevator(IElevatorMover elevatorMover)
        {
            CurrentFloor = 0;
            PassengerCount = 0;
            FloorRequests = new List<FloorRequest>();
            DoorState = enElevatorDoorState.Closed;
            Direction = enElevatorDirection.None;

            this.elevatorMover = elevatorMover.Initialize(this);
        }

        public IElevatorMover ElevatorMover => this.elevatorMover;


        public void AddFloorRequest(FloorRequest floorRequest)
        {
            if (!IsFull(floorRequest.PassengerCount))
            {
                FloorRequests.Add(floorRequest);
            }
            else
            {
                ElevatorRequestOverloadChanged?.Invoke(floorRequest);
            }
        }

        public async Task MoveToNextFloorAsync()
        {
            await elevatorMover.MoveToNextFloorAsync();  
        }

        public void RemoveFloorRequest(FloorRequest floorRequest)
        {
            FloorRequests.Remove(floorRequest);
        }

        public ElevatorStatus GetElevatorStatus()
        {
            return new ElevatorStatus
            {
                Id = Id,
                CurrentFloor = CurrentFloor,
                Direction = Direction,
                IsMoving = IsMoving,
                PassengerCount = PassengerCount
            };
        }

        public bool IsFull(int passangerCount)
        {
            return (PassengerCount + passangerCount) > MaxPassengerLimit;
        }

        public int GetExcessPassangers(int passangerCount)
        {
            return (PassengerCount + passangerCount) - MaxPassengerLimit;
        }

        public int IncrementCurrentFloor(int step)
        {
            return CurrentFloor += step;
        }

        public void DecrementPassengerCount(int passengerCount)
        {
            PassengerCount -= passengerCount;
        }

        void IElevator.IncrementPassengerCount(int passengerCount)
        {
            PassengerCount += passengerCount;
        }

        public void ResetPassengerCount()
        {
            PassengerCount = 0;
        }

        public void SetCallingFloor(int callingFloor)
        {
            CallingFloor = callingFloor;
        }

    }
}
