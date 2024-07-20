namespace ElevatorSimulation.InterfaceAdapters
{
    public static class ConsoleControllerConstants
    {
        public const string MenuPrompt = "Select an option:";
        public const string CallElevatorOption = "1. Call Elevator";
        public const string MoveElevatorOption = "2. Move Elevator";
        public const string ShowElevatorStatusOption = "3. Show Elevator Status";
        public const string QuitOption = "q. Quit";
        public const string InvalidOptionMessage = "Invalid option. Please try again.";
        public const string EnterFloorNumberMessage = "Enter the floor number you currently on: ";
        public const string EnterTargetFloorNumberMessage = "Enter the floor number to take you: ";
        public const string EnterPassengerCountMessage = "Enter the number of passengers: ";
        public const string InvalidFloorNumberMessage = "Invalid floor number.";
        public const string InvalidPassengerCountMessage = "Invalid number of passengers.";
        public const string EnterElevatorIdMessage = "Enter the elevator ID: ";
        public const string EnterMoveToFloorNumberMessage = "Enter the floor number to move to: ";
        public const string InvalidElevatorIdMessage = "Invalid elevator ID.";
        public const string ElevatorStatusMessageFormat = "Elevator {0}: Floor {1}, Direction {2}, {3}, Passengers {4}";
        public const string MovingStatus = "Moving";
        public const string StationaryStatus = "Stationary";
    }

}
