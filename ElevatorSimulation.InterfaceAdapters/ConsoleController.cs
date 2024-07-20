using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ElevatorSimulation.InterfaceAdapters.Tests")]
namespace ElevatorSimulation.InterfaceAdapters
{
    public class ConsoleController
    {
        private readonly IElevatorControlUseCase elevatorControlUseCase;

        public ConsoleController(IElevatorControlUseCase elevatorControlUseCase)
        {
            this.elevatorControlUseCase = elevatorControlUseCase;
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
                        Console.WriteLine(ConsoleControllerConstants.InvalidOptionMessage);
                        break;
                }
            }
        }

        internal static void DisplayMenu()
        {
            Console.WriteLine(ConsoleControllerConstants.MenuPrompt);
            Console.WriteLine(ConsoleControllerConstants.CallElevatorOption);
            Console.WriteLine(ConsoleControllerConstants.MoveElevatorOption);
            Console.WriteLine(ConsoleControllerConstants.ShowElevatorStatusOption);
            Console.WriteLine(ConsoleControllerConstants.QuitOption);
        }

        internal void CallElevator()
        {
            Console.Write(ConsoleControllerConstants.EnterFloorNumberMessage);
            if (int.TryParse(Console.ReadLine(), out int callingFloor))
            {
                Console.Write(ConsoleControllerConstants.EnterTargetFloorNumberMessage);
                if (int.TryParse(Console.ReadLine(), out int targetFloor))
                {
                    Console.Write(ConsoleControllerConstants.EnterPassengerCountMessage);
                    if (int.TryParse(Console.ReadLine(), out int passengers))
                    {
                        elevatorControlUseCase.CallElevator(new FloorRequest(callingFloor, targetFloor, passengers));
                    }
                    else
                    {
                        Console.WriteLine(ConsoleControllerConstants.InvalidPassengerCountMessage);
                    }
                }
                else
                {
                    Console.WriteLine(ConsoleControllerConstants.InvalidFloorNumberMessage);
                }
            }
            else
            {
                Console.WriteLine(ConsoleControllerConstants.InvalidFloorNumberMessage);
            }
        }

        internal void MoveElevator()
        {
            Console.Write(ConsoleControllerConstants.EnterElevatorIdMessage);
            if (int.TryParse(Console.ReadLine(), out int elevatorId))
            {
                Console.Write(ConsoleControllerConstants.EnterMoveToFloorNumberMessage);
                if (int.TryParse(Console.ReadLine(), out int floor))
                {
                    elevatorControlUseCase.MoveElevator(elevatorId, floor);
                }
                else
                {
                    Console.WriteLine(ConsoleControllerConstants.InvalidFloorNumberMessage);
                }
            }
            else
            {
                Console.WriteLine(ConsoleControllerConstants.InvalidElevatorIdMessage);
            }
        }

        internal void ShowElevatorStatus()
        {
            var statuses = elevatorControlUseCase.GetElevatorStatus();
            foreach (var status in statuses)
            {
                Console.WriteLine(
                    string.Format(
                        ConsoleControllerConstants.ElevatorStatusMessageFormat,
                        status.Id,
                        status.CurrentFloor,
                        status.Direction,
                        status.IsMoving ? ConsoleControllerConstants.MovingStatus : ConsoleControllerConstants.StationaryStatus,
                        status.PassengerCount
                    )
                );
            }
        }
    }

}
