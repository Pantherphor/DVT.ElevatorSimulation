using ElevatorSimulation.Domain.Interfaces;

namespace ElevatorSimulation.Domain.Entities
{
    public class Building
    {
        public IElevatorSystem ElevatorSystem { get; }

        public Building(IElevatorSystem elevatorSystem)
        {
            ElevatorSystem = elevatorSystem;
        }
    }
}
