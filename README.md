# 🧠 Workflow Engine — Infonetica Assignment

This project implements a custom **workflow engine** as specified in the Infonetica assignment. It supports dynamic state transitions for various workflows like approvals, tasks, or order flows. To make the implementation easier to understand and demonstrate, a **Pizza Order Tracker** example was added as a secondary use case.

---

## 📄 Assignment Overview

The original task required:

* A system that defines workflows using states and transitions
* Ability to create instances of a workflow
* Transition between states using valid actions
* Maintain a history of transitions

This solution meets all those requirements using a minimal and self-contained approach.

---

## ✅ Features

* Define workflows with custom states and actions
* Instantiate new workflow instances at runtime
* Validate and apply state transitions
* Keep track of history for every instance
* In-memory architecture (no DB required)
* RESTful API using ASP.NET Core
* Basic frontend interface using HTML + JS (secondary demo)

---

## 🧠 Core Concepts

### 1. Workflow Definition

A reusable template that defines:

* States (with `isInitial` and `isFinal` flags)
* Actions (with allowed transitions between states)

### 2. Workflow Instance

A single runtime object based on a definition:

* Tracks the current state
* Holds a list of past actions (history)

### 3. Endpoints

| Endpoint                                  | Purpose                        |
| ----------------------------------------- | ------------------------------ |
| `POST /defs`                              | Register a workflow definition |
| `POST /instances?defId=xyz`               | Start a new instance           |
| `POST /instances/{id}/actions/{actionId}` | Move instance via action       |

---

## 🛠 How to Run Backend

### Requirements

* [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download)

### Steps

```bash
cd path/to/project

dotnet run
```

Then visit: `http://localhost:5068`

---

## 🌐 Secondary UI: Pizza Order Tracker

A simple HTML + JS frontend is included to demonstrate how the workflow system can be applied to real-world use cases. It simulates pizza orders moving through states like:

* `order_placed` → `preparing` → `baking` → `ready` → `delivered`

This is not part of the original assignment but helps visualize the logic and is great for interviews or demos.

---

## 💡 Why This Implementation Stands Out

* Clean, readable, and extendable code
* Covers all required assignment features
* Bonus UI shows practical integration
* No extra tooling or DB setup required
* Beginner-friendly, yet robust

---

## 🔧 Sample Workflow JSON

Use this with `POST /defs`:

```json
{
  "id": "pizza_order",
  "states": [
    {"id": "order_placed", "isInitial": true, "isFinal": false},
    {"id": "preparing", "isInitial": false, "isFinal": false},
    {"id": "baking", "isInitial": false, "isFinal": false},
    {"id": "ready", "isInitial": false, "isFinal": false},
    {"id": "delivered", "isInitial": false, "isFinal": true}
  ],
  "actions": [
    {"id": "start_prep", "fromStates": ["order_placed"], "toState": "preparing"},
    {"id": "start_bake", "fromStates": ["preparing"], "toState": "baking"},
    {"id": "finish_bake", "fromStates": ["baking"], "toState": "ready"},
    {"id": "deliver", "fromStates": ["ready"], "toState": "delivered"}
  ]
}
```

---

## 🧾 Files

| File         | Purpose                            |
| ------------ | ---------------------------------- |
| `Program.cs` | Main backend server with all logic |
| `pizza.html` | Optional frontend UI for demo      |
| `style.css`  | Styling for demo UI                |

---

## 📦 Future Suggestions (Optional)

* Add persistence (SQLite, file storage)
* User authentication or roles
* Visual drag-and-drop workflow designer

---

## 🙌 Final Notes

This project **fully satisfies** the Infonetica assignment requirements, and adds a real-world demonstration layer for better clarity and impact.

Feel free to fork, adapt, or scale it for other workflow-based systems (support tickets, document approval, etc).
