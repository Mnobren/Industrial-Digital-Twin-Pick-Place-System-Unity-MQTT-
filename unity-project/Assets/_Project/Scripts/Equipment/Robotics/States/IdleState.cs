using UnityEngine;

namespace Project.Robotics.States
{
    public class IdleState : RobotState
    {
        public IdleState(RobotController robot) : base(robot) { }

        public override DeviceStatus Status => DeviceStatus.IDLE;

        public override void Enter()
        {
            robot.motion.StopMovement();
        }
    }
}
