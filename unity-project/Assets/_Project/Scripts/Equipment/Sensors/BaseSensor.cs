using System;
using UnityEngine;

namespace Project.Sensors
{
    public abstract class BaseSensor : MonoBehaviour
    {
        protected bool rawState;
        protected bool currentState;
        public event Action<bool> OnSensorStateChanged;
        private bool forcedInvalid = false;
        public virtual bool HasValidReading => !forcedInvalid;

        protected virtual void SetState(bool newState)
        {
            rawState = newState;

            bool finalState = HasValidReading ? rawState : !rawState;

            if (currentState == finalState)
                return;

            currentState = finalState;
            OnSensorStateChanged?.Invoke(currentState);
        }

        public bool GetState()
        {
            return currentState;
        }

        public virtual void ForceInvalidReading(bool value)
        {
            forcedInvalid = value;
            SetState(rawState);
        }
    }
}
