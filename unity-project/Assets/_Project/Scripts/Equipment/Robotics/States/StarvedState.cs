using UnityEngine;

namespace Project.Robotics.States
{
    public class StarvedState : RobotState
    {
        public StarvedState(RobotController robot) : base(robot) { }

        public override DeviceStatus Status => DeviceStatus.WAIT;

        public override void Enter()
        {
            robot.motion.StopMovement();
        }

        public override void Update()
        {
            if (robot.HasPartAtPickPosition())
            {
                robot.SetMoveToPickState();
            }
        }
    }
}
