using System;
using Project.Robotics;
using Project.Robotics.States;
using UnityEngine;

public class RobotLEDVisual : MonoBehaviour
{
    [SerializeField] private RobotController controller;
    [SerializeField] private Renderer rend;

    private void OnEnable()
    {
        controller.OnStatusChange += UpdateColor;
        UpdateColor(controller.status); // sync inicial
    }

    private void OnDisable()
    {
        controller.OnStatusChange -= UpdateColor;
    }

    private void UpdateColor(DeviceStatus status)
    {
        switch (status)
        {
            case DeviceStatus.IDLE:
                rend.material.color = Color.white;
                break;

            case DeviceStatus.RUN:
                rend.material.color = Color.green;
                break;

            case DeviceStatus.WAIT:
                rend.material.color = Color.yellow;
                break;
                
            case DeviceStatus.FAULT:
                rend.material.color = Color.red;
                break;
        }
    }
}