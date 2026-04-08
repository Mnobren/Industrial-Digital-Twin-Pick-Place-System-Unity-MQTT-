using Project.Cell;
using UnityEngine;

namespace Project.Cell.States
{
    public class Idle : CellState
    {
        public Idle(CellManager manager) : base(manager) { }

        public override DeviceStatus Status => DeviceStatus.IDLE;
    }
}