using ElevatorSimulation.Domain.Enums;
using ElevatorSimulation.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorSimulation.Domain.Entities
{
    public class Elevator : IElevator
    {
        public int CurrentFloor { get; set; }
        public int Id { get; set; }
        public int PassengerCount { get; set; }
        public bool IsMoving { get; set; }
        public int MaxPassengerLimit { get; set; }

        public enElevatorDoorState DoorState { get; private set; }
        public enElevatorDirection Direction { get; set; }


        public IList<FloorRequest> FloorRequests { get; }

        public Elevator()
        {
            CurrentFloor = 0;
            FloorRequests = new List<FloorRequest>();
            DoorState = enElevatorDoorState.Closed;
            Direction = enElevatorDirection.None;
        }


        public void AddFloorRequest(FloorRequest targetFloor)
        {
            FloorRequests.Add(targetFloor);
        }

        public async Task MoveToNextFloorAsync()
        {
            if (FloorRequests.Any())
            {
                var floorRequest = FloorRequests.FirstOrDefault();
                PassengerCount = floorRequest.PassengerCount;
                int targetFloor = floorRequest.TargetFloor;
                if (!IsMoving && CurrentFloor != targetFloor)
                {
                    Direction = targetFloor > CurrentFloor ? enElevatorDirection.Up : enElevatorDirection.Down;
                    IsMoving = true;

                    int step = Direction == enElevatorDirection.Up ? 1 : -1;
                    while (CurrentFloor != targetFloor)
                    {
                        await Task.Delay(1000); // Simulate time to move one floor
                        CurrentFloor += step;
                        if (CurrentFloor != targetFloor)
                        {
                            await HandlePassengersAtCurrentFloor();
                        }
                    }

                    IsMoving = false;
                }
            }
            
        }

        private async Task HandlePassengersAtCurrentFloor()
        {
            var offloadingRequests = FloorRequests.Where(r => r.TargetFloor == CurrentFloor).ToList();

            foreach (var request in offloadingRequests)
            {
                await OffloadPassengers(request);
            }
        }

        private async Task OffloadPassengers(FloorRequest floorRequest)
        {
            await Task.Delay(2000); // Simulate door opened time while offloading

            if (CurrentFloor == floorRequest.TargetFloor)
            {
                PassengerCount -= floorRequest.PassengerCount;
                RemoveFloorRequest(floorRequest);
                if (PassengerCount < 0)
                {
                    PassengerCount = 0;
                }
            }
        }

        private void RemoveFloorRequest(FloorRequest floorRequest)
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

        public bool IsFull()
        {
            return PassengerCount > MaxPassengerLimit;
        }

        public int GetExcessPassangers()
        {
            return PassengerCount - MaxPassengerLimit;
        }
    }
}
