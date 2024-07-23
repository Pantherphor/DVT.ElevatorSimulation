using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        internal void CallElevator()//TODO: Made async
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
                        elevatorControlUseCase.CallElevatorAsync(new FloorRequest(callingFloor, targetFloor, passengers));
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

        internal void MoveElevator()//TODO: Made async
        {
            Console.Write(ConsoleControllerConstants.EnterElevatorIdMessage);
            if (int.TryParse(Console.ReadLine(), out int elevatorId))
            {
                Console.Write(ConsoleControllerConstants.EnterMoveToFloorNumberMessage);
                if (int.TryParse(Console.ReadLine(), out int floor))
                {
                    elevatorControlUseCase.MoveElevatorAsync(elevatorId, floor);
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

        internal void ShowElevatorStatus() //TODO: Made async
        {
            var elevatorHistory = elevatorControlUseCase.GetElevatorMovementHistory();
            DisplayElevatorHistory(elevatorHistory);
            
            DisplayElecatorStatusSummery();
        }

        private void DisplayElecatorStatusSummery()
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
                        status.DoorState,
                        status.IsMoving ? ConsoleControllerConstants.MovingStatus : ConsoleControllerConstants.StationaryStatus,
                        status.PassengerCount
                    )
                );
            }
        }

        private void DisplayElevatorHistory(IDictionary<int, IList<ElevatorMovementHistory>> elevatorHistory)
        {
            DisplayHistoryTableHeader();
            foreach (var history in elevatorHistory.Values.SelectMany(h => h))
            {
                Console.WriteLine($"| {history.ElevatorId,8} | {history.CallingFloor,13} | {history.CurrentFloor,13} | {history.TargetFloor,12} | {history.Direction,-9} | {history.PassengerCount,10} | {history.IsMoving,6} | {history.DoorState,11} | {history.Timestamp:HH:mm:ss} |");
            }
            Console.WriteLine(ConsoleControllerConstants.TableHistorySeparator);
        }

        private void DisplayHistoryTableHeader()
        {
            Console.WriteLine(ConsoleControllerConstants.TableHistorySeparator);
            Console.WriteLine(ConsoleControllerConstants.TableHistoryHeader);
            Console.WriteLine(ConsoleControllerConstants.TableHistorySeparator);
        }

    }

}
