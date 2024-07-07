using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorSimulation.Domain.Entities
{
    public interface IElevatorSystem
    {
        void CallElevator(int floor, int passengerCount);
        void MoveElevator(int elevatorId, int floor);
        List<ElevatorStatus> GetElevatorStatus();
    }

    public class ElevatorSystem
    {
        private readonly List<Elevator> elevators;
        private readonly int maxPassengerLimit;

        public ElevatorSystem(int numberOfElevators, int maxPassengerLimit)
        {
            this.maxPassengerLimit = maxPassengerLimit;
            elevators = new List<Elevator>();

            for (int i = 0; i < numberOfElevators; i++)
            {
                elevators.Add(new Elevator { Id = i + 1, CurrentFloor = 0, PassengerCount = 0, IsMoving = false });
            }
        }

        public void CallElevator(int floor, int passengerCount)
        {
            var nearestElevator = elevators.OrderBy(e => Math.Abs(e.CurrentFloor - floor))
                                            .ThenBy(e => e.PassengerCount)
                                            .FirstOrDefault();

            if (nearestElevator != null)
            {
                nearestElevator.AddFloorRequest(floor);
                nearestElevator.PassengerCount += passengerCount;

                if (nearestElevator.PassengerCount > maxPassengerLimit)
                {
                    // Handle overloading by moving excess passengers to another elevator or other logic
                    int excessPassengers = nearestElevator.PassengerCount - maxPassengerLimit;
                    nearestElevator.PassengerCount = maxPassengerLimit;
                    CallElevator(floor, excessPassengers);
                }
            }
        }

        public void MoveElevator(int elevatorId, int floor)
        {
            var elevator = elevators.FirstOrDefault(e => e.Id == elevatorId);

            if (elevator != null)
            {
                elevator.AddFloorRequest(floor);
            }
        }

        public List<ElevatorStatus> GetElevatorStatus()
        {
            return elevators.Select(e => new ElevatorStatus
            {
                Id = e.Id,
                CurrentFloor = e.CurrentFloor,
                Direction = e.Direction,
                IsMoving = e.IsMoving,
                PassengerCount = e.PassengerCount
            }).ToList();
        }
    }
}
