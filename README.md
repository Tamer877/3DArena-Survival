# 3D Wave-based Arena Survival (Architecture & Gameplay Prototype)

A high-performance, modular 3D top-down survival prototype built with Unity URP (Universal Render Pipeline). 

This project was developed with a primary focus on **Software Engineering best practices**, **Clean Architecture**, and **Mobile Performance Optimization** (Zero-Allocation patterns, decoupled systems, and scalable AI design) tailored for top-tier mobile & casual game studios.

---

## Technical Architecture & Design Patterns

The project completely decouples core gameplay systems to ensure testability, maintainability, and scalability.

```text
       [ Input / Mouse Aim ]
                 │
                 ▼
       [ PlayerController ] (Rigidbody / FixedUpdate)
                 │
                 ▼
          [ Weapon System ] ──── (Raycast Hitscan)
                 │
        ┌────────┴────────┐
        ▼                 ▼
[ Pooled Hit VFX ]  [ IDamageable ] ◄── (Abstract Interface)
                          ▲
            ┌─────────────┴─────────────┐
            │                           │
    [ Player Character ]        [ Enemy Character ]
            │                           ▲
            │ (Action Events)           │ (NavMesh + FSM)
            ▼                           │
       [ GameHUD ]              [ WaveManager ] ◄── [ Enemy ObjectPool ]
    (Observer Pattern)
```

### 1. Object Pooling (Zero-Allocation Memory Discipline)
* **Problem:** Frequent `Instantiate` and `Destroy` calls for bullets, hit particles, and enemies cause severe heap fragmentation and trigger frequent Garbage Collection (GC) spikes, degrading mobile frame rates.
* **Solution:** Implemented a generic `ObjectPool<T> where T : Component` to pre-warm and reuse instances. Objects are activated/deactivated with state resetting (`ResetEnemy`), achieving continuous runtime execution with near-zero runtime heap allocation.

### 2. Loose Coupling & Abstraction (`IDamageable`)
* **Principle:** Adheres to the **Open-Closed Principle (OCP)** and **Interface Segregation Principle (ISP)**.
* Combat logic does not depend on concrete implementations (`Player` or `Enemy`). Any damageable entity (destructible obstacles, barrels, players, enemies) implements `IDamageable`. Weapons interact strictly via `TryGetComponent<IDamageable>()`.

### 3. Observer Pattern (Event-Driven UI Layer)
* **Principle:** Complete decoupling of Gameplay and UI layers.
* The UI never polls character data inside `Update()`. Instead, `Character` and `WaveManager` broadcast C# `System.Action` events (`OnHealthChanged`, `OnDeath`, `OnWaveStarted`). 
* UI components subscribe during `OnEnable` and clean up during `OnDisable` to guarantee memory safety and prevent event subscription leaks.

### 4. Finite State Machine (FSM) & Vector Math for AI
* **AI Architecture:** Enemy behavior is governed by an extensible State Machine (`IState`, `ChaseState`, `AttackState`) rather than brittle nested conditionals.
* **Vector Mathematics:** Integrated field-of-view evaluation using the **Dot Product** ($\vec{A} \cdot \vec{B} = \vert{}\vec{A}\vert{} \vert{}\vec{B}\vert{} \cos\theta$) instead of expensive trigonometric or distance checks, optimizing CPU cycle budget.
* **Pathfinding:** Dynamic navigation via Unity's `NavMeshAgent`, employing `NavMesh.SamplePosition` to prevent spawn-point clipping and tunneling.

### 5. Deterministic Physics & Input Handling
* Input is captured per-frame in `Update()` to eliminate input dropping.
* Physical movement and aiming rotations are strictly evaluated inside `FixedUpdate()` via `Rigidbody.MovePosition` and `Rigidbody.MoveRotation`, avoiding collision tunneling and physics jitter.

---

## Tech Stack & Specifications

* **Engine:** Unity (URP - Universal Render Pipeline)
* **Language:** C# (.NET Standard)
* **Physics Engine:** PhysX (Discrete Raycasting & Rigidbody Interpolation)
* **Navigation:** Unity NavMesh System
* **Architecture:** Component-Based, Interface-Driven, FSM, Generic Object Pooling
* **Version Control:** Feature-branching workflow, structured commit conventions (`feat:`, `fix:`, `refactor:`)

---

## Folder Structure

```text
Assets/
└── _Project/
    ├── Prefabs/
    │   ├── Characters/
    │   └── Combat/
    └── Scripts/
        ├── AI/             # FSM core, States (Chase, Attack), Enemy AI
        ├── Characters/     # Base Character, PlayerController, Player
        ├── Combat/         # Raycast Weapon, Pooled VFX
        ├── Core/           # IDamageable, Generic ObjectPool<T>
        ├── Managers/       # WaveManager, Spawner logic
        └── UI/             # Event-driven HUD & GameOver panels
```

---

## How to Run
1. Clone the repository:
   ```bash
   git clone <REPO_URL>
   ```
2. Open the project in Unity (URP compatible).
3. Open `Assets/_Project/Scenes/MainArena.unity` (or your active scene).
4. Hit **Play**. Use **WASD** to move, **Mouse Cursor** to aim, and **Left Mouse Button** to fire.
