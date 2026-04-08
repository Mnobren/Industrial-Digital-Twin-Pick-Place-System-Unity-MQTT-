using Project.Simulation;
using UnityEngine;

namespace Project.Cell.States
{
    public class Error : CellState
    {
        public Error(CellManager manager) : base(manager) { }

        public override DeviceStatus Status => DeviceStatus.FAULT;
    }
}