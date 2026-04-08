using System;
using System.Collections.Generic;
using UnityEngine;
using Project.Simulation.Conveyor.States;
using Project.StateMachine;

namespace Project.Simulation
{
    public class ConveyorController : MonoBehaviour
    {
        protected Project.StateMachine.FiniteStateMachine StateMachine { get; private set; }

        [SerializeField] public float speed = 0.5f;
        
        public event System.Action<IState> OnStateChange;
        public event Action<DeviceStatus> OnStatusChange;

        public DeviceStatus status { get; private set; }

        public float Speed => speed;
        private string lastError;

        private readonly List<Transform> workpiecesOnBelt = new();

        internal void Initialize()//Depende do CellManager
        {
            StateMachine = new Project.StateMachine.FiniteStateMachine();

            StateMachine.AddState(new Stopped(this));
            StateMachine.AddState(new Running(this));
            StateMachine.AddState(new Blocked(this));
            StateMachine.AddState(new Error(this));

            StateMachine.ChangeState<Stopped>();

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
            {
                StateMachine.Update();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Workpiece")) return;

            workpiecesOnBelt.Add(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Workpiece")) return;

            workpiecesOnBelt.Remove(other.transform);
        }

        private void HandleStateChange(IState state)
        {
            Debug.Log($"Conveyor : {state.GetType().Name}");
            SetStatus(state.Status);
            OnStateChange?.Invoke(state);
        }

        #region State Machine API

        public void SetStopped()
        {
            if (StateMachine.IsInState<Error>()) return;
            StateMachine.ChangeState<Stopped>();
        }
        public void SetRunning()
        {
            if (StateMachine.IsInState<Error>()) return;
            speed = 0.5f;
            StateMachine.ChangeState<Running>();
        }
        public void SetBlocked()
        {
            if (StateMachine.IsInState<Error>()) return;
            StateMachine.ChangeState<Blocked>();
        }
        public void SetError()
        {
            StateMachine.ChangeState<Error>();
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

        public void TriggerError(string errorMessage)
        {
            StopImmediately();
            lastError = errorMessage;
            Debug.LogError(errorMessage);
            StateMachine.ChangeState<Error>();
        }

        public void ClearError()
        {
            lastError = string.Empty;
            StateMachine.ChangeState<Stopped>();
        }

        internal void Move()
        {
            Vector3 delta = transform.forward * speed * Time.deltaTime;
            Transport(delta);
        }

        private void Transport(Vector3 delta)
        {
            foreach (var wp in workpiecesOnBelt)
            {
                wp.position += delta;
            }
        }

        private void SetStatus(DeviceStatus status)
        {
            if (this.status == status) return;

            this.status = status;
            OnStatusChange?.Invoke(status);
        }
        
        internal void StopImmediately()
        {
            speed = 0f;
        }
    }
}