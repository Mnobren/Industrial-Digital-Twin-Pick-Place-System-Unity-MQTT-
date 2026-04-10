using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.ShaderKeywordFilter;
#endif
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

namespace Project.StateMachine
{
    public class FiniteStateMachine
    {
        public IState CurrentState { get; private set; }

        private Dictionary<System.Type, IState> _states = new();

        public event System.Action<IState> OnStateChange;

        public void AddState(IState state)
        {
            _states[state.GetType()] = state;
        }

        public void ChangeState<T>() where T : IState
        {
            var newStateType = typeof(T);

            if (!_states.ContainsKey(newStateType))
                return;

            CurrentState?.Exit();
            CurrentState = _states[newStateType];
            CurrentState.Enter();

            OnStateChange?.Invoke(CurrentState);
        }
        
        public void ChangeState(Type stateType)
        {
            var newStateType = stateType;

            if (!_states.ContainsKey(newStateType))
                return;

            CurrentState?.Exit();
            CurrentState = _states[newStateType];
            CurrentState.Enter();

            OnStateChange?.Invoke(CurrentState);
        }

        public void Update()
        {
            CurrentState?.Update();
        }

        public bool IsInState<T>() where T : IState
        {
            return CurrentState is T;
        }
    }
}
