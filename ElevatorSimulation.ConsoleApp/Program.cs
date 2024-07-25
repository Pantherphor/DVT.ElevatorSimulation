using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Services;
using ElevatorSimulation.InterfaceAdapters;
using ElevatorSimulation.InterfaceAdapters.Adapters;
using ElevatorSimulation.Services;
using System.Collections.Generic;

namespace ElevatorSimulation.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var elevators = new List<IElevator>
            {
                new Elevator(new ElevatorMover()) { Id = 1, MaxPassengerLimit = 10 },
                new Elevator(new ElevatorMover()) { Id = 2, MaxPassengerLimit = 10 },
            };
            var elevatorSystem = new ElevatorSystem(elevators);
            var building = new Building(elevatorSystem);
            var useCase = new ElevatorControlUseCase(building);
            var console = new ConsoleAdapter();
            var controller = new ConsoleController(useCase, console);

            controller.Run();
        }
    }
}
