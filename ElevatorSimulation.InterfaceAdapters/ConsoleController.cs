using ElevatorSimulation.Application.UseCases;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.InterfaceAdapters.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ElevatorSimulation.InterfaceAdapters.Tests")]
namespace ElevatorSimulation.InterfaceAdapters
{
    public class ConsoleController
    {
        private readonly IElevatorControlUseCase elevatorControlUseCase;
        private readonly IConsole console;

        public ConsoleController(IElevatorControlUseCase elevatorControlUseCase, IConsole console)
        {
            this.elevatorControlUseCase = elevatorControlUseCase;
            this.console = console;
        }

        public void Run()
        {
            while (true)
            {
                DisplayMenu();
                var option = console.ReadLine();

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
                        console.WriteLine(ConsoleConstants.InvalidOptionMessage);
                        break;
                }
            }
        }

        internal void DisplayMenu()
        {
            console.WriteLine(ConsoleConstants.MenuPrompt);
            console.WriteLine(ConsoleConstants.CallElevatorOption);
            console.WriteLine(ConsoleConstants.MoveElevatorOption);
            console.WriteLine(ConsoleConstants.ShowElevatorStatusOption);
            console.WriteLine(ConsoleConstants.QuitOption);
        }

        internal async void CallElevator()
        {
            console.Write(ConsoleConstants.EnterFloorNumberMessage);
            if (int.TryParse(console.ReadLine(), out int callingFloor))
            {
                console.Write(ConsoleConstants.EnterTargetFloorNumberMessage);
                if (int.TryParse(console.ReadLine(), out int targetFloor))
                {
                    console.Write(ConsoleConstants.EnterPassengerCountMessage);
                    if (int.TryParse(console.ReadLine(), out int passengers))
                    {
                        await elevatorControlUseCase.CallElevatorAsync(new FloorRequest(callingFloor, targetFloor, passengers));
                    }
                    else
                    {
                        console.WriteLine(ConsoleConstants.InvalidPassengerCountMessage);
                    }
                }
                else
                {
                    console.WriteLine(ConsoleConstants.InvalidFloorNumberMessage);
                }
            }
            else
            {
                console.WriteLine(ConsoleConstants.InvalidFloorNumberMessage);
            }
        }

        internal async void MoveElevator()
        {
            console.Write(ConsoleConstants.EnterElevatorIdMessage);
            if (int.TryParse(console.ReadLine(), out int elevatorId))
            {
                console.Write(ConsoleConstants.EnterMoveToFloorNumberMessage);
                if (int.TryParse(console.ReadLine(), out int floor))
                {
                    await elevatorControlUseCase.MoveElevatorAsync(elevatorId, floor);
                }
                else
                {
                    console.WriteLine(ConsoleConstants.InvalidFloorNumberMessage);
                }
            }
            else
            {
                console.WriteLine(ConsoleConstants.InvalidElevatorIdMessage);
            }
        }

        internal void ShowElevatorStatus()
        {
            var elevatorHistory = elevatorControlUseCase.GetElevatorMovementHistory();
            DisplayElevatorHistory(elevatorHistory);
            DisplayElevatorStatusSummary();
        }

        private void DisplayElevatorStatusSummary()
        {
            var statuses = elevatorControlUseCase.GetElevatorStatus();
            foreach (var status in statuses)
            {
                console.WriteLine(
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

        private void DisplayElevatorHistory(IDictionary<int, IList<ElevatorMovementHistory>> elevatorHistory)
        {
            DisplayHistoryTableHeader();
            foreach (var history in elevatorHistory.Values.SelectMany(h => h))
            {
                console.WriteLine($"| {history.ElevatorId,8} | {history.CallingFloor,13} | {history.CurrentFloor,13} | {history.TargetFloor,12} | {history.Direction,-9} | {history.PassengerCount,10} | {history.IsMoving,6} | {history.DoorState,11} | {history.Timestamp:HH:mm:ss} |");
            }
            console.WriteLine(ConsoleConstants.TableHistorySeparator);
        }

        private void DisplayHistoryTableHeader()
        {
            console.WriteLine(ConsoleConstants.TableHistorySeparator);
            console.WriteLine(ConsoleConstants.TableHistoryHeader);
            console.WriteLine(ConsoleConstants.TableHistorySeparator);
        }
    }
}
