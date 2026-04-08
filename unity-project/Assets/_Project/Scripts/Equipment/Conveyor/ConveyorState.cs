using Project.StateMachine;

namespace Project.Simulation
{
    public abstract class ConveyorState : IState
    {
        protected ConveyorController conveyor;

        public abstract DeviceStatus Status { get; }

        protected ConveyorState(ConveyorController conveyor)
        {
            this.conveyor = conveyor;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}
