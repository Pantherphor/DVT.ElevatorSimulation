using ElevatorSimulation.Domain.Enums;
using System;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevatorMoverEvents
    {
        event Action<int, string> ElevatorDoorStateChanged;
        event Action<int, enElevatorDirection, int, int, int, bool> ElevatorStatusChanged;
    }
}
