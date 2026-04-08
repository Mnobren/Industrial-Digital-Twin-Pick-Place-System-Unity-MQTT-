public enum DeviceStatus
{
    IDLE,
    RUN,
    WAIT,
    FAULT
}

namespace Project.StateMachine
{
    public interface IState
    {
        public abstract DeviceStatus Status { get; }
        void Enter();
        void Update();
        void Exit();
    }
}