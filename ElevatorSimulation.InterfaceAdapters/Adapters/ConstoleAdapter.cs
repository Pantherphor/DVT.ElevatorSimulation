using ElevatorSimulation.InterfaceAdapters.Interfaces;
using System;

namespace ElevatorSimulation.InterfaceAdapters.Adapters
{
    public class ConsoleAdapter : IConsole
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Write(string message)
        {
            Console.Write(message);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }

}
