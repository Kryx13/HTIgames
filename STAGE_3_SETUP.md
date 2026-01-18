# 🎯 Stage 3: Shooting Gallery - Setup Guide

## 📋 Overview
Stage 3 is a shooting challenge with **5 sequences** of targets. Each sequence has targets with increasing difficulty (static → moving → fast → unpredictable). When all targets in a sequence are destroyed, a platform appears allowing progression to the next sequence. Complete all 5 to unlock the exit door to Stage 4.

**Key Mechanic:** Precision shooting + target practice + progressive difficulty

---

## 🛠️ Scripts Required
- ✅ `TargetSequence.cs` - Manages a sequence of targets, activates platform when complete
- ✅ `ShootingGalleryManager.cs` - Tracks all 5 sequences, opens exit door
- ✅ `Target.cs` - (Already exists from Phase 2) Basic shootable targets
- ✅ `MovingTarget.cs` - (Already exists from Phase 2) Moving targets
- ✅ `Gun.cs` - (Already exists from Phase 2) Player shooting system

---

## 🎯 Difficulty Progression

### Sequence 1: Static Targets ⭐
- 3 static targets at close range
- Easy warm-up

### Sequence 2: Slow Horizontal Movement ⭐⭐
- 3 targets moving horizontally (slow speed)
- Predictable pattern

### Sequence 3: Fast Horizontal Movement ⭐⭐⭐
- 4 targets moving horizontally (fast speed)
- Requires leading the shot

### Sequence 4: Vertical OR Horizontal (Alternating) ⭐⭐⭐⭐
- 4 targets, some move vertically, some horizontally
- Mixed patterns

### Sequence 5: Random/Unpredictable Movement ⭐⭐⭐⭐⭐
- 5 targets with random movement
- Hardest challenge

---

## 🏗️ Step-by-Step Setup

### 1. Create the Room Structure

#### Using StageBuilder
1. Create empty GameObject: `StageBuilder_Stage3`
2. Add component: `StageBuilder`
3. Configure:
   - Room Size: `(30, 10, 40)` (large, tall room for shooting)
   - Room Name: `Stage3_ShootingGallery`
   - Floor Color: Dark gray
   - Wall Color: Stone
4. Right-click → **Build Room**
5. Delete StageBuilder GameObject

---

### 2. Create Shooting Gallery Manager

1. Create empty GameObject: `ShootingGalleryManager`
2. Position: `(0, 5, 0)` (center of room)
3. Add component: `ShootingGalleryManager`
4. Configure:
   - Sequences: Leave empty (will auto-find)
   - Exit Door: Will assign later
   - ✅ Deactivate Exit On Start: `true`
   - ✅ Show Debug Logs: `true`

---

### 3. Build Sequence 1: Static Targets

#### Create Sequence Parent
1. Create empty GameObject: `Sequence_1_Static`
2. Position: `(0, 3, -15)` (north end of room)
3. Add component: `TargetSequence`
4. Configure:
   - Sequence Number: `1`
   - Sequence Name: `"Sequence 1: Static"`
   - Platform To Activate: Will assign later
   - ✅ Deactivate On Start: `true`

#### Create 3 Static Targets
1. Create 3 Spheres as children of `Sequence_1_Static`:
   - `Target_1_1`: Position `(-3, 0, 0)`
   - `Target_1_2`: Position `(0, 0, 0)`
   - `Target_1_3`: Position `(3, 0, 0)`

2. For each target:
   - Scale: `(1, 1, 1)`
   - Add component: `Target`
   - Configure:
     - Max Health: `1` (one-shot kill)
     - Destroy On Death: ✅ `true`
   - Color: Red

#### Create Platform 1
1. Create Cube: `Platform_1`
2. Position: `(0, 1, -10)` (between sequence 1 and 2)
3. Scale: `(4, 0.5, 4)`
4. Color: Green
5. **Initially deactivate** in hierarchy

6. Assign to `Sequence_1_Static`:
   - Platform To Activate: Drag `Platform_1`

---

### 4. Build Sequence 2: Slow Horizontal

#### Create Sequence Parent
1. Create empty GameObject: `Sequence_2_SlowHorizontal`
2. Position: `(0, 3, -5)` (after platform 1)
3. Add component: `TargetSequence`
4. Configure:
   - Sequence Number: `2`
   - Sequence Name: `"Sequence 2: Slow Horizontal"`

#### Create 3 Moving Targets
1. Create 3 Spheres as children:
   - `MovingTarget_2_1`: Position `(-4, 0, 0)`
   - `MovingTarget_2_2`: Position `(0, 0, 0)`
   - `MovingTarget_2_3`: Position `(4, 0, 0)`

2. For each target:
   - Add component: `MovingTarget`
   - Add component: `Target` (MovingTarget needs Target too)
   - Configure MovingTarget:
     - Movement Type: `Horizontal`
     - Move Speed: `2` (slow)
     - Move Range: `3`
   - Configure Target:
     - Max Health: `1`
     - Destroy On Death: ✅
   - Color: Orange

#### Create Platform 2
1. Create Cube: `Platform_2`
2. Position: `(0, 1, 0)`
3. Scale: `(4, 0.5, 4)`
4. Deactivate
5. Assign to Sequence 2

---

### 5. Build Sequence 3: Fast Horizontal

Repeat same process as Sequence 2, but:
- Position: `(0, 3, 5)`
- 4 targets (increase difficulty)
- Move Speed: `5` (fast)
- Color: Yellow

---

### 6. Build Sequence 4: Vertical/Horizontal Mix

- Position: `(0, 3, 10)`
- 4 targets:
  - 2 with `Movement Type: Horizontal`
  - 2 with `Movement Type: Vertical`
- Move Speed: `3-4`
- Color: Cyan

---

### 7. Build Sequence 5: Random Movement

- Position: `(0, 3, 15)`
- 5 targets
- Movement Type: `Random`
- Move Speed: `4`
- Color: Magenta

---

### 8. Create Exit Door

1. Create Cube: `ExitDoor_ToStage4`
2. Position: `(0, 2, 20)` (end of gallery)
3. Scale: `(3, 4, 0.5)`
4. Add `BoxCollider` → ✅ Is Trigger
5. Add `DoorTrigger`:
   - Door Type: `Scene Transition`
   - Target Scene Index: `4` (Stage 4)
6. Color: Green (initially deactivated)
7. **Deactivate** in hierarchy

8. Assign to ShootingGalleryManager:
   - Exit Door: Drag `ExitDoor_ToStage4`

---

### 9. Assign Sequences to Manager

1. Select `ShootingGalleryManager`
2. In Inspector → `Sequences` array
3. Set Size: `5`
4. Drag each sequence object:
   - Element 0: `Sequence_1_Static`
   - Element 1: `Sequence_2_SlowHorizontal`
   - Element 2: `Sequence_3_FastHorizontal`
   - Element 3: `Sequence_4_VerticalHorizontal`
   - Element 4: `Sequence_5_Random`

---

### 10. Add Player Start Position

1. Create empty GameObject: `SpawnPoint_Stage3`
2. Position: `(0, 1, -20)` (south end, facing targets)
3. Rotation: `(0, 0, 0)` (facing north)
4. Add component: `SpawnPoint`
5. Configure:
   - ✅ Is Default Spawn: `true`
   - Spawn ID: `Stage3`

---

### 11. Optional: Add Walls Between Sequences

To separate each sequence visually:
1. Create wall cubes between platforms
2. Prevents skipping ahead
3. Forces player to complete sequences in order

---

### 12. Optional: Add Tutorial Sign

1. Create `TutorialSign_ShootingGallery`
2. Position: `(0, 3, -18)` (at entrance)
3. Add component: `TutorialSign`
4. Text:
```
🎯 SHOOTING GALLERY

Destroy all targets in each sequence
Platforms appear when sequence is complete
5 sequences - increasing difficulty

Left Click to SHOOT
```

---

## 🎮 Testing Checklist

- [ ] Player spawns at south end facing targets
- [ ] Sequence 1: All 3 static targets can be shot
- [ ] Platform 1 appears after destroying all targets in Sequence 1
- [ ] Sequence 2: 3 slow-moving targets work correctly
- [ ] Platform 2 appears after Sequence 2 complete
- [ ] Sequence 3: 4 fast-moving targets are challenging but fair
- [ ] Sequence 4: Mixed vertical/horizontal targets
- [ ] Sequence 5: 5 random-moving targets (hardest)
- [ ] Exit door appears and turns green after all 5 sequences complete
- [ ] Exit door loads Stage 4
- [ ] Gun has enough range to hit all targets
- [ ] Targets have proper hit detection
- [ ] No console errors

---

## 📦 Final Hierarchy

```
Stage3_ShootingGallery
├── Floor
├── Walls (North, South, East, West)
├── Ceiling
│
├── SpawnPoint_Stage3
├── TutorialSign_ShootingGallery (optional)
│
├── ShootingGalleryManager
│
├── Sequence_1_Static (TargetSequence)
│   ├── Target_1_1 (Target component)
│   ├── Target_1_2
│   └── Target_1_3
│
├── Platform_1 (initially inactive)
│
├── Sequence_2_SlowHorizontal (TargetSequence)
│   ├── MovingTarget_2_1 (MovingTarget + Target)
│   ├── MovingTarget_2_2
│   └── MovingTarget_2_3
│
├── Platform_2 (initially inactive)
│
├── Sequence_3_FastHorizontal (TargetSequence)
│   ├── MovingTarget_3_1
│   ├── MovingTarget_3_2
│   ├── MovingTarget_3_3
│   └── MovingTarget_3_4
│
├── Platform_3 (initially inactive)
│
├── Sequence_4_VerticalHorizontal (TargetSequence)
│   ├── MovingTarget_4_1 (Horizontal)
│   ├── MovingTarget_4_2 (Horizontal)
│   ├── MovingTarget_4_3 (Vertical)
│   └── MovingTarget_4_4 (Vertical)
│
├── Platform_4 (initially inactive)
│
├── Sequence_5_Random (TargetSequence)
│   ├── MovingTarget_5_1 (Random)
│   ├── MovingTarget_5_2 (Random)
│   ├── MovingTarget_5_3 (Random)
│   ├── MovingTarget_5_4 (Random)
│   └── MovingTarget_5_5 (Random)
│
├── Platform_5 (initially inactive)
│
└── ExitDoor_ToStage4 (DoorTrigger - initially inactive)
```

---

## 🎯 Difficulty Tuning

**Too Easy?**
- Reduce target size (smaller hit box)
- Increase movement speed
- Add more targets per sequence
- Reduce gun fire rate or accuracy

**Too Hard?**
- Increase target size
- Slow down movement
- Reduce number of targets
- Add aim assist or larger hitboxes
- Give infinite ammo (if ammo system exists)

**Alternate Patterns:**
- Circular movement
- Figure-8 pattern
- Diagonal movement
- Sine wave patterns

---

## 💡 Advanced Features (Optional)

### Score System
- Points based on sequence completion time
- Bonus for headshots (if targets have headshot zones)
- Combo multipliers for rapid kills

### Ammo Management
- Limited ammo pickups between sequences
- Forces accuracy

### Target Variety
- Small targets (hard to hit)
- Large targets (easy)
- Moving in/out of cover
- Destructible cover

### Time Limits
- Complete each sequence within time limit
- Adds pressure

---

## 🚀 Next Steps

Once Stage 3 is complete:
1. Test all 5 sequences work correctly
2. Verify difficulty curve feels fair
3. Check MovingTarget patterns are smooth
4. Ensure exit door appears reliably
5. Move to **Stage 4: Riddle Room**

---

✅ **Stage 3 is now ready!**
