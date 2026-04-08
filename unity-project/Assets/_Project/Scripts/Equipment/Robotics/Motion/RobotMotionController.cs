using System.Collections.Generic;
using System;
using UnityEngine;

namespace Project.Robotics.Motion
{
    public class RobotMotionController : MonoBehaviour
    {
        [Header("Motion Settings")]
        [SerializeField] private float speed = 1f;
        [SerializeField] private float positionTolerance = 0.001f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float alignmentTolerance = 2f; // graus

        public System.Action<Transform> OnPartPlaced;

        public bool IsMoving { get; private set; }

        private Action onTargetReached;
        private Vector3 targetPosition;
        private List<Transform> attached = new List<Transform>();
        public Vector3 CurrentTarget => targetPosition;
        public bool HasAttachedPart => attached.Count > 0;
        public List<Transform> GetAttachedParts => attached;

        public void MoveTo(Vector3 target, Action onReached = null)
        {
            targetPosition = target;
            onTargetReached = onReached;
            IsMoving = true;
        }

        private void Update()
        {
            if (!IsMoving) return;

            Vector3 toTarget = targetPosition - transform.position;

            if (toTarget.magnitude <= positionTolerance)
            {
                IsMoving = false;
                onTargetReached?.Invoke();
                onTargetReached = null;
                return;
            }

            Vector3 targetDir = toTarget.normalized;
            float angle = Vector3.Angle(transform.forward, targetDir);

            if (angle > alignmentTolerance)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    speed * Time.deltaTime
                );
            }
        }

        public void ResetMotion()
        {
            IsMoving = false;
            onTargetReached = null;
        }

        #region Part Handling

        public Transform AttachPart(Transform workpiece)
        {
            if (workpiece == null) return null;

            workpiece.SetParent(transform);
            attached.Add(workpiece);
            return workpiece;
        }

        public void ReleasePart(Vector3 worldPosition)
        {
            if (transform.childCount == 0 || attached.Count == 0)
            {
                if (transform.childCount != attached.Count) Debug.LogError("ERROR: data inconsistence");
                return;
            }

            Transform part = attached[0];

            OnPartPlaced(part);
            part.SetParent(null);
            attached.Remove(part);
            part.position = worldPosition;
        }

        public void StopMovement()
        {
            IsMoving = false;
        }

        #endregion
    }
}