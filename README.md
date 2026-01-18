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
| **Use Pickaxe** | `Right Click` |
| **Inventory** | `I` |
| **Menu / Pause** | `Esc` |

> **Note:** Items are picked up automatically when walking over them. Press `G` to drop the last item. Press `F` to toggle flashlight (requires Lampe). Left Click to shoot (requires Pistolet). Right Click to break walls (requires Pioche).

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

### 🟢 Phase 2: Core Systems (COMPLETED ✅)
- [x] **Interaction System** (Automatic trigger-based pickup)
- [x] **Inventory System** (2 slots + backpack extension to 5)
- [x] **Item Data** (ScriptableObjects for all items)
- [x] **Item Pickup System** (OnTriggerEnter with auto-detection)
- [x] **Item Drop System** (G key - spawns items in world)
- [x] **Item: Sac** (Backpack +3 slots with 3D visual model)
- [x] **Item: Lampe** (Flashlight toggle with F key, spotlight + visual model)
- [x] **Item: Pistolet** (Raycast shooting, Left Click, bullet trails, impact effects)
- [x] **Item: Pioche** (Destructible walls, Right Click, swing animation)
- [x] **Push Mechanics** (BasicRigidBodyPush + PushableObject auto-setup)
- [x] **GameManager** (Singleton with timer, pause, game state management)
- [x] **SettingsManager** (Mouse sensitivity, volume, quality settings with PlayerPrefs)
- [x] **LeaderboardManager** (Top 10 scores with JSON persistence)
- [x] **ItemVisibilityManager** (Show/hide equipped item models)
- [x] **UI: Main Menu** (Play, Settings, Leaderboard, Credits, Quit)
- [x] **UI: Pause Menu** (Resume, Restart, Settings, Quit - with input blocking)
- [x] **UI: Settings Panel** (Sliders for sensitivity/volume, quality dropdown)
- [x] **UI: Leaderboard Panel** (Display top 10 with names and times)
- [x] **UI: EndGame Screen** (Final time, name input, top score detection, restart/menu options)
- [x] **EndGameTrigger** (Win zone collision detection)
- [x] **Input Blocking** (All inputs blocked during pause and game end states)
- [x] **Targets System** (Shootable targets with health, events, visual feedback)
- [x] **Moving Targets** (Horizontal, vertical, and random movement patterns)
- [x] **Destructible Walls** (Health system, visual feedback, pickaxe interaction)

### 🟡 Phase 3: Level Design & Stages (37.5% Complete 🚧)
- [x] **Stage 0: Tutorial Room** ✅
  - [x] TutorialSign.cs - Billboard world-space text
  - [x] DoorTrigger.cs - Scene/teleport/visual doors
  - [x] SpawnPoint.cs - Player spawn system
  - [x] KillZone.cs - Respawn zones
  - [x] StageBuilder.cs - Quick room generation
  - [x] Setup guide: `STAGE_0_SETUP.md`

- [x] **Stage 1: Falling Platforms** ✅
  - [x] FallingPlatform.cs - Timed fall with countdown
  - [x] 3-second timer with visual feedback
  - [x] Fall respawn to Stage 0 (KillZone)
  - [x] Platform auto-reset after 5 seconds
  - [x] Setup guide: `STAGE_1_SETUP.md`

- [ ] **Stage 2: Door Maze (5 Rooms)**
  - [ ] 5 identical room layouts
  - [ ] Door teleportation logic between rooms
  - [ ] Room number UI indicator
  - [ ] Pistolet placement in Room 5
  - [ ] Exit door to Stage 3 (from Room 4)

- [ ] **Stage 3: Shooting Gallery**
  - [ ] 5 target sequences (MovingTarget script ready ✅)
  - [ ] Platform activation on target destroy
  - [ ] Progressive difficulty (static → random movement)
  - [ ] Exit door to Stage 4

- [ ] **Stage 4: Riddle Room**
  - [ ] Room with multiple objects
  - [ ] Riddle inscription (TextMeshPro)
  - [ ] Stele with trigger (correct item detection)
  - [ ] Door opening logic

- [x] **Stage 5: Destructible Maze** ✅
  - [x] MazeBuilder.cs - Generates 5×20 maze
  - [x] MapDisplay.cs - Wall-mounted map texture
  - [x] Destructible/solid wall system (DestructibleWall ✅)
  - [x] Auto-placement: Sac, Pioche, Lampe
  - [x] Hidden Lampe in breakable block
  - [x] Map visual (green = path, black = wall)
  - [x] Setup guide: `STAGE_5_SETUP.md`

- [ ] **Stage 6: Darkness Zone**
  - [ ] Total darkness (no lights)
  - [ ] Floor with holes (KillZone ✅)
  - [ ] Narrow safe path navigation
  - [ ] Flashlight requirement check

- [ ] **Stage 7: Final Room**
  - [ ] Empty room with door
  - [ ] Amulette slot on door (DoorTrigger with requireItem ✅)
  - [ ] Inscription hint (TutorialSign ✅)
  - [ ] Door opening to EndGameZone

- [x] **Win Zone** ✅
  - [x] EndGameTrigger placement (already functional from Phase 2)

### 🟣 Phase 4: Polish & Finalization
- [x] **Main Menu & Pause Menu** ✅ (Fully functional with all buttons)
- [x] **Timer system (global)** ✅ (GameManager tracks time, TimerUI displays)
- [x] **Leaderboard (save/load)** ✅ (Top 10 with JSON persistence)
- [x] **End Game screen** ✅ (Time display, name input, save score, restart/quit)
- [ ] **Sound Design & Music**
  - [ ] Footsteps, jump, land SFX
  - [ ] Item pickup sounds
  - [ ] Gun shot, pickaxe hit, flashlight toggle
  - [ ] UI button clicks
  - [ ] Ambient music (dark atmosphere)
  - [ ] Victory jingle
- [ ] **Lighting & Atmosphere (URP)**
  - [ ] Global Volume post-processing
  - [ ] Shadows and lighting per stage
  - [ ] Darkness zones (Stage 6)
  - [ ] Particle effects (dust, torch flames)
- [ ] **Playtesting & Balancing**
  - [ ] Difficulty tuning (platform timers, target speeds)
  - [ ] Player movement feel
  - [ ] Checkpoint system (optional)
- [ ] **Build & Deployment**
  - [ ] Windows standalone build
  - [ ] Performance optimization
  - [ ] Bug fixes

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
    ├── GameControls.inputactions      # Input Action Asset (WASD, Jump, Shoot, Pickaxe, etc.)
    │
    ├── ──── MANAGERS ────
    ├── GameManager.cs                 # Singleton - timer, pause, game states, scene transitions
    ├── InputManager.cs                # Singleton - centralized input handling
    ├── SettingsManager.cs             # Singleton - settings with PlayerPrefs persistence
    ├── LeaderboardManager.cs          # Singleton - top 10 scores with JSON save/load
    ├── ItemVisibilityManager.cs       # Singleton - show/hide equipped item models
    │
    ├── ──── PLAYER ────
    ├── PlayerController.cs            # Movement, jump, run, camera rotation (input blocking)
    ├── Inventory.cs                   # Inventory system (2-5 slots, add/remove, backpack)
    ├── PlayerPushSetup.cs             # Auto-setup player push capability
    ├── PlayerSetupChecker.cs          # Debug tool - validates player configuration
    │
    ├── ──── ITEMS & INTERACTION ────
    ├── ItemData.cs                    # ScriptableObject - item definitions
    ├── ItemPickup.cs                  # Automatic pickup via OnTriggerEnter
    ├── ItemDropper.cs                 # Drop system (G key) - spawns items in world
    ├── InteractionSystemHelper.cs     # Auto-configure pickup objects (collider, trigger)
    ├── BackpackVisual.cs              # Visual backpack model on player
    ├── Flashlight.cs                  # Flashlight system (F key, spotlight, visual model)
    ├── Gun.cs                         # Gun system (Left Click, raycast, bullet trail, effects)
    ├── Pickaxe.cs                     # Pickaxe system (Right Click, break walls, swing animation)
    ├── IInteractable.cs               # Interface for interactive objects (doors, levers)
    ├── Interactor.cs                  # (Legacy) Raycast system - kept for future use
    │
    ├── ──── OBJECTS & WORLD ────
    ├── Target.cs                      # Shootable targets with health, events, feedback
    ├── MovingTarget.cs                # Moving targets (horizontal, vertical, random patterns)
    ├── DestructibleWall.cs            # Breakable walls for pickaxe with health system
    ├── PushableObject.cs              # Auto-configure pushable objects (Rigidbody)
    ├── FallingPlatform.cs             # Timed falling platforms with shake & countdown
    ├── EndGameTrigger.cs              # Win zone collision detection
    │
    ├── ──── STAGE SYSTEMS ────
    ├── TutorialSign.cs                # Billboard world-space text signs (Stage 0)
    ├── DoorTrigger.cs                 # Multi-purpose doors (scene, teleport, visual)
    ├── SpawnPoint.cs                  # Player spawn locations with gizmos
    ├── KillZone.cs                    # Respawn zones for falls/deaths
    ├── StageBuilder.cs                # Quick room generation tool
    ├── MazeBuilder.cs                 # 5×20 destructible maze generator (Stage 5)
    ├── MapDisplay.cs                  # Wall-mounted map texture (Stage 5)
    │
    ├── ──── UI ────
    ├── MainMenuUI.cs                  # Main menu button handlers (Play, Settings, Leaderboard, Quit)
    ├── PauseMenuController.cs         # Pause menu handlers (Resume, Restart, Settings, Quit)
    ├── SettingsUI.cs                  # Settings panel (sensitivity, volume, quality)
    ├── LeaderboardUI.cs               # Leaderboard display (top 10 entries)
    ├── EndGameUI.cs                   # End game screen (time, name input, save, buttons)
    └── TimerUI.cs                     # Timer display during gameplay
```

---

## 🧩 Key Interfaces & Classes

### 🎮 Managers (Singletons)
| Class | Role |
| :--- | :--- |
| `GameManager` | Central game state, timer, pause, scene transitions, game end |
| `InputManager` | Centralized input reading (Move, Look, Jump, Run, etc.) |
| `SettingsManager` | Settings persistence (sensitivity, volume, quality) |
| `LeaderboardManager` | Top 10 score tracking with JSON save/load |
| `ItemVisibilityManager` | Show/hide equipped item models (Gun, Pickaxe, Flashlight) |

### 👤 Player & Movement
| Class | Role |
| :--- | :--- |
| `PlayerController` | Movement, jump, run, camera with CharacterController (input blocking) |
| `Inventory` | Item storage (2-5 slots), add/remove, backpack expansion |
| `PlayerPushSetup` | Configures player push capability (BasicRigidBodyPush) |

### 🎒 Items & Interaction
| Class | Role |
| :--- | :--- |
| `ItemData` | ScriptableObject — item properties (name, icon, type) |
| `ItemPickup` | Automatic pickup via OnTriggerEnter |
| `ItemDropper` | Drop last item in front of player (G key) |
| `BackpackVisual` | Displays backpack 3D model on player when equipped |
| `Flashlight` | Spotlight toggle with F key (requires Lampe) |
| `Gun` | Raycast shooting with Left Click (requires Pistolet) |
| `Pickaxe` | Break destructible walls with Right Click (requires Pioche) |
| `InteractionSystemHelper` | Auto-configure pickup objects (collider, trigger, layer) |

### 🌍 World Objects
| Class | Role |
| :--- | :--- |
| `Target` | Shootable targets with health, events, visual feedback |
| `MovingTarget` | Targets with movement patterns (horizontal, vertical, random) |
| `DestructibleWall` | Breakable walls with health system for pickaxe |
| `PushableObject` | Auto-configures pushable objects with Rigidbody |
| `FallingPlatform` | Timed falling platforms (Stage 1) |
| `EndGameTrigger` | Win zone collision detection (triggers LevelComplete) |

### 🖼️ UI Systems
| Class | Role |
| :--- | :--- |
| `MainMenuUI` | Main menu button handlers (Play, Settings, Leaderboard, Quit) |
| `PauseMenuController` | Pause menu handlers (Resume, Restart, Settings, Quit) |
| `SettingsUI` | Settings panel (sensitivity slider, volume, quality dropdown) |
| `LeaderboardUI` | Leaderboard display (top 10 entries with names and times) |
| `EndGameUI` | End game screen (final time, name input, save score, buttons) |
| `TimerUI` | Real-time timer display during gameplay |

### 🔌 Interfaces
| Interface | Role |
| :--- | :--- |
| `IInteractable` | *(Future)* Interface for interactive objects (doors, levers, switches) |

---

## 📦 Scene Hierarchy

### Main Menu (Scene 0)
```
MainMenu
├── Main Camera
├── Directional Light
├── Canvas (Screen Space - Overlay)
│   ├── MainMenuPanel
│   │   ├── Title (TextMeshPro)
│   │   ├── PlayButton
│   │   ├── SettingsButton
│   │   ├── LeaderboardButton
│   │   ├── CreditsButton
│   │   └── QuitButton
│   ├── SettingsPanel (initially hidden)
│   │   ├── SensitivitySlider
│   │   ├── VolumeSlider
│   │   ├── QualityDropdown
│   │   ├── ApplyButton
│   │   └── BackButton
│   ├── LeaderboardPanel (initially hidden)
│   │   ├── Title
│   │   ├── EntryList (10 entries)
│   │   └── BackButton
│   └── CreditsPanel (initially hidden)
│       └── BackButton
├── GameManager (MainMenuUI, SettingsManager, LeaderboardManager)
└── EventSystem
```

### Game Scene (Scene 1)
```
GameScene
├── Main Camera          # CinemachineBrain + URP Camera
├── Virtual Camera       # Cinemachine 3rd Person Follow → CameraRoot
├── Directional Light
├── Global Volume        # URP Post-processing
├── Player               # Layer: 3 | CharacterController, PlayerController, Inventory
│   ├── CameraRoot       # Cinemachine Follow/LookAt target
│   ├── Flashlight (child of CameraRoot - spotlight)
│   ├── GunModel (child of Player)
│   ├── PickaxeModel (child of Player)
│   ├── FlashlightModel (child of Player)
│   └── BackpackModel (child of Player)
│
├── GameManager          # GameManager, InputManager, SettingsManager, LeaderboardManager, ItemVisibilityManager
│
├── Canvas (Screen Space - Overlay)
│   ├── Crosshair (Image - centered)
│   ├── TimerUI (TextMeshPro - top center)
│   ├── InventoryUI (Panel - top left)
│   ├── PauseMenuPanel (initially hidden)
│   │   ├── ResumeButton
│   │   ├── RestartButton
│   │   ├── SettingsButton
│   │   └── QuitButton
│   ├── SettingsPanel (initially hidden)
│   │   ├── Sliders & Dropdowns
│   │   ├── ApplyButton
│   │   └── BackButton
│   └── EndGamePanel (initially hidden)
│       ├── FinalTimeText
│       ├── CongratsText
│       ├── NameInputField
│       ├── SubmitButton
│       ├── SkipButton
│       └── TryAgainButton
│
├── EventSystem          # InputSystemUIInputModule
├── Floor                # MeshCollider
├── EndGameZone          # EndGameTrigger (OnTriggerEnter)
│
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