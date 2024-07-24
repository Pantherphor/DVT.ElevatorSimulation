# Elevator System Challenge

## Overview

This project is a simulation of an elevator control system designed to handle multiple elevators in a building. It allows elevators to service floor requests efficiently and ensures that all elevators can handle requests simultaneously without recursive functions, thereby avoiding potential stack overflow issues.

## Features

- Handles multiple elevators.
- Efficiently assigns floor requests to the nearest available elevator.
- Ensures that elevators move to the calling floor first and then to the target floor.
- Uses an iterative approach to process floor requests.
- Logs elevator status changes, including door state changes and passenger count changes.

## Architecture

The project is structured using clean architecture principles to ensure testability, robustness, scalability, and extendability. The main components are:

- `IElevator`: Interface representing an elevator.
- `IElevatorMover`: Interface for the elevator mover responsible for moving the elevator.
- `IElevatorSystem`: Interface for the elevator system managing multiple elevators.
- `ElevatorMover`: Class implementing the `IElevatorMover` interface, handling the movement of elevators.
- `ElevatorSystem`: Class implementing the `IElevatorSystem` interface, managing the entire elevator system.
- `ConsoleController`: Class handling user input and output through the console.
- `ElevatorControlUseCase`: Class implementing the use cases for elevator control.

## Getting Started

### Prerequisites

- .NET SDK (version 5.0 or later)

### Installation

1. Clone the repository:

```sh
https://github.com/Pantherphor/DVT.ElevatorSimulation.git
```

### How to run
1. `dotnet run`

### Sample Console
```sh

Enter the floor number you currently on: 3
Enter the floor number to take you: 7
Enter the number of passengers: 1

Elevator status:
| Elevator | Calling Floor | Current Floor | Target Floor | Direction | Passenger Count |  Moving   |
| -------- | ------------- | ------------- | ------------ | --------- | --------------- | --------- |
| 1        | 3             | 1             | 7            | Up        | 1               | True      |
| 2        | 0             | 0             | 0            | None      | 0               | False     |

Enter calling floor: q

Exiting the Elevator System. Goodbye!
```
