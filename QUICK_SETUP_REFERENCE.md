# Quick Setup Reference

## 3 Essential Prefabs (Create Once, Use Everywhere)

### 1. Player Prefab
```
Player (GameObject)
├── CharacterController
├── PlayerController
├── Inventory
└── CameraRoot (child)
    └── Main Camera (child)
        ├── Camera
        ├── AudioListener
        └── UniversalAdditionalCameraData
```

### 2. GameManagers Prefab
```
GameManagers (GameObject)
├── InputManager
├── GameManager
├── SettingsManager
├── LeaderboardManager
└── ItemVisibilityManager
```

### 3. EventSystem Prefab
```
EventSystem (GameObject)
├── EventSystem
└── StandaloneInputModule
```

---

## Item Progression Through Game

```
STAGE 0 (Tutorial)
  └─→ Pick up: Ancient Amulet ⭐
       │
STAGE 1 (Falling Platforms)
  └─→ No items
       │
STAGE 2 (Door Maze)
  └─→ Pick up: Pistol 🔫
       │
STAGE 3 (Shooting Gallery)
  └─→ USE: Pistol 🔫
       │
STAGE 4 (Riddle Room)
  └─→ No items
       │
STAGE 5 (Destructible Maze)
  └─→ Pick up: Flashlight 🔦
       │
STAGE 6 (Darkness Zone)
  └─→ USE: Flashlight 🔦
       │
STAGE 7 (Final Room)
  └─→ USE: Ancient Amulet ⭐ → WIN! 🎉
```

---

## Every Scene Needs (Minimum)

1. **Player** (prefab)
2. **GameManagers** (prefab)
3. **Directional Light**
4. **SpawnPoint**
5. **Exit Door or Trigger**

---

## Stage-by-Stage Quick Setup

### Stage 0: Tutorial
```
✓ Player prefab
✓ GameManagers prefab
✓ Light
+ StageBuilder → "Build Tutorial Room"
+ Ancient Amulet item (auto-placed by builder)
```

### Stage 1: Falling Platforms
```
✓ Player prefab
✓ GameManagers prefab
✓ Light
+ Manual: Create falling platforms
+ KillZone below
+ Exit door
```

### Stage 2: Door Maze
```
✓ Player prefab
✓ GameManagers prefab
✓ EventSystem prefab (for UI)
✓ Light
+ MazeRoomBuilder → "Build Maze"
+ Pistol item (auto-placed in Room 5)
```

### Stage 3: Shooting Gallery
```
✓ Player prefab
✓ GameManagers prefab
✓ Light
+ Manual: Create 5 target sequences
+ ShootingGalleryManager
+ Exit door
```

### Stage 4: Riddle Room
```
✓ Player prefab
✓ GameManagers prefab
✓ EventSystem prefab (for UI)
✓ Light
+ RiddleRoomBuilder → "Build Riddle Room"
+ RiddleUI_Manager (with RiddleUI script)
```

### Stage 5: Destructible Maze
```
✓ Player prefab
✓ GameManagers prefab
✓ Light
+ MazeBuilder → "Build Maze"
+ Flashlight item (auto-placed by builder)
```

### Stage 6: Darkness Zone
```
✓ Player prefab
✓ GameManagers prefab
✓ Light
+ DarknessZoneBuilder → "Build Darkness Zone"
+ DarknessZone component (auto-added)
```

### Stage 7: Final Room
```
✓ Player prefab
✓ GameManagers prefab
✓ Light
+ FinalRoomBuilder → "Build Final Room"
+ Amulet slot (auto-created)
```

---

## Step-by-Step: Creating Your First Scene

### Example: Setting Up Stage 0

1. **Create Scene**
   - File > New Scene
   - Save As: `Stage_0_Tutorial`

2. **Add Essential Prefabs**
   - Drag `Player` prefab into scene
   - Drag `GameManagers` prefab into scene

3. **Add Light**
   - GameObject > Light > Directional Light

4. **Add Builder**
   - GameObject > Create Empty
   - Name: "TutorialRoomBuilder"
   - Add Component > StageBuilder

5. **Build Stage**
   - Right-click StageBuilder component
   - Select: "Build Tutorial Room"

6. **Verify**
   - Check Ancient Amulet spawned on pedestal
   - Enter Play Mode
   - Test player movement
   - Pick up Amulet
   - Check inventory

7. **Add to Build Settings**
   - File > Build Settings
   - Add Open Scenes
   - Verify scene index = 0

---

## Common Mistakes Checklist

### Before Testing Each Scene:
- [ ] Did I add Player prefab? (Not copy/paste Player!)
- [ ] Did I add GameManagers prefab?
- [ ] Did I add EventSystem (if UI needed)?
- [ ] Did I run the builder script?
- [ ] Is Directional Light in scene?
- [ ] Is scene added to Build Settings?

### If Player Doesn't Work:
- [ ] Is it a prefab instance (blue text in Hierarchy)?
- [ ] Does it have CharacterController component?
- [ ] Does Main Camera have URP Camera Data?
- [ ] Does scene have InputManager?

### If Items Don't Spawn:
- [ ] Did builder script run successfully?
- [ ] Check builder flags (Place Pistol, Place Flashlight)
- [ ] Check console for builder debug logs
- [ ] Verify item prefabs exist in project

---

## Testing Without Full Playthrough

To test a stage in isolation:

### Stage 3 (needs Pistol):
```
1. Open Stage 3 scene
2. Select Player in Hierarchy
3. Find Inventory component
4. In Inspector: Add Item "Pistol"
5. Play Mode - now you have Pistol
```

### Stage 6 (needs Flashlight):
```
1. Open Stage 6 scene
2. Select Player
3. Inventory > Add Item "Flashlight"
4. Play Mode
```

### Stage 7 (needs Ancient Amulet):
```
1. Open Stage 7 scene
2. Select Player
3. Inventory > Add Item "Ancient Amulet"
4. Play Mode
5. Press E at slot to place Amulet
```

---

## Build Order Recommendation

### Phase 1: Prefabs (Do This First!)
1. Create Player prefab
2. Create GameManagers prefab
3. Create EventSystem prefab
4. Create Item prefabs (Amulet, Pistol, Flashlight)

### Phase 2: Test Stage
5. Build Stage 0 (Tutorial)
6. Test Player prefab works
7. Test Ancient Amulet pickup
8. Fix any issues before continuing

### Phase 3: All Stages
9. Build Stages 1-7 in order
10. Test each stage individually
11. Test full playthrough (0→7)

---

## Final Checklist Before Build

- [ ] All 8 scenes created
- [ ] All scenes use prefabs (not copied GameObjects)
- [ ] All 3 items placed correctly
- [ ] All scenes in Build Settings (in order)
- [ ] Full playthrough tested
- [ ] All items collected in correct order
- [ ] Victory works in Stage 7

---

## File Structure

```
Project
├── Assets
│   ├── _Scripts
│   │   ├── (All your scripts)
│   ├── Prefabs
│   │   ├── Player.prefab ⭐
│   │   ├── GameManagers.prefab ⭐
│   │   ├── EventSystem.prefab ⭐
│   │   ├── Item_AncientAmulet.prefab
│   │   ├── Item_Pistol.prefab
│   │   └── Item_Flashlight.prefab
│   ├── Scenes
│   │   ├── Stage_0_Tutorial.unity
│   │   ├── Stage_1_FallingPlatforms.unity
│   │   ├── Stage_2_DoorMaze.unity
│   │   ├── Stage_3_ShootingGallery.unity
│   │   ├── Stage_4_RiddleRoom.unity
│   │   ├── Stage_5_DestructibleMaze.unity
│   │   ├── Stage_6_DarknessZone.unity
│   │   └── Stage_7_FinalRoom.unity
│   └── Materials (your materials)
```

---

## Priority Summary

### MUST CREATE (Core Prefabs):
1. ⭐ Player prefab
2. ⭐ GameManagers prefab

### SHOULD CREATE (Items):
3. Ancient Amulet prefab (required for victory)
4. Pistol prefab (required for Stage 3)
5. Flashlight prefab (makes Stage 6 easier)

### OPTIONAL:
6. EventSystem prefab (can auto-create per scene)
7. Door prefab (builders create basic doors)
8. Platform prefabs (can create manually)

---

## Quick Command Reference

### Create Prefab:
1. Set up GameObject in scene
2. Drag from Hierarchy to Project/Prefabs folder
3. Done! Blue text = prefab instance

### Use Prefab in Scene:
1. Drag prefab from Project to Hierarchy
2. Prefab automatically updates in all scenes

### Update Prefab:
1. Select prefab instance in scene (blue text)
2. Make changes
3. Inspector > Overrides > Apply All
4. All instances update automatically

---

Good luck with your game! 🎮✨
