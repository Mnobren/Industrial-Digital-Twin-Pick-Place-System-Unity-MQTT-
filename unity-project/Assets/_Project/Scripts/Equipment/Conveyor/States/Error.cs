using Project.Simulation;

namespace Project.Simulation.Conveyor.States
{
    public class Error : ConveyorState
    {
        public Error(ConveyorController conveyor) : base(conveyor) { }

        public override DeviceStatus Status => DeviceStatus.FAULT;
    }
}