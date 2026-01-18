# Prefabs & Scene Setup Guide

## Overview
This guide explains what prefabs you need to create and what GameObjects/items should be in each stage scene.

---

## Essential Prefabs to Create First

These prefabs should be created once and reused in every scene:

### 1. Player Prefab
**Name:** `Player`
**Components Required:**
- CharacterController
- PlayerController (script)
- Inventory (script)
- AudioSource (optional, for footsteps)
- CameraRoot (child GameObject)
  - Main Camera (child of CameraRoot)
    - Camera component
    - AudioListener
    - UniversalAdditionalCameraData (URP)

**How to Create:**
1. Set up Player GameObject with all components
2. Add Main Camera as child under CameraRoot
3. Test that movement and camera work
4. Drag Player GameObject from Hierarchy to Project folder
5. This creates the prefab

**Why Prefab:** Ensures consistent player setup across all scenes, avoids the copy/paste issues you encountered.

---

### 2. Managers Prefab
**Name:** `GameManagers`
**Components:**
- InputManager (script)
- GameManager (script)
- SettingsManager (script)
- LeaderboardManager (script)
- ItemVisibilityManager (script)

**How to Create:**
1. Create empty GameObject named "GameManagers"
2. Add all 5 manager scripts to it
3. Configure default settings
4. Drag to Project folder to create prefab

**Why Prefab:** These singletons are needed in every scene. Using a prefab prevents missing manager errors.

---

### 3. EventSystem Prefab (for UI)
**Name:** `EventSystem`
**Components:**
- EventSystem
- StandaloneInputModule

**How to Create:**
1. In any scene, create UI element (Canvas automatically creates EventSystem)
2. Select the EventSystem GameObject
3. Drag to Project folder

**Why Prefab:** Needed for any scene with UI (riddles, pause menu, etc.)

---

## Item Prefabs

### Ancient Amulet Prefab
**Name:** `Item_AncientAmulet`
**Location:** Stage 0
**Components:**
- Cube (visual)
- ItemPickup (script)
  - Item Name: "Ancient Amulet"
  - Item Type: Collectible
- Collider (trigger)
- Rotate script (optional, for visual effect)

**Visual:** Golden glowing sphere or ornate object

---

### Pistol Prefab
**Name:** `Item_Pistol`
**Location:** Stage 2 (Room 5)
**Components:**
- Gun model or cube (visual)
- ItemPickup (script)
  - Item Name: "Pistol"
  - Item Type: Weapon
- Collider (trigger)

**Visual:** Gun shape, dark gray/black

---

### Flashlight Prefab
**Name:** `Item_Flashlight`
**Location:** Stage 5 (Maze)
**Components:**
- Cylinder or flashlight model (visual)
- ItemPickup (script)
  - Item Name: "Flashlight"
  - Item Type: Tool
- Collider (trigger)
- Point Light (child)
  - Range: 15
  - Intensity: 2
  - Initially disabled

**Visual:** Flashlight or torch shape, metallic

---

## Stage-by-Stage Setup Guide

---

## Stage 0: Tutorial Room

### Scene Name: `Stage_0_Tutorial`

### Required GameObjects:
1. **Player Prefab** ✓
2. **GameManagers Prefab** ✓
3. **Directional Light**
4. **Stage_0_TutorialRoom** (built by StageBuilder)
   - Floor
   - Walls
   - Tutorial Signs (4-5 signs)
   - Pushable Block
   - Ancient Amulet (Item Prefab)
   - Exit Door

### Items to Place:
- **Ancient Amulet** (Item Prefab)
  - Position: On pedestal in room
  - Critical: Player must pick this up!

### Builder Script:
- Create empty GameObject
- Add `StageBuilder` component
- Context menu: "Build Tutorial Room"

### How to Set Up:
```
1. Create new scene: Stage_0_Tutorial
2. Drag Player prefab into scene
3. Drag GameManagers prefab into scene
4. Add Directional Light
5. Create empty GameObject "TutorialRoomBuilder"
6. Add StageBuilder component
7. Right-click > Build Tutorial Room
8. Verify Ancient Amulet spawned on pedestal
```

---

## Stage 1: Falling Platforms

### Scene Name: `Stage_1_FallingPlatforms`

### Required GameObjects:
1. **Player Prefab** ✓
2. **GameManagers Prefab** ✓
3. **Directional Light**
4. **SpawnPoint_Stage1**
5. **PlatformLayout** (manual or builder)
   - 10-15 FallingPlatform objects
6. **KillZone** (below platforms)
7. **Exit Door**

### Items to Place:
- None

### Builder Script:
- Manual creation recommended
- Or create custom builder

### How to Set Up:
```
1. Create new scene: Stage_1_FallingPlatforms
2. Drag Player prefab into scene
3. Drag GameManagers prefab into scene
4. Create SpawnPoint at start
5. Create falling platforms manually:
   - Cube (3x0.5x3)
   - Add FallingPlatform script
   - Position in path across gap
6. Create KillZone below (Y = -20)
7. Create Exit Door at end
```

---

## Stage 2: Door Maze

### Scene Name: `Stage_2_DoorMaze`

### Required GameObjects:
1. **Player Prefab** ✓
2. **GameManagers Prefab** ✓
3. **EventSystem Prefab** ✓ (for RoomNumberUI)
4. **Directional Light**
5. **Stage_2_DoorMaze** (built by MazeRoomBuilder)
   - Room 1 (2 doors)
   - Room 2 (3 doors)
   - Room 3 (4 doors)
   - Room 4 (5 doors)
   - Room 5 (5 doors + Pistol)
6. **RoomNumberUI** (auto-created or manual)

### Items to Place:
- **Pistol** (Item Prefab)
  - Position: Room 5, on pedestal
  - Needed for Stage 3

### Builder Script:
- Add `MazeRoomBuilder` component
- Context menu: "Build Maze"

### How to Set Up:
```
1. Create new scene: Stage_2_DoorMaze
2. Drag Player prefab into scene
3. Drag GameManagers prefab into scene
4. Drag EventSystem prefab into scene
5. Create empty GameObject "MazeRoomBuilder"
6. Add MazeRoomBuilder component
7. Configure: Place Pistol in Room 5 ✓
8. Right-click > Build Maze
9. Verify Pistol spawned in Room 5
```

---

## Stage 3: Shooting Gallery

### Scene Name: `Stage_3_ShootingGallery`

### Required GameObjects:
1. **Player Prefab** ✓
2. **GameManagers Prefab** ✓
3. **Directional Light**
4. **Stage_3_ShootingGallery** (manual creation recommended)
   - 5 TargetSequence groups
   - Each sequence has 3-5 targets
   - Platform for each sequence
   - Exit door

### Items to Place:
- None (Pistol should be in inventory from Stage 2)

### Builder Script:
- Manual creation recommended
- Create 5 areas with targets

### How to Set Up:
```
1. Create new scene: Stage_3_ShootingGallery
2. Drag Player prefab into scene
3. Drag GameManagers prefab into scene
4. Create floor and walls
5. Create 5 target sequences:
   - Create parent GameObject "Sequence_1"
   - Add TargetSequence script
   - Create 3-5 Target objects as children
   - Assign platform to activate
6. Create exit door (initially disabled)
7. Add ShootingGalleryManager
8. Assign all sequences to manager
```

---

## Stage 4: Riddle Room

### Scene Name: `Stage_4_RiddleRoom`

### Required GameObjects:
1. **Player Prefab** ✓
2. **GameManagers Prefab** ✓
3. **EventSystem Prefab** ✓ (for RiddleUI)
4. **Directional Light**
5. **Stage_4_RiddleRoom** (built by RiddleRoomBuilder)
   - Floor
   - 3 Steles with riddles
   - Exit Platform
   - Exit Door
6. **RiddleUI_Manager** (for answer input)

### Items to Place:
- None

### Builder Script:
- Add `RiddleRoomBuilder` component
- Context menu: "Build Riddle Room"

### How to Set Up:
```
1. Create new scene: Stage_4_RiddleRoom
2. Drag Player prefab into scene
3. Drag GameManagers prefab into scene
4. Drag EventSystem prefab into scene
5. Create empty GameObject "RiddleRoomBuilder"
6. Add RiddleRoomBuilder component
7. Right-click > Build Riddle Room
8. Create empty GameObject "RiddleUI_Manager"
9. Add RiddleUI component
10. Enable Auto Create UI ✓
```

---

## Stage 5: Destructible Maze

### Scene Name: `Stage_5_DestructibleMaze`

### Required GameObjects:
1. **Player Prefab** ✓
2. **GameManagers Prefab** ✓
3. **Directional Light**
4. **Stage_5_Maze** (built by MazeBuilder)
   - 5x20 maze grid
   - Destructible walls
   - Flashlight item
   - Exit door
5. **MapDisplay** (optional, shows maze map)

### Items to Place:
- **Flashlight** (Item Prefab)
  - Position: Placed by MazeBuilder automatically
  - Needed for Stage 6

### Builder Script:
- Add `MazeBuilder` component
- Context menu: "Build Maze"

### How to Set Up:
```
1. Create new scene: Stage_5_DestructibleMaze
2. Drag Player prefab into scene
3. Drag GameManagers prefab into scene
4. Create empty GameObject "MazeBuilder"
5. Add MazeBuilder component
6. Configure:
   - Place Flashlight ✓
   - Place Pistol Ammo ✓ (optional)
7. Right-click > Build Maze
8. Verify Flashlight spawned in maze
9. (Optional) Create MapDisplay on wall
```

---

## Stage 6: Darkness Zone

### Scene Name: `Stage_6_DarknessZone`

### Required GameObjects:
1. **Player Prefab** ✓
2. **GameManagers Prefab** ✓
3. **Directional Light** (will be overridden by darkness)
4. **Stage_6_DarknessZone** (built by DarknessZoneBuilder)
   - Narrow path (15 segments)
   - Kill zone below
   - Exit platform
   - Exit door
5. **DarknessZone_Manager** (manages lighting)

### Items to Place:
- None (Flashlight should be in inventory from Stage 5)

### Builder Script:
- Add `DarknessZoneBuilder` component
- Context menu: "Build Darkness Zone"

### How to Set Up:
```
1. Create new scene: Stage_6_DarknessZone
2. Drag Player prefab into scene
3. Drag GameManagers prefab into scene
4. Create empty GameObject "DarknessZoneBuilder"
5. Add DarknessZoneBuilder component
6. Configure:
   - Number of Segments: 15
   - Path Width: 2
   - Randomize Path ✓
7. Right-click > Build Darkness Zone
8. DarknessZone component auto-added to parent
```

---

## Stage 7: Final Room

### Scene Name: `Stage_7_FinalRoom`

### Required GameObjects:
1. **Player Prefab** ✓
2. **GameManagers Prefab** ✓
3. **Directional Light**
4. **Stage_7_FinalRoom** (built by FinalRoomBuilder)
   - Floor
   - Amulet Slot
   - Pedestal
   - Final Door
   - Victory Zone
   - Torches (4)

### Items to Place:
- None (Ancient Amulet should be in inventory from Stage 0)

### Builder Script:
- Add `FinalRoomBuilder` component
- Context menu: "Build Final Room"

### How to Set Up:
```
1. Create new scene: Stage_7_FinalRoom
2. Drag Player prefab into scene
3. Drag GameManagers prefab into scene
4. Create empty GameObject "FinalRoomBuilder"
5. Add FinalRoomBuilder component
6. Configure:
   - Add Pedestal ✓
   - Add Torches ✓
   - Number of Torches: 4
7. Right-click > Build Final Room
8. Verify Amulet slot created with door reference
```

---

## Complete Item Flow

### Ancient Amulet
- **Spawn:** Stage 0 (Tutorial Room)
- **Carried Through:** All stages (1-6)
- **Used:** Stage 7 (Final Room door slot)
- **Critical:** MUST pick up in Stage 0!

### Pistol
- **Spawn:** Stage 2 (Door Maze, Room 5)
- **Used:** Stage 3 (Shooting Gallery)
- **Optional for:** Stage 5 (can shoot walls faster)

### Flashlight
- **Spawn:** Stage 5 (Destructible Maze)
- **Used:** Stage 6 (Darkness Zone)
- **Strongly Recommended:** Makes Stage 6 much easier

---

## Scene Checklist Template

Use this checklist for every new scene:

```
[ ] Player prefab added to scene
[ ] GameManagers prefab added to scene
[ ] EventSystem prefab added (if UI needed)
[ ] Directional Light added
[ ] SpawnPoint created and positioned
[ ] Builder script added (if applicable)
[ ] Builder executed successfully
[ ] Required items placed (check stage requirements)
[ ] Exit door/trigger added
[ ] Scene added to Build Settings
[ ] Scene tested in Play Mode
[ ] Player can move and camera works
[ ] Items can be picked up (if applicable)
[ ] Exit/progression works
```

---

## Common Setup Errors to Avoid

### Error 1: Missing Managers
**Symptom:** NullReferenceException errors
**Solution:** Always add GameManagers prefab to scene

### Error 2: No EventSystem
**Symptom:** UI buttons don't work
**Solution:** Add EventSystem prefab when using UI

### Error 3: Camera Missing URP Data
**Symptom:** URP warning about camera data
**Solution:** Use GameObject > HTI Games > Fix All Cameras (URP)

### Error 4: Items Not Spawning
**Symptom:** Items missing from scene
**Solution:**
- Check builder settings (Place Pistol, Place Flashlight flags)
- Verify item prefabs exist
- Check builder debug logs

### Error 5: Player Copying Issues
**Symptom:** Player works in one scene, breaks in another
**Solution:** Use Player prefab, don't copy/paste raw GameObject

---

## Build Settings Setup

### Add All Scenes to Build
1. File > Build Settings
2. Add scenes in order:
   - Stage_0_Tutorial
   - Stage_1_FallingPlatforms
   - Stage_2_DoorMaze
   - Stage_3_ShootingGallery
   - Stage_4_RiddleRoom
   - Stage_5_DestructibleMaze
   - Stage_6_DarknessZone
   - Stage_7_FinalRoom

3. Verify scene indices match stage numbers

---

## Testing Workflow

### Full Playthrough Test
1. Start at Stage 0
2. Pick up Ancient Amulet
3. Progress through Stages 1-2
4. Pick up Pistol in Stage 2
5. Use Pistol in Stage 3
6. Progress through Stage 4
7. Pick up Flashlight in Stage 5
8. Use Flashlight in Stage 6
9. Use Ancient Amulet in Stage 7
10. Verify victory!

### Quick Stage Test
To test individual stage:
1. Open stage scene
2. If testing Stage 7, manually add Amulet to inventory:
   - Inventory inspector > Add Item > "Ancient Amulet"
3. If testing Stage 3, add Pistol to inventory
4. If testing Stage 6, add Flashlight to inventory

---

## Summary: Essential Prefabs

| Prefab Name | Used In | Why Needed |
|-------------|---------|------------|
| Player | All stages | Consistent player setup |
| GameManagers | All stages | Singleton managers |
| EventSystem | Stages 2, 4 | UI interaction |
| Item_AncientAmulet | Stage 0 | Final victory item |
| Item_Pistol | Stage 2 | Shooting mechanic |
| Item_Flashlight | Stage 5 | Darkness navigation |

---

## Summary: Stage Requirements

| Stage | Player | Managers | EventSystem | Items | Builder |
|-------|--------|----------|-------------|-------|---------|
| 0 | ✓ | ✓ | - | Amulet | StageBuilder |
| 1 | ✓ | ✓ | - | - | Manual |
| 2 | ✓ | ✓ | ✓ | Pistol | MazeRoomBuilder |
| 3 | ✓ | ✓ | - | - | Manual |
| 4 | ✓ | ✓ | ✓ | - | RiddleRoomBuilder |
| 5 | ✓ | ✓ | - | Flashlight | MazeBuilder |
| 6 | ✓ | ✓ | - | - | DarknessZoneBuilder |
| 7 | ✓ | ✓ | - | - | FinalRoomBuilder |

---

## Next Steps

1. **Create the 3 essential prefabs** (Player, GameManagers, EventSystem)
2. **Create the 3 item prefabs** (Amulet, Pistol, Flashlight)
3. **Set up Stage 0** following the guide
4. **Test that prefabs work** by creating Stage 1
5. **Continue building stages in order** (0→1→2→3→4→5→6→7)
6. **Test full playthrough** when all stages complete

Good luck! 🎮
