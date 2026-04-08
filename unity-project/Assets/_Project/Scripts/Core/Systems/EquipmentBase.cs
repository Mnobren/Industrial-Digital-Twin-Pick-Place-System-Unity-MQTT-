using UnityEngine;

namespace Project.Core.Systems
{
    public enum EquipmentState
    {
        IDLE,
        RUN,
        WAIT,
        FAULT
    }

    public abstract class EquipmentBase : MonoBehaviour
    {
        public EquipmentState CurrentState { get; protected set; }

        public virtual void SetState(EquipmentState newState)
        {
            CurrentState = newState;
        }

        public bool IsRunning() => CurrentState == EquipmentState.RUN;
        public bool IsFaulted() => CurrentState == EquipmentState.FAULT;
    }
}
