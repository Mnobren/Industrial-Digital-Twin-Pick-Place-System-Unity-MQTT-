using UnityEngine;
using TMPro;
using Project.KPIs;

namespace Project.UI
{
    public class KPIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI partsText;
        [SerializeField] private TextMeshProUGUI cycleText;
        [SerializeField] private TextMeshProUGUI throughputText;
        [SerializeField] private TextMeshProUGUI downtimeText;

        private void Update()
        {
            partsText.text = "Parts: " + KPIManager.Instance.GetTotalParts();
            cycleText.text = "Avg Cycle: " + KPIManager.Instance.GetAverageCycleTime().ToString("F2") + " s";
            throughputText.text = "Throughput: " + KPIManager.Instance.GetThroughputPerMinute().ToString("F2") + " parts/min";
            downtimeText.text = "Downtime: " + KPIManager.Instance.GetDowntime().ToString("F1") + " s";
        }
    }
}