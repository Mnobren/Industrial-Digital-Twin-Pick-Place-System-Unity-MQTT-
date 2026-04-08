using Project.StateMachine;

namespace Project.Robotics
{
    public abstract class RobotState : IState
    {
        protected RobotController robot;

        public abstract DeviceStatus Status { get; }

        protected RobotState(RobotController robot)
        {
            this.robot = robot;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}
