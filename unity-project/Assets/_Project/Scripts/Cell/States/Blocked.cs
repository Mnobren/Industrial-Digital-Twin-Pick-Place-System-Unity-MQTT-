using Project.Simulation;
using UnityEngine;

namespace Project.Cell.States
{
    public class Blocked : CellState
    {
        public Blocked(CellManager manager) : base(manager) { }

        public override DeviceStatus Status => DeviceStatus.WAIT;
    }
}