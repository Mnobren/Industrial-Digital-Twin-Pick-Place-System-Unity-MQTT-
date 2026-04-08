using Project.Cell;
using UnityEngine;

namespace Project.Cell.States
{
    public class Booting : CellState
    {
        public Booting(CellManager manager) : base(manager) { }

        public override DeviceStatus Status => DeviceStatus.RUN;
    }
}