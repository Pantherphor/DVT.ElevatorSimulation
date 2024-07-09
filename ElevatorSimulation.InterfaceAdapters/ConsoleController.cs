using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ElevatorSimulation.InterfaceAdapters.Tests")]
namespace ElevatorSimulation.InterfaceAdapters
{
    public class ConsoleController
    {
        private readonly IElevatorSystem elevatorSystem;

        public ConsoleController(IElevatorSystem elevatorSystem)
        {
            this.elevatorSystem = elevatorSystem;
        }

        public void Run()
        {
            while (true)
            {
                DisplayMenu();
                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        CallElevator();
                        break;
                    case "2":
                        MoveElevator();
                        break;
                    case "3":
                        ShowElevatorStatus();
                        break;
                    case "q":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        internal static void DisplayMenu()
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Call Elevator");
            Console.WriteLine("2. Move Elevator");
            Console.WriteLine("3. Show Elevator Status");
            Console.WriteLine("q. Quit");
        }

        internal void CallElevator()
        {
            Console.Write("Enter the floor number: ");
            if (int.TryParse(Console.ReadLine(), out int floor))
            {
                Console.Write("Enter the number of passengers: ");
                if (int.TryParse(Console.ReadLine(), out int passengers))
                {
                    elevatorSystem.CallElevator(new FloorRequest(floor, passengers));
                }
                else
                {
                    Console.WriteLine("Invalid number of passengers.");
                }
            }
            else
            {
                Console.WriteLine("Invalid floor number.");
            }
        }

        internal void MoveElevator()
        {
            Console.Write("Enter the elevator ID: ");
            if (int.TryParse(Console.ReadLine(), out int elevatorId))
            {
                Console.Write("Enter the floor number to move to: ");
                if (int.TryParse(Console.ReadLine(), out int floor))
                {
                    elevatorSystem.MoveElevator(elevatorId, floor);
                }
                else
                {
                    Console.WriteLine("Invalid floor number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid elevator ID.");
            }
        }

        internal void ShowElevatorStatus()
        {
            var statuses = elevatorSystem.GetElevatorStatus();
            foreach (var status in statuses)
            {
                Console.WriteLine($"Elevator {status.Id}: Floor {status.CurrentFloor}, Direction {status.Direction}, " +
                                  $"{(status.IsMoving ? "Moving" : "Stationary")}, Passengers {status.PassengerCount}");
            }
        }
    }
}
