using UnityEngine;

namespace Project.Cell.States
{
    public class Running : CellState
    {
        public Running(CellManager manager) : base(manager) { }

        public override DeviceStatus Status => DeviceStatus.RUN;
    }
}