namespace ElevatorSimulation.Application.Services
{
    public static class ConsoleConstants
    {
        public const string TableSeparator = "------------------------------------------------------------------------------------------------";
        public const string TableHeader = "| Elevator | Calling Floor | Current Floor | Target Floor | Direction | Passengers | IsMoving |";
        public const string DoorClosedHeader = "----------------------------------------------------------------";
        public const string PassangerCountHeader = "----------------------------------------------------------";
        public const string CallElevatorHeader = "----------------------------------------------------------";

        public const string ElevatorIdMessage = "Elevator ID: ";
        public const string EventMessage = "Message: ";
        public const string CallingFloorMessage = "Calling Floor";
        public const string TargetFloorMessage = "Target Floor";
        public const string PassengersMessage = "Passengers";
        public const string DirectionMessage = "Direction";
        public const string IsMovingMessage = "IsMoving";
        public const string InvalidOptionMessage = "Invalid option. Please try again.";
        public const string InvalidFloorNumberMessage = "Invalid floor number.";
        public const string InvalidPassengerCountMessage = "Invalid number of passengers.";
        public const string InvalidElevatorIdMessage = "Invalid elevator ID.";
        public const string InvalidTargetFloorMessage = "Invalid floor number.";
    }

}
