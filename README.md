# 🧩 Digital Twin – Pick & Place System (Unity + MQTT)

## 🎯 Overview

This project implements a **Digital Twin of a Pick & Place system** developed in Unity, designed to simulate industrial automation scenarios and communicate with external systems using the **MQTT protocol**.

The Digital Twin replicates the behavior of a real pick-and-place operation, allowing interaction, monitoring, and control through message exchange.

---

## 🧠 Concept

A Digital Twin is a virtual representation of a physical system. In this project, the Unity environment simulates:

* Object detection and handling
* Pick & Place operations
* System state transitions
* Real-time communication via MQTT

The architecture is designed to integrate with external platforms such as Node-RED for orchestration and data flow management.

---

## ⚙️ Technologies Used

* Unity (3D simulation environment)
* C#
* MQTT protocol
* Node-RED (external, not included in this repository)

---

## 🔌 MQTT Integration

The system communicates using MQTT topics to:

* Receive commands (e.g., start/stop operations)
* Publish system states (e.g., idle, picking, placing)
* Simulate real-world device communication

⚠️ Note:
The Node-RED server is **not included** in this repository.
You must configure your own MQTT broker and Node-RED flows externally.

---

## ▶️ Demo

👉 [Access the WebGL Demo here](mnobren.github.io)
(*Replace with your GitHub Pages link*)

---

## 🏗️ System Architecture

```
[ Node-RED ] ⇄ [ MQTT Broker ] ⇄ [ Unity Digital Twin ]
```

* Unity acts as the Digital Twin (simulation layer)
* MQTT enables communication
* Node-RED orchestrates logic and integration

---

## 🚀 How to Run

### Option 1 – Web Demo

Access the project directly in your browser:
👉 [Demo Link](#)

### Option 2 – Local Build

1. Download the build from the repository
2. Run the executable
3. Ensure MQTT broker is running
4. Configure connection settings if necessary

---

## 🧪 Possible Extensions

* Integration with real PLCs or sensors
* Real-time data ingestion
* Dashboard visualization
* AI-based optimization

---

## 📸 Screenshots

...

---

## 👤 Author

Mauricio Nobre Nunes Pereira
LinkedIn:
GitHub:

---

## 📄 License

This project is for educational and portfolio purposes.
