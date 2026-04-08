using Project.Robotics;
using UnityEngine;

namespace Project.KPIs
{
    public class KPIManager : MonoBehaviour
    {
        public static KPIManager Instance;

        [SerializeField] private RobotController robot;

        public int totalPartsProduced  { get; private set; }
        public float totalCycleTime  { get; private set; }
        public int completedCycles  { get; private set; }
        public float downtime  { get; private set; }
        public bool systemRunning  { get; private set; }
        public float lastCycleStart  { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            RobotController.OnCycleStarted += StartCycle;
            RobotController.OnCycleCompleted += CompleteCycle;
        }

        private void OnDisable()
        {
            RobotController.OnCycleStarted -= StartCycle;
            RobotController.OnCycleCompleted -= CompleteCycle;
        }

        private void Update()
        {
            if (!systemRunning)
                downtime += Time.deltaTime;
        }

        public void StartCycle()
        {
            lastCycleStart = Time.time;
        }

        public void CompleteCycle()
        {
            float cycleTime = Time.time - lastCycleStart;

            totalPartsProduced++;
            totalCycleTime += cycleTime;
            completedCycles++;
        }

        public void SetSystemRunning(bool running)
        {
            systemRunning = running;
        }

        public bool IsSystemRunning()
        {
            return systemRunning;
        }

        public int GetTotalParts() => totalPartsProduced;

        public float GetAverageCycleTime()
        {
            if (completedCycles == 0) return 0f;
            return totalCycleTime / completedCycles;
        }

        public float GetThroughputPerMinute()
        {
            float minutes = Time.time / 60f;
            if (minutes <= 0f) return 0f;

            return totalPartsProduced / minutes;
        }

        public float GetDowntime() => downtime;
    }
}
