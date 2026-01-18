# 🚪 Stage 2: Door Maze (5 Rooms) - Setup Guide

## 📋 Overview
Stage 2 is a confusing maze of 5 identical-looking rooms connected by doors. Each room has a different number of doors, and players must navigate through the network to find Room 5 (which contains the **Pistol**), then find their way to Room 4 to exit to Stage 3.

**Key Mechanic:** Spatial disorientation + memorization + deduction

---

## 🗺️ Room Network Structure

```
Room 1 (2 doors)  →  Room 2 | Room 1 (loop)
Room 2 (3 doors)  →  Room 3 | Room 2 (loop) | Room 1
Room 3 (4 doors)  →  Room 4 | Room 3 (loop) | Room 2 | Room 1
Room 4 (5 doors)  →  EXIT to Stage 3 | Room 4 (loop) | Room 5 | Room 3 | Room 2
Room 5 (5 doors)  →  Room 1 | Room 2 | Room 3 | Room 4 | Room 5 (loop) + PISTOL
```

**Key Insight:** Players must count doors to identify rooms!

---

## 🛠️ Scripts Required
- ✅ `RoomIdentifier.cs` - Identifies which room the player is in
- ✅ `RoomNumberUI.cs` - Displays current room number on screen
- ✅ `DoorMazeConnector.cs` - Connects doors to rooms (optional helper)
- ✅ `MazeRoomBuilder.cs` - Auto-generates the entire 5-room maze
- ✅ `DoorTrigger.cs` - (Already exists) Handles teleportation
- ✅ `SpawnPoint.cs` - (Already exists) Spawn points in each room

---

## 🏗️ Step-by-Step Setup

### Method A: Automatic (Recommended)

#### 1. Use MazeRoomBuilder

1. Create empty GameObject: `MazeRoomBuilder_Stage2`
2. Add component: `MazeRoomBuilder`
3. Configure in Inspector:
   - Room Size: `(10, 6, 10)`
   - Room Spacing: `20` (rooms won't be visible from each other)
   - Room Colors: Leave default (helps distinguish rooms visually)
   - ✅ Place Pistol In Room 5: `true`
   - Maze Parent Name: `Stage2_DoorMaze`

4. Right-click script → **Build Maze**
5. Delete MazeRoomBuilder GameObject (maze is created)

**Done!** The complete 5-room maze with all doors is generated automatically.

---

### Method B: Manual Setup (For Custom Layouts)

#### 1. Create Room Template

1. Use `StageBuilder` to create one room:
   - Room Size: `(10, 6, 10)`
   - Name: `Room_Template`

2. Add RoomIdentifier:
   - Room Number: `1`
   - Room Name: `Room 1`
   - Detection Radius: `5`

3. Add SpawnPoint in center:
   - Position: `(0, 1, 0)`
   - Spawn ID: `Room1_Spawn`

#### 2. Duplicate Rooms

1. Duplicate Room_Template 5 times
2. Name them: `Room_1`, `Room_2`, `Room_3`, `Room_4`, `Room_5`
3. Position them far apart: `(0,0,0)`, `(20,0,0)`, `(40,0,0)`, etc.
4. Update each room's RoomIdentifier number (1-5)

#### 3. Create Doors

For each room, create door cubes on the north wall:

**Room 1 (2 doors):**
- Door 1: Position `(-3, 1.5, 4.5)` → Room 2
- Door 2: Position `(3, 1.5, 4.5)` → Room 1

**Room 2 (3 doors):**
- Door 1: `(-4.5, 1.5, 4.5)` → Room 3
- Door 2: `(0, 1.5, 4.5)` → Room 2
- Door 3: `(4.5, 1.5, 4.5)` → Room 1

**Room 3 (4 doors):**
- Door 1: `(-4.5, 1.5, 4.5)` → Room 4
- Door 2: `(-1.5, 1.5, 4.5)` → Room 3
- Door 3: `(1.5, 1.5, 4.5)` → Room 2
- Door 4: `(4.5, 1.5, 4.5)` → Room 1

**Room 4 (5 doors):**
- Door 1: `(-6, 1.5, 4.5)` → Stage 3 (scene transition)
- Door 2: `(-3, 1.5, 4.5)` → Room 4
- Door 3: `(0, 1.5, 4.5)` → Room 5
- Door 4: `(3, 1.5, 4.5)` → Room 3
- Door 5: `(6, 1.5, 4.5)` → Room 2

**Room 5 (5 doors):**
- Door 1: `(-6, 1.5, 4.5)` → Room 1
- Door 2: `(-3, 1.5, 4.5)` → Room 2
- Door 3: `(0, 1.5, 4.5)` → Room 3
- Door 4: `(3, 1.5, 4.5)` → Room 4
- Door 5: `(6, 1.5, 4.5)` → Room 5

#### 4. Configure Each Door

For each door cube:
1. Scale: `(2, 3, 0.5)`
2. Add `BoxCollider` → ✅ Is Trigger
3. Add `DoorTrigger` component:
   - Door Type: `Teleport`
   - Teleport Destination: Assign target room's SpawnPoint
4. Color: Blue (material color)

---

### 2. Add Room Number UI

#### Option A: Auto-Create
1. Create empty GameObject: `RoomNumberUI_Manager`
2. Add component: `RoomNumberUI`
3. Configure:
   - ✅ Auto Create UI: `true`
   - UI Position: `(10, -10)` (top-left)
   - Room Prefix: `"Room "`
   - ✅ Animate On Change: `true`

#### Option B: Manual UI
1. In Canvas, create Panel: `RoomNumberPanel`
2. Add TextMeshPro child: `RoomNumberText`
3. Position in top-left corner
4. Add `RoomNumberUI` component to a GameObject
5. Assign references in Inspector

---

### 3. Place Pistol in Room 5

1. Go to `Room_5`
2. Create Sphere: `Pickup_Pistol`
3. Position: `(0, 1, 0)` (center of room)
4. Scale: `(0.5, 0.5, 0.5)`
5. Add `SphereCollider` → ✅ Is Trigger
6. Add `ItemPickup` component
7. Assign `ItemData`: Drag `Data_Pistol` from `Assets/_Data/`
8. Set Layer: `Interactable`
9. Color: Red (for visibility)

---

### 4. Create Exit Door in Room 4

1. In Room 4, find Door 1 (leftmost door)
2. Modify its DoorTrigger:
   - Door Type: `Scene Transition`
   - Target Scene Index: `3` (Stage 3)
   - ❌ Teleport Destination: Leave empty
3. Color this door **Green** to indicate exit

---

### 5. Optional: Visual Differentiation

To help playtesters (but make it subtle):

**Subtle Room Colors:**
- Room 1: Slight red tint on floor
- Room 2: Slight green tint
- Room 3: Slight blue tint
- Room 4: Slight yellow tint
- Room 5: Slight purple tint

**Or keep all identical** for maximum challenge!

---

## 🎮 Testing Checklist

- [ ] Room 1 has 2 doors (to Room 2, to Room 1)
- [ ] Room 2 has 3 doors (to Room 3, Room 2, Room 1)
- [ ] Room 3 has 4 doors (to Room 4, Room 3, Room 2, Room 1)
- [ ] Room 4 has 5 doors (to Stage 3, Room 4, Room 5, Room 3, Room 2)
- [ ] Room 5 has 5 doors (to all rooms)
- [ ] Pistol is in Room 5 and can be picked up
- [ ] Room number UI displays correctly when entering each room
- [ ] Room number UI animates when changing rooms
- [ ] Teleportation works instantly (no loading screen)
- [ ] Exit door in Room 4 loads Stage 3
- [ ] Player spawns in center of each room after teleport

---

## 📦 Final Hierarchy

```
Stage2_DoorMaze
├── Room_1
│   ├── Floor, Walls, Ceiling
│   ├── SpawnPoint (center)
│   ├── Door_To_Room2
│   │   └── DoorTrigger (Teleport to Room 2)
│   └── Door_To_Room1
│       └── DoorTrigger (Teleport to Room 1)
│
├── Room_2
│   ├── Floor, Walls, Ceiling
│   ├── SpawnPoint
│   ├── Door_To_Room3
│   ├── Door_To_Room2
│   └── Door_To_Room1
│
├── Room_3
│   ├── Floor, Walls, Ceiling
│   ├── SpawnPoint
│   ├── Door_To_Room4
│   ├── Door_To_Room3
│   ├── Door_To_Room2
│   └── Door_To_Room1
│
├── Room_4
│   ├── Floor, Walls, Ceiling
│   ├── SpawnPoint
│   ├── Door_To_Stage3 (Scene Transition - GREEN)
│   ├── Door_To_Room4
│   ├── Door_To_Room5
│   ├── Door_To_Room3
│   └── Door_To_Room2
│
└── Room_5
    ├── Floor, Walls, Ceiling
    ├── SpawnPoint
    ├── Pickup_Pistol (ItemPickup component)
    ├── Door_To_Room1
    ├── Door_To_Room2
    ├── Door_To_Room3
    ├── Door_To_Room4
    └── Door_To_Room5

UI (in Canvas)
└── RoomNumberPanel
    └── RoomNumberText (TextMeshPro)
```

---

## 🎯 Difficulty Tuning

**Too Easy?**
- Make all rooms identical (no color tints)
- Remove room number UI (force player to count doors)
- Randomize door destinations on each entry

**Too Hard?**
- Add room number signs inside each room
- Color-code doors by destination
- Add a map showing room connections

**Puzzle Hints:**
- "Count the doors to know which room you're in"
- "Room 5 has the pistol, but how do you get there?"
- "The exit is in Room 4"

---

## 🧩 Solution Path Example

**Goal:** Get Pistol from Room 5 → Exit via Room 4

**Optimal Path:**
1. Start in Room 1 (2 doors)
2. Go to Room 2 (3 doors)
3. Go to Room 3 (4 doors)
4. Go to Room 5 (5 doors) - **Get Pistol**
5. Go back to Room 4 (5 doors)
6. Take EXIT door (green) to Stage 3

**Alternative:** Players can wander randomly, but counting doors reveals the pattern.

---

## 💡 Advanced Features (Optional)

### Dynamic Door Labels
Add TextMeshPro above each door showing destination:
- "To Room 3"
- "Exit"

### Door Sound Effects
- Different pitch for each room (audio cue)
- Footstep echo changes per room

### Mini-Map
- 2D map showing room connections
- Hidden by default, found as a pickup

---

## 🚀 Next Steps

Once Stage 2 is complete:
1. Test full navigation from Room 1 → 5 → 4 → Exit
2. Verify pistol pickup works
3. Verify room UI updates correctly
4. Move to **Stage 3: Shooting Gallery**

---

✅ **Stage 2 is now ready!**
