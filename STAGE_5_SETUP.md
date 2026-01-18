# ⛏️ Stage 5: Destructible Maze - Setup Guide

## 📋 Overview
Stage 5 is a **5 wide × 20 long** maze filled with destructible walls. Only walls on the correct path can be destroyed with the **Pioche**. A **Map** on the wall shows the correct path (green = destructible, black = solid).

**Items to collect:**
- ✅ **Sac** (Backpack) +3 inventory slots
- ✅ **Pioche** (Pickaxe) - Required to break walls
- ✅ **Lampe** (Flashlight) - Hidden in a destructible block
- ✅ **Map** - Wall-mounted texture showing the path

---

## 🛠️ Scripts Required
- ✅ `MazeBuilder.cs` - Generates 5×20 maze with correct path
- ✅ `MapDisplay.cs` - Shows maze map on wall
- ✅ `DestructibleWall.cs` - (Already exists from Phase 2)
- ✅ `Pickaxe.cs` - (Already exists from Phase 2)
- ✅ `ItemPickup.cs` - (Already exists from Phase 2)

---

## 🏗️ Step-by-Step Setup

### 1. Create the Room Structure

#### Using StageBuilder
1. Create empty GameObject: `StageBuilder_Stage5`
2. Add component: `StageBuilder`
3. Configure:
   - Room Size: `(15, 6, 45)` (narrow and long for maze)
   - Room Name: `Stage5_MazeRoom`
   - Floor Color: Dark Stone `(0.25, 0.25, 0.25)`
   - Wall Color: Brown Stone `(0.4, 0.3, 0.2)`
4. Right-click → **Build Room**
5. Delete StageBuilder GameObject

---

### 2. Generate the Maze

1. Create empty GameObject: `MazeBuilder_Stage5`
2. Position: `(0, 0, 0)` (center of room)
3. Add component: `MazeBuilder`
4. Configure in Inspector:

   **Maze Dimensions:**
   - Width: `5`
   - Length: `20`
   - Block Size: `2.0`
   - Block Height: `3.0`

   **Path Definition:**
   - Correct Path: Use the default serpentining path OR:
   - Right-click script → **Generate Random Path**

   **Colors:**
   - Destructible Color: Brown `(0.6, 0.4, 0.2)`
   - Indestructible Color: Dark Gray `(0.3, 0.3, 0.3)`

   **Item Placements:**
   - Backpack Position: `(0, 5)` (early in maze)
   - Pickaxe Position: `(2, 10)` (middle of maze)
   - Flashlight Position: `(4, 15)` (hidden, late in maze)

   **Naming:**
   - Maze Name: `Stage5_Maze`

5. Right-click script → **Build Maze**

The maze is now generated! Destructible walls will have the `DestructibleWall` component automatically attached.

---

### 3. Manually Assign ItemData to Pickups

The MazeBuilder creates item pickups, but you need to assign the ItemData assets:

1. Select `Stage5_Maze → Pickup_Sac`
2. In Inspector → `ItemPickup` component
3. Assign `ItemData`: Drag `Data_Sac` from `Assets/_Data/`

Repeat for:
- `Pickup_Pioche` → `Data_Pioche`
- `Pickup_Lampe` → `Data_Lampe`

---

### 4. Create the Map Display

1. Create empty GameObject: `Map_WallDisplay`
2. Position: `(-6, 3, 10)` (on west wall, middle of maze)
3. Rotation: `(0, 90, 0)` (facing east so players can see it)
4. Add component: `MapDisplay`
5. Configure:

   **Map Settings:**
   - Maze Width: `5`
   - Maze Length: `20`
   - Correct Path: **Copy the same array as MazeBuilder**

   **Visual:**
   - Texture Width: `100` pixels
   - Texture Height: `400` pixels
   - Path Color: Green `(0, 1, 0)`
   - Wall Color: Black `(0, 0, 0)`
   - Background Color: Parchment `(0.8, 0.8, 0.7)`

   **Display:**
   - ✅ Auto Create Map: `true`
   - Map Scale: `(2, 4, 0.1)` (tall rectangular map)

6. **Right-click script → Generate Map Texture** (if not auto-created)

The map will appear on the wall showing green (destructible) and black (solid) blocks.

---

### 5. Add Entrance & Exit

#### Entrance (From Stage 4)
1. Create empty GameObject: `Entrance_FromStage4`
2. Position: `(2, 1, -2)` (just before maze start)
3. This is where the previous stage's door teleports/loads the player

#### Exit (To Stage 6)
1. Create empty GameObject: `Door_ToStage6`
2. Position: `(2, 1.5, 42)` (end of maze)
3. Add `BoxCollider` (trigger)
   - Size: `(3, 4, 1)`
4. Add `DoorTrigger`
5. Configure:
   - Door Type: `Scene Transition` or `Teleport`
   - Target Scene: Stage 6 (Darkness)
   - ❌ Require Item: `false`

---

### 6. Add Lighting

Since the maze is dark and enclosed:

1. **Add Point Lights** every few blocks:
   - Create `Point Light`
   - Position: Above maze blocks (Y = 4)
   - Intensity: `3`
   - Range: `8`
   - Color: Orange `(1, 0.6, 0)` (torch-like)

2. **Duplicate lights** along the maze (every 10 blocks or so)

3. **Optional: Add torch models** as children of lights

---

### 7. Hide the Flashlight (Optional Challenge)

To make the Lampe harder to find:

1. Find `Pickup_Lampe` in the maze
2. Move it **inside** a destructible block:
   - Find the destructible block at position `(4, 15)`
   - Place Lampe at the exact center of that block
3. Player must break the block to reveal the Lampe

**Visual clue:** Make that block a slightly different color (golden hint).

---

### 8. Add Tutorial Sign at Entrance

1. Create empty GameObject: `TutorialSign_MazeEntrance`
2. Position: `(0, 2, -3)` (before maze)
3. Add component: `TutorialSign`
4. Configure:
   - Tutorial Text:
     ```
     🗺️ LABYRINTHE DESTRUCTIBLE

     Trouvez la PIOCHE pour casser les murs
     Consultez la CARTE sur le mur
     Seuls les blocs du chemin correct sont destructibles !
     ```

---

### 9. Optional: Add Wall Torches & Atmosphere

**Particle Systems:**
- Attach torch flame particles to Point Lights
- Dust particles when breaking walls (add to DestructibleWall script)

**Audio:**
- Ambient echo/dripping water
- Pickaxe breaking sound (already in Pickaxe.cs)

---

## 🎮 Testing Checklist

- [ ] Maze generates correctly (5 wide, 20 long)
- [ ] Destructible blocks are brown, solid blocks are gray
- [ ] Backpack, Pioche, and Lampe are placed correctly
- [ ] Map displays on wall with green path and black walls
- [ ] Map accurately shows the correct path
- [ ] Player can pick up Pioche
- [ ] Right-click breaks destructible walls
- [ ] Right-click does NOT break solid walls
- [ ] Lampe is hidden and findable
- [ ] Inventory expands when picking up Sac
- [ ] Exit door leads to Stage 6
- [ ] Lighting is adequate (or flashlight helps)

---

## 📦 Final Hierarchy

```
Stage5_MazeRoom
├── Floor
├── Walls (North, South, East, West)
├── Ceiling
│
├── Entrance_FromStage4
│
├── Stage5_Maze (generated by MazeBuilder)
│   ├── Wall_0_0_Destructible (DestructibleWall)
│   ├── Wall_1_0_Solid
│   ├── Wall_2_0_Solid
│   ├── ... (100 blocks total)
│   ├── Pickup_Sac (at position 0,5)
│   ├── Pickup_Pioche (at position 2,10)
│   └── Pickup_Lampe (at position 4,15)
│
├── Map_WallDisplay (MapDisplay component)
│   └── MapDisplay (Quad with texture)
│
├── TutorialSign_MazeEntrance
│
├── Door_ToStage6
│
└── Lighting
    ├── TorchLight_1
    ├── TorchLight_2
    └── ... (every 10 blocks)
```

---

## 🎯 Difficulty Tuning

**Path Too Easy?**
- Generate a more complex random path
- Make path narrower (change correctPath to zigzag more)
- Reduce health of destructible walls (1 hit instead of 3)

**Path Too Hard?**
- Widen the path (allow 2 adjacent blocks to be destructible)
- Add more hints (glowing destructible blocks)
- Place a second map halfway through

**Flashlight Too Easy to Find?**
- Don't mark the block containing it on the map
- Place it deeper in the maze
- Require breaking multiple blocks to reach it

---

## 🚀 Next Steps

Once Stage 5 is complete:
1. Test full maze traversal with Pioche
2. Verify map is readable and helpful
3. Adjust lighting (should be dim, flashlight useful)
4. Move to **Stage 6: Darkness Navigation**

---

## 💡 Pro Tips

**Maze Customization:**
- Edit `correctPath` array in MazeBuilder to design your own path
- Right-click → **Show Path in Console** to debug
- Use **Gizmos** (green wireframe cubes) to preview path before building

**Map Texture Export:**
- The map texture is generated at runtime
- To save it as an asset, add code to save Texture2D to PNG

**Prefab Workflow:**
- Make destructible blocks a prefab variant
- Add particle effects, sounds to the prefab
- All destructible blocks will inherit changes

**Performance:**
- 100 blocks with colliders can be heavy
- Consider using a single MeshCollider for solid walls
- Destructible walls keep individual colliders

---

## 🧩 Path Design Examples

**Simple Straight Path:**
```csharp
correctPath = { 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2 };
```

**Serpent Path (Default):**
```csharp
correctPath = { 2,2,1,1,2,3,3,2,1,0,0,1,2,3,4,4,3,2,2,2 };
```

**Diagonal Zigzag:**
```csharp
correctPath = { 0,1,2,3,4,3,2,1,0,1,2,3,4,3,2,1,0,1,2,3 };
```

---

✅ **Stage 5 is now ready!**
