using Project.Cell;
using Project.StateMachine;

namespace Project.Cell
{
    public abstract class CellState : IState
    {
        protected CellManager manager;

        public abstract DeviceStatus Status { get; }

        protected CellState(CellManager manager)
        {
            this.manager = manager;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}