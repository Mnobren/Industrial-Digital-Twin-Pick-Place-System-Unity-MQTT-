namespace Project.Robotics.States
{
    public class ErrorState : RobotState
    {
        public ErrorState(RobotController robot) : base(robot) { }

        public override DeviceStatus Status => DeviceStatus.FAULT;

        public override void Enter()
        {
            robot.motion.StopMovement();
        }

        public override void Update()
        {
            // Aguarda reset manual
            if (robot.IsResetRequested())
            {
                robot.ClearError();
            }
        }
    }
}