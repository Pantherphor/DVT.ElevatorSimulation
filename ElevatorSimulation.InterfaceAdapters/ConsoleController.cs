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
                        Console.WriteLine(ConsoleConstants.InvalidOptionMessage);
                        break;
                }
            }
        }

        internal static void DisplayMenu()
        {
            Console.WriteLine(ConsoleConstants.MenuPrompt);
            Console.WriteLine(ConsoleConstants.CallElevatorOption);
            Console.WriteLine(ConsoleConstants.MoveElevatorOption);
            Console.WriteLine(ConsoleConstants.ShowElevatorStatusOption);
            Console.WriteLine(ConsoleConstants.QuitOption);
        }

        internal async void CallElevator()
        {
            Console.Write(ConsoleConstants.EnterFloorNumberMessage);
            if (int.TryParse(Console.ReadLine(), out int callingFloor))
            {
                Console.Write(ConsoleConstants.EnterTargetFloorNumberMessage);
                if (int.TryParse(Console.ReadLine(), out int targetFloor))
                {
                    Console.Write(ConsoleConstants.EnterPassengerCountMessage);
                    if (int.TryParse(Console.ReadLine(), out int passengers))
                    {
                        await elevatorControlUseCase.CallElevatorAsync(new FloorRequest(callingFloor, targetFloor, passengers));
                    }
                    else
                    {
                        Console.WriteLine(ConsoleConstants.InvalidPassengerCountMessage);
                    }
                }
                else
                {
                    Console.WriteLine(ConsoleConstants.InvalidFloorNumberMessage);
                }
            }
            else
            {
                Console.WriteLine(ConsoleConstants.InvalidFloorNumberMessage);
            }
        }

        internal async void MoveElevator()
        {
            Console.Write(ConsoleConstants.EnterElevatorIdMessage);
            if (int.TryParse(Console.ReadLine(), out int elevatorId))
            {
                Console.Write(ConsoleConstants.EnterMoveToFloorNumberMessage);
                if (int.TryParse(Console.ReadLine(), out int floor))
                {
                    await elevatorControlUseCase.MoveElevatorAsync(elevatorId, floor);
                }
                else
                {
                    Console.WriteLine(ConsoleConstants.InvalidFloorNumberMessage);
                }
            }
            else
            {
                Console.WriteLine(ConsoleConstants.InvalidElevatorIdMessage);
            }
        }

        internal void ShowElevatorStatus()
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
                        ConsoleConstants.ElevatorStatusMessageFormat,
                        status.Id,
                        status.CurrentFloor,
                        status.Direction,
                        status.DoorState,
                        status.IsMoving ? ConsoleConstants.MovingStatus : ConsoleConstants.StationaryStatus,
                        status.PassengerCount
                    )
                );
            }
        }

        private static void DisplayElevatorHistory(IDictionary<int, IList<ElevatorMovementHistory>> elevatorHistory)
        {
            DisplayHistoryTableHeader();
            foreach (var history in elevatorHistory.Values.SelectMany(h => h))
            {
                Console.WriteLine($"| {history.ElevatorId,8} | {history.CallingFloor,13} | {history.CurrentFloor,13} | {history.TargetFloor,12} | {history.Direction,-9} | {history.PassengerCount,10} | {history.IsMoving,6} | {history.DoorState,11} | {history.Timestamp:HH:mm:ss} |");
            }
            Console.WriteLine(ConsoleConstants.TableHistorySeparator);
        }

        private static void DisplayHistoryTableHeader()
        {
            Console.WriteLine(ConsoleConstants.TableHistorySeparator);
            Console.WriteLine(ConsoleConstants.TableHistoryHeader);
            Console.WriteLine(ConsoleConstants.TableHistorySeparator);
        }

    }

}
