using NUnit.Framework;
using UnityEngine;

namespace Project.Sensors
{
    [RequireComponent(typeof(Collider))]
    public class PresenceSensor : BaseSensor
    {
        public bool HasPart { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Workpiece"))
                return;

            SetState(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Workpiece"))
                return;

            SetState(false);
        }

        protected override void SetState(bool newState)
        {
            if(HasValidReading) { HasPart = newState; }
            else { HasPart = !newState; }
            base.SetState(newState);
        }

        public override void ForceInvalidReading(bool value)
        {
            if(value == HasValidReading) HasPart = !HasPart;
            base.ForceInvalidReading(value);
        }
    }
}
