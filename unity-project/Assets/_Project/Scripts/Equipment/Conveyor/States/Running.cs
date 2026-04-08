using UnityEngine;

namespace Project.Simulation.Conveyor.States
{
    public class Running : ConveyorState
    {
        public Running(ConveyorController conveyor) : base(conveyor) { }

        public override DeviceStatus Status => DeviceStatus.RUN;

        public override void Update()
        {
            conveyor.Move();
        }
    }
}