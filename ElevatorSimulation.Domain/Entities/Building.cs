using System.Collections.Generic;

namespace ElevatorSimulation.Domain.Entities
{
    public class Building
    {
        public List<Elevator> Elevators { get; private set; }
        public int MinFloor { get; private set; }
        public int MaxFloor { get; private set; }

        public Building(int minFloor, int maxFloor, int elevatorCount)
        {
            MinFloor = minFloor;
            MaxFloor = maxFloor;
            Elevators = new List<Elevator>();

            for (int i = 0; i < elevatorCount; i++)
            {
                Elevators.Add(new Elevator());
            }
        }
    }
}
