using UnityEngine;

namespace Project.Robotics.States
{
    public class PlaceState : RobotState
    {
        private float timer;
        private float placeDuration = 1.0f;

        public PlaceState(RobotController robot) : base(robot) { }

        public override DeviceStatus Status => DeviceStatus.RUN;

        public override void Enter()
        {
            timer = 0f;
        }

        public override void Update()
        {
            timer += Time.deltaTime;

            if (timer >= placeDuration)
            {
                if (!robot.IsHoldingPart())
                {
                    robot.TriggerError("Place failed: no part in gripper");
                    return;
                }

                robot.ReleasePart();
                robot.SetStarved();
            }
        }

        public override void Exit()
        {
            timer = 0f;
        }
    }
}
