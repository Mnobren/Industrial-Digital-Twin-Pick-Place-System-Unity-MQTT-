using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project.Cell
{
    public class WorkpieceSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject workpiecePrefab;
        [SerializeField] private Vector3 spawnPoint;

        public Func<bool> CanSpawn; // 🔥 delega decisão

        public void TrySpawn()
        {
            if (CanSpawn != null && !CanSpawn())
                return;

            Instantiate(workpiecePrefab, spawnPoint, Quaternion.identity, transform);
        }
    }
}
