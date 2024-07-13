using ElevatorSimulation.Domain.Enums;
using System;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevatorEvents
    {
        event Action<int, string> ElevatorDoorStateChanged;
        event Action<int, int, int, int> ElevatorPassangerCountChanged;
        event Action<int, enElevatorDirection, int, int, int, bool> ElevatorStatusChanged;
    }
}
