using System.ComponentModel.Design.Serialization;
using Project.Cell.States;
using Project.KPIs;
using Project.Robotics;
using Project.Robotics.States;
using Project.Sensors;
using Project.StateMachine;
using Project.Simulation;
using Project;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Cell
{
    public class CellManager : MonoBehaviour
    {
        public Project.StateMachine.FiniteStateMachine StateMachine { get; private set; }

        [SerializeField] private KPIManager kpi;
        [SerializeField] private GameObject root;
        [SerializeField] private DataLogger dataLogger;

        private const int MaxPartsInSystem = 3;

        private RobotController robot;
        private ConveyorController conveyor;
        private PresenceSensor pickSensor;
        private WorkpieceSpawner spawner;
        private bool lastSensorState;
        private int currentParts;
        
        private bool pendingLog = false;
        private CellEvent pendingEvent;
        
        public event System.Action<DeviceStatus> OnStatusChange;

        public DeviceStatus status { get; private set; }

        private void Awake()
        {
            StateMachine = new Project.StateMachine.FiniteStateMachine();

            StateMachine.AddState(new Booting(this));
            StateMachine.AddState(new Idle(this));
            StateMachine.AddState(new Running(this));
            StateMachine.AddState(new Blocked(this));
            StateMachine.AddState(new Error(this));

            StateMachine.ChangeState<Booting>();

            conveyor = root.GetComponentInChildren<ConveyorController>();
            robot = root.GetComponentInChildren<RobotController>();
            pickSensor = root.GetComponentInChildren<PresenceSensor>();
            spawner = root.GetComponentInChildren<WorkpieceSpawner>();
        }

        private void OnEnable()
        {
            pickSensor.OnSensorStateChanged += OnPickSensorChanged;
            robot.motion.OnPartPlaced += RemovePart;
            StateMachine.OnStateChange += HandleStateChange;
        }

        private void OnDisable()
        {
            pickSensor.OnSensorStateChanged -= OnPickSensorChanged;
            robot.motion.OnPartPlaced -= RemovePart;
            StateMachine.OnStateChange -= HandleStateChange;

            robot.OnStateChange -= HandleRobotStateChange;
            conveyor.OnStateChange -= HandleConveyorStateChange;
        }

        private void Start()
        {
            spawner.CanSpawn = CanSpawn;
            robot.Initialize();
            conveyor.Initialize();
            robot.OnStateChange += HandleRobotStateChange;
            conveyor.OnStateChange += HandleConveyorStateChange;
            SetRunning();
            spawner.TrySpawn(); // first piece
            lastSensorState = pickSensor.GetState();
        }

        private void Update()
        {
            if(pendingLog)
            {
                _ = dataLogger.Log(pendingEvent);
                pendingLog = false;
            }
        }

        private void HandleStateChange(IState state)
        {
            Debug.Log($"Cell Manager State: {state.GetType().Name}");
            SetStatus(state.Status);

            UpdateAndLogCellEvent("cell", state);
        }

        private void HandleRobotStateChange(IState state)
        {
            UpdateAndLogCellEvent("robot", state);
        }

        private void HandleConveyorStateChange(IState state)
        {
            UpdateAndLogCellEvent("conveyor", state);
        }

        private void UpdateAndLogCellEvent(string source, IState state)
        {
            string robotState = robot.GetCurrentState().GetType().Name;
            string conveyorState = conveyor.GetCurrentState()?.GetType().Name ?? "null";
            string cellState = StateMachine.CurrentState.GetType().Name;
            bool sensorActive = pickSensor.GetState();
            int pieceCount = KPIManager.Instance.totalPartsProduced;
            float cycleTime = KPIManager.Instance.totalCycleTime / Mathf.Max(1, KPIManager.Instance.completedCycles);
            bool isError = StateMachine.IsInState<Error>();
            string timestamp = System.DateTime.UtcNow.ToString("o");

            var evt = new CellEvent
            {
                cellState = cellState,
                robotState = robotState,
                conveyorState = conveyorState,
                pieceCount = pieceCount,
                cycleTime = cycleTime,
                pickSensorActive = sensorActive,
                isError = isError,
                eventType = "state_change",
                source = source,
                timestamp = timestamp
            };

            pendingLog = true;
        }

        private void SetStatus(DeviceStatus status)
        {
            if (this.status == status) return;

            this.status = status;
            OnStatusChange?.Invoke(status);
        }
        
        private void OnPickSensorChanged(bool detected)
        {
            if (StateMachine.IsInState<Booting>())
                return;

            robot.SetPartAtPickPosition(detected);
            if (detected)
            {
                OnWorkpieceDetectedAtPick();
            }
            else if (!detected && lastSensorState)
            {
                OnPartLeftPick();
            }
            lastSensorState = detected;
        }

        public void OnWorkpieceDetectedAtPick()
        {
            if (StateMachine.IsInState<Error>()) return;
            conveyor.SetBlocked();

            if (robot.IsHoldingPart() || robot.IsInState<ErrorState>()) return;
            robot.OnPartArrived();
        }

        private void OnPartLeftPick()
        {
            if (StateMachine.IsInState<Error>()) return;

            conveyor.SetRunning();
            spawner.TrySpawn();
        }

        public void OnError(string message)
        {
            conveyor.TriggerError(message);
            robot.TriggerError(message);

            KPIManager.Instance.SetSystemRunning(false);
            StateMachine.ChangeState<Error>();
        }

        public void ResetCell()
        {
            if (!StateMachine.IsInState<Error>()) return;

            robot.ClearError();
            conveyor.Initialize();

            SetIdle();
        }

        private void SetIdle()
        {
            if (StateMachine.IsInState<Idle>()) return; //Idempotence
            StateMachine.ChangeState<Idle>();
            conveyor.SetStopped();
            robot.SetIdle();
        }

        private void SetIdleExcept(MonoBehaviour equipment)
        {
            if (StateMachine.IsInState<Idle>()) return; //Idempotence
            StateMachine.ChangeState<Idle>();
            if (equipment != conveyor)
            {
                conveyor.SetStopped();
            }
            if (equipment != robot)
            {
                robot.SetIdle();
            }
        }

        private void SetRunning()
        {
            if (StateMachine.IsInState<Running>()) return; //Idempotence
            KPIManager.Instance.SetSystemRunning(true);
            StateMachine.ChangeState<Running>();
            robot.SetStarved();
            if (!pickSensor.HasPart) conveyor.SetRunning();
        }

        private void SetBlocked()
        {
            if (StateMachine.IsInState<Blocked>()) return; //Idempotence

            conveyor.SetBlocked();
            StateMachine.ChangeState<Blocked>();
        }

        public async void RemovePart(Transform part)
        {
            await Awaitable.WaitForSecondsAsync(3.0f);
            Destroy(part.gameObject);
        }

        public bool CanSpawn()
        {
            return currentParts < MaxPartsInSystem
                && !pickSensor.HasPart;
                //&& !robot.IsHoldingPart();
        }

        #region FaultSimulation

        public void StartCell()
        {
            var aux = true;
            if (!StateMachine.IsInState<Cell.States.Idle>()) aux = false;
            if (!robot.IsInState<Robotics.States.IdleState>()) aux = false;
            if (!conveyor.IsInState<Simulation.Conveyor.States.Stopped>()) aux = false;
            if (!aux)
            {
                Debug.LogWarning("UI → Start Cell: Cancel. System is not ready.");
                return;
            }

            Debug.Log("UI → Start Cell");

            SetRunning();

            robot.ResumePreviousOperation();

            if (pickSensor.HasPart) OnWorkpieceDetectedAtPick();
        }
        
        public void TriggerCellError()
        {
            Debug.Log("UI → Trigger Cell Error");

            OnError("Manual Cell Error");
        }

        public void ResetCellManual()
        {
            if (!StateMachine.IsInState<Error>()) return;
            
            Debug.Log("UI → Reset Cell");

            if (robot.IsInState<ErrorState>()) robot.ClearError();
            if (conveyor.IsInState<Simulation.Conveyor.States.Error>()) conveyor.ClearError();

            StateMachine.ChangeState<Idle>();
        }
        
        public void TriggerRobotError()
        {
            Debug.Log("UI → Trigger Robot Error");

            SetIdleExcept(robot);
            robot.TriggerError("Manual UI Error");
        }

        public void ResetRobotError()
        {
            if (!robot.IsInState<Robotics.States.ErrorState>()) return;
            
            Debug.Log("UI → Reset Robot Error");

            robot.ClearError();
        }

        public void TriggerConveyorError()
        {
            Debug.Log("UI → Trigger Conveyor Error");

            SetIdleExcept(conveyor);
            conveyor.TriggerError("Manual UI Error");
        }

        public void ResetConveyorError()
        {
            if (!conveyor.IsInState<Simulation.Conveyor.States.Error>()) return;

            Debug.Log("UI → Reset Conveyor Error");

            conveyor.ClearError();
        }

        public void ToggleSensorError()
        {
            if (pickSensor.HasValidReading)
            {
                Debug.Log("UI → Sensor is reversed");

                pickSensor.ForceInvalidReading(true);
                GameObject.FindGameObjectWithTag("Switch Button").GetComponent<Image>().color = Color.red;
            }
            else
            {
                Debug.Log("UI → Sensor is normal");

                pickSensor.ForceInvalidReading(false);
                GameObject.FindGameObjectWithTag("Switch Button").GetComponent<Image>().color = Color.green;
            }
        }

        #endregion
    }
}