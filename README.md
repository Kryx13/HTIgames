# 🏛️ Scape - 3D Puzzle & Parkour

A third-person 3D adventure game combining demanding parkour mechanics and logical puzzles set deep within a dark and mysterious Aztec pyramid.

**Goal:** Escape the dungeon as quickly as possible (Speedrun focus).

---

## 🛠️ Tech Stack

* **Engine:** Unity 6 (6000.2.14f1)
* **Pipeline:** Universal Render Pipeline (URP)
* **Language:** C#
* **Key Systems:**
    * New Input System
    * Cinemachine (3rd Person Camera)
    * Modular Architecture (Interfaces, Managers)

---

## 🎮 Controls (Keyboard/Mouse)

The game natively supports both QWERTY and AZERTY layouts.

| Action | Key |
| :--- | :--- |
| **Move** | `WASD` / `ZQSD` |
| **Camera** | `Mouse` |
| **Jump** | `Space` |
| **Run** | `Left Shift` |
| **Pick up Items** | `Automatic (walk over item)` |
| **Drop Item** | `G` |
| **Toggle Flashlight** | `F` |
| **Shoot (Gun)** | `Left Click` |
| **Inventory** | `I` |
| **Menu / Pause** | `Esc` |

> **Note:** Items are picked up automatically when walking over them. Press `G` to drop the last item. Press `F` to toggle flashlight (requires Lampe). Left Click to shoot (requires Pistolet).

---

## 🎒 Inventory & Items

**Base inventory:** 2 slots (expandable to 5 with Backpack)

| Item | Effect | Location |
| :--- | :--- | :--- |
| **Amulette** | Opens final door (Stage 7) | Stage 0 |
| **Pistolet** | Shoots targets/enemies | Stage 2 (Room 5) |
| **Sac** | +3 inventory slots | Stage 5 |
| **Pioche** | Breaks fragile walls | Stage 5 |
| **Lampe** | Illuminates dark areas | Stage 5 (hidden) |
| **Map** | Shows maze path (wall item) | Stage 5 |

---

## 🗺️ Stage Breakdown

### Stage 0 — Tutorial
- Player falls from the sky into the first room
- Tutorial signs explain controls
- Mechanics: Move → Push blocks to reach door → Pick up **Amulette**

### Stage 1 — Falling Platforms
- Jump across platforms (blocks fall after **3 seconds**)
- Falling returns player to Stage 0
- Blocks reset on room re-entry

### Stage 2 — Door Maze (5 Rooms)
| Room | Doors | Destinations |
|------|-------|--------------|
| 1 | 2 | → Room 2, → Room 1 |
| 2 | 3 | → Room 3, → Room 2, → Room 1 |
| 3 | 4 | → Room 4, → Room 3, → Room 2, → Room 1 |
| 4 | 5 | → **Stage 3**, → Room 4, → Room 5, → Room 3, → Room 2 |
| 5 | 5 | → Rooms 1-5 (contains **Pistolet**) |

*Rooms are visually identical except for door count and pistol in Room 5.*

### Stage 3 — Shooting Gallery
- Shoot targets to activate platforms (5 sequences)
- Target behavior progression:
  1. Static
  2. Slow horizontal movement
  3. Fast horizontal movement
  4. Slow vertical OR horizontal (alternating)
  5. Random/unpredictable movement

### Stage 4 — Riddle Room
- Room filled with various objects
- Riddle inscription hints at correct item
- Place correct item on **stele** to open door

### Stage 5 — Destructible Maze
- Dimensions: **5 wide × 20 long** blocks
- Collect: **Sac**, **Pioche**, **Map** (wall)
- Only path blocks are destructible (shown on map)
- Hidden **Lampe** in breakable block

### Stage 6 — Darkness
- Total darkness, flashlight required
- Floor mostly holes, narrow safe path

### Stage 7 — Final Room
- Empty room with amulet slot on door
- Inscription: *"Pour savoir où on va, on doit savoir d'où on vient."*
- Hint: Amulette is in Stage 0

### 🏁 End Screen
- Display total completion time
- Leaderboard with saved runs
- Option to save run & restart

---

## 🖥️ UI Screens

### Main Menu
- `Play` | `Settings` | `Leaderboard` | `Credits` | `Quit`

### Pause Menu (Esc)
- `Resume` | `Restart` | `Settings` | `Quit to Menu`

### Settings
- Volume (Master, SFX, Music)
- Graphics quality
- Controls remapping

---

## 🔊 Audio Design

| Category | Examples |
| :--- | :--- |
| **Ambiance** | Dark pyramid atmosphere, echo, dripping water |
| **Player** | Footsteps (stone), jump, land, push object |
| **Items** | Pickup sound, pistol shot, pickaxe hit, flashlight toggle |
| **UI** | Button hover, click, menu open/close |
| **Events** | Block falling, door opening, target hit, victory jingle |

---

## 🗺️ Roadmap & Progress

### 🟢 Phase 1: Core (3C)
- [x] Project Setup & Git
- [x] Player Controller (Move, Jump, Run)
- [x] 3rd Person Camera (Cinemachine)
- [x] Input System (InputManager + GameControls)

### 🟡 Phase 2: Systems
- [x] Interaction System (Automatic trigger-based pickup)
- [x] Inventory System (2 slots + backpack extension)
- [x] Item Data (ScriptableObjects)
- [x] Item Pickup System (OnTriggerEnter)
- [x] Item: Sac (Backpack +3 slots with visual)
- [x] Item: Lampe (Spotlight toggle with F key)
- [x] Item: Pistolet (Raycast shooting, Left Click)
- [x] Push mechanics (BasicRigidBodyPush + PushableObject)
- [ ] Item: Pioche (Destructible walls)

### 🔴 Phase 3: Stages
- [ ] Stage 0: Tutorial room
- [ ] Stage 1: Falling platforms (timer + reset)
- [ ] Stage 2: Door maze (5 rooms logic)
- [ ] Stage 3: Shooting gallery (target AI)
- [ ] Stage 4: Riddle + stele
- [ ] Stage 5: Maze + destructible blocks
- [ ] Stage 6: Darkness navigation
- [ ] Stage 7: Amulette finale

### 🟣 Phase 4: Polish
- [ ] Main Menu & Pause Menu
- [ ] Timer system (global)
- [ ] Leaderboard (save/load)
- [ ] Sound Design & Music
- [ ] Lighting & Atmosphere (URP)
- [ ] Playtesting & Balancing

---

## 🏗️ Project Structure

```
Assets/
├── _Data/                          # ScriptableObjects (Item definitions)
│   ├── Data_Amulette.asset
│   ├── Data_Lampe.asset
│   ├── Data_Pistolet.asset
│   ├── Data_Sac.asset
│   └── Data_pioche.asset
│
├── _Scenes/
│   └── SampleScene.unity           # Main development scene
│
└── _Scripts/
    ├── GameControls.cs                # Auto-generated Input Actions wrapper
    ├── GameControls.inputactions      # Input Action Asset (WASD, Jump, Shoot, etc.)
    ├── IInteractable.cs               # Interface for interactive objects (doors, levers)
    ├── InputManager.cs                # Singleton handling all inputs
    ├── Interactor.cs                  # (Legacy) Raycast system - kept for future use
    ├── Inventory.cs                   # Player inventory (slots, add/remove, backpack)
    ├── ItemData.cs                    # ScriptableObject definition for items
    ├── ItemDropper.cs                 # Drop system (G key) - spawns items in world
    ├── ItemPickup.cs                  # Automatic pickup via OnTriggerEnter
    ├── InteractionSystemHelper.cs     # Auto-configure pickup objects
    ├── BackpackVisual.cs              # Visual backpack on player when equipped
    ├── Flashlight.cs                  # Flashlight system (F key toggle, spotlight)
    ├── Gun.cs                         # Gun system (Left Click, raycast shooting)
    ├── Target.cs                      # Shootable targets with health and events
    ├── MovingTarget.cs                # Moving targets with different patterns
    ├── PushableObject.cs              # Auto-configure pushable objects (Rigidbody)
    ├── PlayerPushSetup.cs             # Setup player push system
    ├── PlayerController.cs            # Movement, jump, camera rotation
    └── PlayerSetupChecker.cs          # Debug tool - validates player config
```

---

## 🧩 Key Interfaces & Classes

| Class/Interface | Role |
| :--- | :--- |
| `InputManager` | Singleton — centralized input reading |
| `PlayerController` | Handles movement, jump, camera with CharacterController |
| `Inventory` | Manages item list, max slots, backpack expansion |
| `ItemData` | ScriptableObject — item properties (name, icon, isBackpack) |
| `ItemPickup` | MonoBehaviour — automatic pickup via OnTriggerEnter |
| `ItemDropper` | Drops last inventory item in front of player (G key) |
| `BackpackVisual` | Displays backpack model on player when equipped |
| `Flashlight` | Toggles spotlight with F key (requires Lampe item) |
| `Gun` | Shooting system with Left Click (requires Pistolet item) |
| `Target` | Shootable targets with health, events, visual feedback |
| `MovingTarget` | Targets with movement patterns (horizontal, vertical, random) |
| `PushableObject` | Auto-configures pushable objects with Rigidbody |
| `PlayerPushSetup` | Configures player to push objects (BasicRigidBodyPush) |
| `InteractionSystemHelper` | Auto-configures pickups (collider, trigger, layer) |
| `IInteractable` | *(Optional)* Interface for future interactive objects (doors, levers) |

---

## 📦 Scene Hierarchy (SampleScene)

```
SampleScene
├── Main Camera          # CinemachineBrain + URP Camera
├── Virtual Camera       # Cinemachine 3rd Person Follow → CameraRoot
├── Directional Light
├── Global Volume        # URP Post-processing
├── Player               # Layer: 3 | CharacterController, PlayerController, Inventory, Interactor
│   └── CameraRoot       # Cinemachine Follow/LookAt target
├── GameManager          # InputManager singleton
├── Floor                # MeshCollider
├── Canvas               # Screen Space Overlay
│   └── Image            # Crosshair (10x10 centered)
├── EventSystem          # InputSystemUIInputModule
└── Pickups (Layer: 6 - Interactable)
    ├── Pickup_amulette  # SphereCollider (trigger)
    ├── Pickup_sac       # BoxCollider (trigger)
    ├── Pickup_pioche    # BoxCollider (trigger)
    ├── Pickup_lampe     # CapsuleCollider (trigger)
    └── Pickup_pistolet  # BoxCollider (trigger)
```

---

## ⚙️ Layer Configuration

| Layer | Name | Usage |
| :--- | :--- | :--- |
| 3 | Player | Player character (ignored by camera collision) |
| 6 | Interactable | All pickup items (for raycast filtering) |

---

*Project developed by Kessel DIAROUMEYE.*