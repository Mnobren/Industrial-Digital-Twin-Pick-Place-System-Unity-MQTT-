using Project.Simulation;
using UnityEngine;

namespace Project.Simulation.Conveyor.States
{
    public class Stopped : ConveyorState
    {
        public Stopped(ConveyorController conveyor) : base(conveyor) { }

        public override DeviceStatus Status => DeviceStatus.IDLE;
    }
}