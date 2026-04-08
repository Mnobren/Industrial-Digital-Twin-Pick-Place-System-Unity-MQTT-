using UnityEngine;

namespace Project.Robotics.States
{
    public class MoveToPickState : RobotState
    {
        public MoveToPickState(RobotController robot) : base(robot) { }

        public override DeviceStatus Status => DeviceStatus.RUN;

        public override void Enter()
        {
            robot.MoveToPick(() => {robot.SetPickState();} );
        }
    }
}
