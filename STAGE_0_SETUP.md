# 🎓 Stage 0: Tutorial Room - Setup Guide

## 📋 Overview
Stage 0 is the tutorial room where players learn basic controls. The player falls from the sky, reads tutorial signs, pushes blocks to create a path, picks up the **Amulette**, and exits through a door to Stage 1.

---

## 🛠️ Scripts Created
- ✅ `TutorialSign.cs` - World-space text with billboard effect
- ✅ `DoorTrigger.cs` - Doors with scene transition, teleport, or visual animation
- ✅ `SpawnPoint.cs` - Player spawn locations with gizmos
- ✅ `KillZone.cs` - Respawn zones for falls
- ✅ `StageBuilder.cs` - Quick room builder helper

---

## 🏗️ Step-by-Step Setup

### 1. Create the Room Structure

#### Option A: Using StageBuilder (Quick)
1. Create empty GameObject: `GameObject → Create Empty`
2. Name it: `StageBuilder_Stage0`
3. Add component: `StageBuilder`
4. Configure in Inspector:
   - Room Size: `(20, 6, 20)`
   - Room Name: `Stage0_TutorialRoom`
   - Floor Color: Gray `(0.3, 0.3, 0.3)`
   - Wall Color: Brown `(0.5, 0.4, 0.3)`
5. Right-click script → **Build Room**
6. Delete the StageBuilder GameObject (room is created)

#### Option B: Manual Setup
1. Create Cube for floor: Scale `(20, 0.5, 20)`, Position `(0, 0, 0)`
2. Create 4 Cubes for walls
3. Create Cube for ceiling (optional)

---

### 2. Add Player Spawn Point

1. Create empty GameObject: `GameObject → Create Empty`
2. Name it: `SpawnPoint_Stage0`
3. Position: `(0, 10, 0)` (player falls from sky)
4. Add component: `SpawnPoint`
5. Configure:
   - ✅ Is Default Spawn: `true`
   - Spawn ID: `Stage0`
   - Gizmo Color: Green

6. **Move existing Player to this position** or configure `spawnOnStart` if using prefab

---

### 3. Add Tutorial Signs

Create 5 tutorial signs with different instructions:

#### Sign 1: Movement
1. Create empty GameObject: `TutorialSign_Movement`
2. Position: `(-8, 1.5, 8)` (near spawn)
3. Add component: `TutorialSign`
4. Configure:
   - Tutorial Text:
     ```
     BIENVENUE DANS LE TEMPLE !

     WASD / ZQSD : Se déplacer
     Souris : Regarder autour
     ```

#### Sign 2: Jump
1. Create: `TutorialSign_Jump`
2. Position: `(-5, 1.5, 5)`
3. Tutorial Text:
   ```
   ESPACE : Sauter
   SHIFT : Courir
   ```

#### Sign 3: Push
1. Create: `TutorialSign_Push`
2. Position: `(0, 1.5, 5)`
3. Tutorial Text:
   ```
   Marchez vers les blocs pour les POUSSER
   Créez un chemin vers la porte !
   ```

#### Sign 4: Pickup
1. Create: `TutorialSign_Pickup`
2. Position: `(5, 1.5, 5)`
3. Tutorial Text:
   ```
   Marchez sur les objets pour les RAMASSER
   G : Lâcher un objet
   I : Inventaire
   ```

#### Sign 5: Exit
1. Create: `TutorialSign_Exit`
2. Position: `(8, 1.5, -8)` (near door)
3. Tutorial Text:
   ```
   TROUVEZ L'AMULETTE !
   Elle vous sera utile plus tard...

   Bonne chance ! 🏛️
   ```

---

### 4. Add Pushable Blocks

Create 3-4 blocks that the player must push to reach the door:

1. Create Cube: `PushableBlock_1`
2. Scale: `(2, 2, 2)`
3. Position: `(3, 1, 0)` (blocking the path)
4. Add component: `PushableObject`
5. Configure (auto-setup will handle Rigidbody)

**Repeat for blocks 2, 3, 4** at strategic positions to create a puzzle.

**Layout idea:**
- Block 1 blocks the path to the Amulette
- Block 2 blocks the path to the door
- Player must push them aside to progress

---

### 5. Place the Amulette

1. Find existing: `Pickup_amulette` (or create new)
2. Position: `(0, 1, -5)` (accessible after pushing blocks)
3. Ensure it has:
   - `ItemPickup` component
   - Trigger collider
   - Layer: `Interactable` (6)

---

### 6. Add Exit Door

1. Create empty GameObject: `Door_ToStage1`
2. Position: `(8, 3, -8)` (elevated, requires blocks to reach)
3. Add component: `BoxCollider`
   - ✅ Is Trigger: `true`
   - Size: `(3, 4, 1)`
4. Add component: `DoorTrigger`
5. Configure:
   - Door Type: `Scene Transition`
   - Target Scene Index: `1` (or next stage scene)
   - ❌ Require Item: `false` (tutorial is free)

**Optional: Add visual door model**
- Create Cube as child
- Scale: `(3, 4, 0.2)`
- Material: Wood/Metal
- Assign to DoorTrigger → Door Model

---

### 7. Add Kill Zone (Optional)

In case player falls off the room:

1. Create empty GameObject: `KillZone_Stage0`
2. Position: `(0, -10, 0)` (below the room)
3. Add component: `BoxCollider`
   - ✅ Is Trigger: `true`
   - Size: `(50, 2, 50)` (large area)
4. Add component: `KillZone`
5. Configure:
   - ✅ Use Default Spawn: `true`
   - Respawn Message: `💀 Tombé dans le vide ! Respawn...`

---

### 8. Lighting & Atmosphere

1. **Directional Light:**
   - Rotation: `(50, -30, 0)` (top-down light)
   - Intensity: `1.5`
   - Color: Warm white

2. **Global Volume (URP):**
   - Add Post-processing effects:
     - Vignette (slight darkness at edges)
     - Color Grading (desaturate slightly for ancient temple feel)

3. **Optional: Add torches**
   - Point lights with orange color
   - Particle systems for flames

---

## 🎮 Testing Checklist

- [ ] Player spawns at (0, 10, 0) and falls
- [ ] Tutorial signs are visible and readable
- [ ] Signs face the player (billboard)
- [ ] Blocks can be pushed
- [ ] Amulette can be picked up
- [ ] Door trigger works (loads next scene or shows log)
- [ ] Kill zone respawns player if they fall
- [ ] Inventory shows Amulette after pickup
- [ ] No collider errors in console

---

## 📦 Final Hierarchy

```
Stage0_TutorialRoom
├── Floor
├── Wall_North
├── Wall_South
├── Wall_East
├── Wall_West
├── Ceiling (optional)
│
├── SpawnPoint_Stage0
│
├── TutorialSign_Movement
├── TutorialSign_Jump
├── TutorialSign_Push
├── TutorialSign_Pickup
├── TutorialSign_Exit
│
├── PushableBlock_1
├── PushableBlock_2
├── PushableBlock_3
│
├── Pickup_amulette
│
├── Door_ToStage1
│   └── DoorModel (visual)
│
└── KillZone_Stage0
```

---

## 🚀 Next Steps

Once Stage 0 is complete:
1. Test full flow: Spawn → Read signs → Push blocks → Get Amulette → Exit
2. Adjust block positions if puzzle is too easy/hard
3. Move to **Stage 1: Falling Platforms**

---

## 💡 Tips

- Use **Scene View gizmos** to visualize:
  - Green sphere/arrow = SpawnPoint
  - Cyan wireframe = TutorialSign
  - Green wireframe = DoorTrigger
  - Red transparent = KillZone

- **Prefab workflow**: Once you create one TutorialSign, make it a prefab and duplicate for other signs

- **Layout tip**: Use top-down view (Scene View → Y axis) to plan block placement

---

✅ **Stage 0 is now ready!**
