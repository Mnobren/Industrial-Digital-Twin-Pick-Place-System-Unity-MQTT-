using Project.Cell;
using UnityEngine;

namespace Project.Robotics.States
{
    public class PickState : RobotState
    {
        public CellManager cellManager { get; private set; }

        private float timer;
        private float pickDuration = 1.0f;

        public PickState(RobotController robot) : base(robot) { }

        public override DeviceStatus Status => DeviceStatus.RUN;

        public override void Enter()
        {
            timer = 0f;
        }

        public override void Update()
        {
            timer += Time.deltaTime;

            if (timer >= pickDuration)
            {
                robot.RefreshWorkpiecesInRange();

                if (!robot.HasPartAtPickPosition() || robot.WorkpiecesInRange.Count == 0)
                {
                    robot.TriggerError("Pick failed: no part detected");
                    return;
                }

                robot.AttachPart();
                robot.SetMoveToPlaceState();
            }
        }

        public override void Exit()
        {
            timer = 0f;
        }
    }
}
