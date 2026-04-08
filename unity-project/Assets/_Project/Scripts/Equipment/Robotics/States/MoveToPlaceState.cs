using UnityEngine;

namespace Project.Robotics.States
{
    public class MoveToPlaceState : RobotState
    {
        public MoveToPlaceState(RobotController robot) : base(robot) { }

        public override DeviceStatus Status => DeviceStatus.RUN;

        public override void Enter()
        {
            robot.MoveToPlace(() => {robot.SetPlaceState();} );
        }
    }
}
