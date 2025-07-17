# 🍕 Pizza Workflow Engine — Infonetica Assignment

A lightweight, beginner-friendly .NET + HTML full-stack project to simulate a real-time pizza order tracking system using a custom workflow engine.

This was built to fulfill an assignment requirement to implement a workflow engine with state transitions, and goes beyond by showcasing a fun and practical example (Pizza Shop 🍕) in a real-world UI.

---

## 🚀 Features

✅ Define custom workflows (states and transitions)

✅ Create multiple instances of a workflow (orders)

✅ Move each instance through valid transitions

✅ In-memory storage (no DB required)

✅ Full frontend interface with buttons to update state

✅ CORS support for clean frontend-backend separation

✅ Beginner-readable code — easy to extend and adapt

---

## 🧠 How it Works

### 🏗 Backend — `Program.cs`

The C# backend is written using ASP.NET Core minimal APIs:

1. **WorkflowDefinition** = Defines states (e.g., `order_placed`, `baking`, `ready`) and actions (`start_bake`, `deliver`, etc.)

2. **WorkflowInstance** = Represents a single pizza order in progress

3. **Endpoints**:

   * `POST /defs` — Add a new workflow definition
   * `POST /instances?defId=xxx` — Create a new instance of a workflow
   * `POST /instances/{id}/actions/{actionId}` — Move to next state
   * `GET /instances` and `GET /defs` — Optional (for listing)

### 💻 Frontend — `pizza.html`

1. `Create New Order` button calls the backend to spawn a new pizza order.

2. UI renders a card per order showing current state and history.

3. Only valid buttons are enabled (based on current state).

4. Transitions update backend and re-render updated order info.

### 🧾 Styling — `style.css`

Simple clean UI, color-coded and mobile-friendly. Separated from HTML.

---

## 🛠 Setup Instructions

### ✅ Requirements:

* [.NET SDK 6+](https://dotnet.microsoft.com/en-us/download)
* Browser (any)

---

### ⚙️ Run the Backend

1. Save your C# backend file as `Program.cs`

2. In terminal:

```bash
cd path/to/your/project

dotnet run
```

You should see:

```
Now listening on: http://localhost:5068
```

---

### 🌐 Run the Frontend

1. Create two files in the same folder:

   * `pizza.html`
   * `style.css`

2. Open `pizza.html` in a browser (no server needed)

---

## 🧪 Example Definition (Pizza Flow)

Use the following to define the pizza workflow (`POST /defs`):

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

Use a tool like Git Bash or Postman:

```bash
curl -X POST http://localhost:5068/defs \
  -H "Content-Type: application/json" \
  -d @pizza.json
```

---

## 🔥 Why This Stands Out

* Simple, extensible workflow engine
* Real-time frontend interaction
* Clean separation of logic (HTML/CSS/JS + C#)
* Beginner-friendly readable code
* In-memory = no setup hassles
* Pizza theme 🍕 adds fun + clarity

---

## 🤝 Credits

This project was built by \[Your Name] as part of an Infonetica workflow engine challenge.

Feel free to fork and build your own custom workflow apps (food orders, customer support tickets, etc.)
