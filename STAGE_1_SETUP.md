# ⏱️ Stage 1: Falling Platforms - Setup Guide

## 📋 Overview
Stage 1 challenges players to jump across platforms that fall after **3 seconds**. Falling returns the player to Stage 0 (tutorial room). Platforms automatically reset when the player re-enters the room.

**Key Mechanic:** Time pressure + precise platforming

---

## 🛠️ Scripts Required
- ✅ `FallingPlatform.cs` - Platform with timer, fall physics, auto-reset
- ✅ `SpawnPoint.cs` - (Already exists from Stage 0)
- ✅ `KillZone.cs` - (Already exists from Stage 0)
- ✅ `DoorTrigger.cs` - (Already exists from Stage 0)

---

## 🏗️ Step-by-Step Setup

### 1. Create the Room Structure

#### Using StageBuilder (Quick Method)
1. Create empty GameObject: `StageBuilder_Stage1`
2. Add component: `StageBuilder`
3. Configure:
   - Room Size: `(30, 10, 30)` (taller for jumping)
   - Room Name: `Stage1_FallingPlatforms`
   - Floor Color: Dark Gray `(0.2, 0.2, 0.2)`
   - Wall Color: Stone `(0.4, 0.4, 0.4)`
4. Right-click script → **Build Room**
5. Delete StageBuilder GameObject

**OR** build manually with cubes.

---

### 2. Add Entrance (From Stage 0)

1. Create empty GameObject: `Entrance_FromStage0`
2. Position: `(0, 2, -12)` (south wall)
3. Add component: `BoxCollider`
   - Size: `(3, 4, 1)`
4. This is where player arrives from Stage 0 door

**Tip:** The DoorTrigger in Stage 0 will teleport or load scene here.

---

### 3. Create Falling Platforms Path

Create **8-10 platforms** that form a path across the room:

#### Platform Template:
1. Create Cube: `FallingPlatform_1`
2. Scale: `(3, 0.5, 3)` (flat platform)
3. Position: `(0, 1, -8)` (first platform after entrance)
4. Add component: `Rigidbody`
   - ❌ Use Gravity: `false` (script will enable it)
   - ✅ Is Kinematic: `true`
5. Add component: `FallingPlatform`
6. Configure:
   - Fall Delay: `3.0` seconds
   - Reset Delay: `5.0` seconds
   - ✅ Auto Reset: `true`
   - ✅ Shake Before Fall: `true`
   - Normal Color: Gray
   - Warning Color: Red
   - ✅ Show Timer: `true` (countdown text above platform)

#### Platform Layout Example:
```
Entrance (0, 2, -12)
   ↓
Platform 1 (0, 1, -8)   ← Safe starting platform (no FallingPlatform script)
   ↓
Platform 2 (0, 1, -4)   ← First falling platform
   ↓
Platform 3 (4, 1.5, 0)  ← Jump up and right
   ↓
Platform 4 (8, 2, 4)    ← Jump up and right
   ↓
Platform 5 (8, 2, 8)    ← Straight ahead
   ↓
Platform 6 (4, 1.5, 10) ← Jump down and left
   ↓
Platform 7 (0, 1, 10)   ← Left
   ↓
Platform 8 (-4, 1, 8)   ← Left and back
   ↓
Exit Door (-8, 2, 4)    ← Door to Stage 2
```

**Difficulty Curve:**
- First 2 platforms: Easy, flat, same height
- Middle platforms: Varied heights (require precise jumps)
- Last 2 platforms: Faster path or tighter spacing

---

### 4. Add Starting Safe Platform

1. Create Cube: `SafePlatform_Start`
2. Scale: `(4, 0.5, 4)` (larger, no fall)
3. Position: `(0, 1, -8)` (right after entrance)
4. **Do NOT add FallingPlatform script** (this one doesn't fall)
5. Material: Green tint to signal "safe"

---

### 5. Add Kill Zone (Respawn to Stage 0)

1. Create empty GameObject: `KillZone_Stage1`
2. Position: `(0, -20, 0)` (far below)
3. Add component: `BoxCollider`
   - ✅ Is Trigger: `true`
   - Size: `(100, 5, 100)` (cover entire fall area)
4. Add component: `KillZone`
5. Configure:
   - Custom Respawn Point: **Assign SpawnPoint_Stage0** (from Stage 0 scene)
   - ❌ Use Default Spawn: `false` (we want Stage 0 specifically)
   - Respawn Message: `💀 Vous êtes tombé ! Retour au début...`

**Important:** If Stage 0 and Stage 1 are in the **same scene**, assign the SpawnPoint directly. If they're in **different scenes**, you'll need scene transition logic.

---

### 6. Add Exit Door (To Stage 2)

1. Create empty GameObject: `Door_ToStage2`
2. Position: `(-8, 2, 4)` (end of platform path)
3. Add component: `BoxCollider`
   - ✅ Is Trigger: `true`
   - Size: `(3, 4, 1)`
4. Add component: `DoorTrigger`
5. Configure:
   - Door Type: `Scene Transition` or `Teleport`
   - Target Scene Index: `2` (or Stage 2 scene)
   - ❌ Require Item: `false`

**Optional: Visual door frame**
- Create cubes to form a door frame
- Material: Glowing blue to indicate exit

---

### 7. Platform Reset System (Optional Enhancement)

If you want platforms to reset when player re-enters the room:

#### Option A: Manual Reset Trigger
1. Create empty GameObject: `PlatformResetTrigger`
2. Position at entrance: `(0, 2, -12)`
3. Add `BoxCollider` (trigger)
4. Create script: `PlatformResetZone.cs`

```csharp
using UnityEngine;

public class PlatformResetZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FallingPlatform[] platforms = FindObjectsOfType<FallingPlatform>();
            foreach (FallingPlatform platform in platforms)
            {
                platform.ResetPlatform();
            }
            Debug.Log("🔄 Toutes les plateformes réinitialisées !");
        }
    }
}
```

5. Attach to `PlatformResetTrigger`

---

### 8. Visual Polish (Optional)

#### Add Lighting
- Point Light above each platform (flickering torches)
- Color: Orange `(1, 0.6, 0)`
- Intensity: `2`
- Range: `8`

#### Add Particle Effects
- Dust particles when platform falls
- Add `ParticleSystem` as child of each FallingPlatform
- Trigger on fall in script

#### Add Sound Effects
- Assign sounds in FallingPlatform:
  - Activate Sound: "Creak" or "Click"
  - Fall Sound: "Rumble" or "Break"

---

## 🎮 Testing Checklist

- [ ] Player enters from Stage 0 door
- [ ] First platform doesn't fall (safe)
- [ ] Stepping on FallingPlatform shows countdown (3, 2, 1)
- [ ] Platform shakes before falling
- [ ] Platform color changes from gray → red
- [ ] Platform falls after 3 seconds
- [ ] Platform resets after 5 seconds (if auto-reset enabled)
- [ ] Falling off respawns player to Stage 0
- [ ] All platforms can be crossed with skillful timing
- [ ] Exit door leads to Stage 2 (or next area)
- [ ] No console errors

---

## 📦 Final Hierarchy

```
Stage1_FallingPlatforms
├── Floor
├── Walls (North, South, East, West)
├── Ceiling
│
├── Entrance_FromStage0 (teleport destination)
├── SafePlatform_Start (no falling)
│
├── FallingPlatform_1
│   └── TimerText (child - created automatically)
├── FallingPlatform_2
├── FallingPlatform_3
├── FallingPlatform_4
├── FallingPlatform_5
├── FallingPlatform_6
├── FallingPlatform_7
├── FallingPlatform_8
│
├── Door_ToStage2
│   └── DoorFrame (visual)
│
├── KillZone_Stage1 (respawn to Stage 0)
│
└── PlatformResetTrigger (optional)
```

---

## 🎯 Difficulty Tuning

**Too Easy?**
- Reduce `fallDelay` to 2 seconds
- Add more platforms (longer path)
- Make platforms smaller (`2x0.5x2`)
- Increase jump height requirement

**Too Hard?**
- Increase `fallDelay` to 4 seconds
- Add more safe platforms (no fall)
- Make platforms larger (`4x0.5x4`)
- Reduce gaps between platforms

---

## 🚀 Next Steps

Once Stage 1 is complete:
1. Test full flow: Stage 0 → Stage 1 → Stage 2
2. Balance platform spacing and fall timer
3. Add visual polish (lights, particles, sounds)
4. Move to **Stage 2: Door Maze (5 Rooms)**

---

## 💡 Advanced Tips

**Multiple Paths:**
- Create 2-3 platform paths (left, middle, right)
- Players can choose difficulty

**Moving Platforms:**
- Extend FallingPlatform with horizontal movement before falling

**Checkpoint System:**
- Add safe platforms halfway through
- Falling respawns to last safe platform instead of Stage 0

---

✅ **Stage 1 is now ready!**
