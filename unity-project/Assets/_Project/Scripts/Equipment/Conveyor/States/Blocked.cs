using Project.Simulation;
using UnityEngine;

namespace Project.Simulation.Conveyor.States
{
    public class Blocked : ConveyorState
    {
        public Blocked(ConveyorController conveyor) : base(conveyor) { }

        public override DeviceStatus Status => DeviceStatus.WAIT;

        public override void Update()
        {
            base.Update();
        }
    }
}