using UnityEngine;
using Project.StateMachine;
using Project.Robotics.States;
using Project.Robotics.Motion;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

namespace Project.Robotics
{
    public class RobotController : MonoBehaviour
    {
        protected Project.StateMachine.FiniteStateMachine StateMachine { get; private set; }

        public RobotMotionController motion;
        [SerializeField] private Vector3 pickPoint;
        [SerializeField] private Vector3 placePoint;
        [SerializeField] private GameObject workpiece;

        public event System.Action<IState> OnStateChange;
        public event Action<DeviceStatus> OnStatusChange;
        public static event Action OnCycleStarted;
        public static event Action OnCycleCompleted;
        
        private List<Transform> workpiecesInRange = new List<Transform>();
        public List<Transform> WorkpiecesInRange => workpiecesInRange;

        public DeviceStatus status { get; private set; }
        private bool partAtPickPosition;
        private bool resetRequested;
        private string lastError;
        Transform attachedPart;

        //Snapshot
        private Type lastStateType;
        private bool lastHoldingPart;
        private bool lastPartAtPick;
        private Vector3 lastTargetPosition;
        private bool hasSavedState = false;

        internal void Initialize()//CellManager calls
        {
            StateMachine = new Project.StateMachine.FiniteStateMachine();

            StateMachine.AddState(new IdleState(this));
            StateMachine.AddState(new StarvedState(this));
            StateMachine.AddState(new MoveToPickState(this));
            StateMachine.AddState(new PickState(this));
            StateMachine.AddState(new MoveToPlaceState(this));
            StateMachine.AddState(new PlaceState(this));
            StateMachine.AddState(new ErrorState(this));

            StateMachine.ChangeState<StarvedState>();

            StateMachine.OnStateChange -= HandleStateChange;
            StateMachine.OnStateChange += HandleStateChange;
        }

        private void OnEnable()
        {
            if (StateMachine != null) 
            {
                StateMachine.OnStateChange -= HandleStateChange; 
                StateMachine.OnStateChange += HandleStateChange;
            }
        }

        private void OnDisable()
        {
            StateMachine.OnStateChange -= HandleStateChange;
        }

        private void Update()
        {
            if (StateMachine != null)
                StateMachine.Update();
        }

        #region Claw Sensor

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Workpiece") && !workpiecesInRange.Contains(other.transform))
            {
                workpiecesInRange.Add(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Workpiece") && workpiecesInRange.Contains(other.transform))
            {
                workpiecesInRange.Remove(other.transform);
            }
        }

        #endregion

        private void HandleStateChange(IState state)
        {
            Debug.Log($"Robot : {state.GetType().Name}");
            SetStatus(state.Status);
            OnStateChange?.Invoke(state);
        }

        #region State Machine API

        public void SetIdle()
        {
            if (!hasSavedState) SaveCurrentState();
            if (StateMachine.IsInState<ErrorState>()) return;
            StateMachine.ChangeState<IdleState>();
        }
        public void SetStarved()
        {
            if (StateMachine.IsInState<ErrorState>()) return;
            StateMachine.ChangeState<StarvedState>();
        }
        public void SetMoveToPickState()
        {
            if (StateMachine.IsInState<ErrorState>()) return;
            StateMachine.ChangeState<MoveToPickState>();
        }
        public void SetPickState()
        {
            if (StateMachine.IsInState<ErrorState>()) return;
            StateMachine.ChangeState<PickState>();
        }
        public void SetMoveToPlaceState()
        {
            if (StateMachine.IsInState<ErrorState>()) return;
            StateMachine.ChangeState<MoveToPlaceState>();
        }
        public void SetPlaceState()
        {
            if (StateMachine.IsInState<ErrorState>()) return;
            StateMachine.ChangeState<PlaceState>();
        }
        public void SetError()
        {
            StateMachine.ChangeState<ErrorState>();
        }
        public bool IsInState<T>() where T : IState
        {
            return StateMachine.IsInState<T>();
        }
        public IState GetCurrentState()
        {
            return StateMachine.CurrentState;
        }

        #endregion

        public void RefreshWorkpiecesInRange()
        {
            workpiecesInRange.Clear();

            Collider[] hits = Physics.OverlapSphere(transform.position, 1.0f);

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Workpiece"))
                {
                    workpiecesInRange.Add(hit.transform);
                }
            }
        }

        #region Industrial API

        public bool HasPartAtPickPosition() => partAtPickPosition;
        public void SetPartAtPickPosition(bool value) => partAtPickPosition = value;

        public bool IsHoldingPart() => attachedPart != null;

        public void OnPartArrived()
        {
            StateMachine.ChangeState<MoveToPickState>();
            NotifyCycleStarted();
        }

        public void OnPartLeft()
        {
            //
        }

        public void AttachPart()
        {
            if(workpiecesInRange.Count <= 0)
            {
                TriggerError("Pick failed: no part in range");
                return;
            }
            partAtPickPosition = false;
            attachedPart = motion.AttachPart(workpiecesInRange[0]);
        }

        public void ReleasePart()
        {
            Vector3 front = transform.position + (transform.forward*1f);
            motion.ReleasePart(front);
            attachedPart = null;
            NotifyCycleCompleted();
        }

        public void NotifyCycleStarted()
        {
            OnCycleStarted?.Invoke();
        }

        public void NotifyCycleCompleted()
        {
            OnCycleCompleted?.Invoke();
        }

        public void TriggerError(string errorMessage)
        {
            SaveCurrentState();
            lastError = errorMessage;
            Debug.LogError(errorMessage);
            StateMachine.ChangeState<ErrorState>();
        }

        public void ClearError()
        {
            lastError = string.Empty;
            resetRequested = false;
            motion.ResetMotion();
            StateMachine.ChangeState<IdleState>();
            RefreshWorkpiecesInRange();
        }

        public bool IsResetRequested() => resetRequested;

        public void RequestReset()
        {
            resetRequested = true;
        }

        private void SetStatus(DeviceStatus status)
        {
            if (this.status == status) return;

            this.status = status;
            OnStatusChange?.Invoke(status);
        }

        #endregion

        public void MoveToPick(Action onReached = null)
        {
            motion.MoveTo(pickPoint, onReached);
        }

        public void MoveToPlace(Action onReached = null)
        {
            motion.MoveTo(placePoint, onReached);
        }

        public bool HasReachedTarget()
        {
            return !motion.IsMoving;
        }

        public void SaveCurrentState()
        {
            if (StateMachine?.CurrentState == null) return;

            lastStateType = StateMachine.CurrentState.GetType();
            lastHoldingPart = IsHoldingPart();
            lastPartAtPick = partAtPickPosition;
            lastTargetPosition = motion.CurrentTarget;

            hasSavedState = true;
        }
        
        public void ResumePreviousOperation()
        {
            if (!hasSavedState)
            {
                StateMachine.ChangeState<StarvedState>();
                return;
            }

            Debug.Log($"Resuming state: {lastStateType.Name}");

            if (motion.HasAttachedPart) { attachedPart = motion.GetAttachedParts[0]; }
            else { attachedPart = null; }

            partAtPickPosition = lastPartAtPick;
            motion.MoveTo(lastTargetPosition);

            StateMachine.ChangeState(lastStateType);
            
            if (lastStateType == typeof(PickState) && !partAtPickPosition)
            {
                StateMachine.ChangeState<StarvedState>();
                return;
            }

            hasSavedState = false;
        }
    }
}