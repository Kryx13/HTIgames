# Stage 6: Darkness Zone - Setup Guide

## Overview
Stage 6 is a tense navigation challenge where players must traverse a narrow winding path in complete darkness. One wrong step means falling into the kill zone below. The flashlight (from Stage 5) is required to see the path ahead.

## Features
- **Complete Darkness**: Very dark ambient lighting and fog
- **Narrow Path**: Thin walkway (2m wide by default)
- **Winding Route**: Randomized turns and curves
- **Kill Zone**: Fall off = respawn
- **Flashlight Requirement**: Need flashlight from Stage 5 maze
- **Auto-Generation**: Builder creates entire zone automatically

---

## Quick Setup (Auto-Build)

### 1. Create Empty GameObject
1. In Unity, create empty GameObject: `GameObject > Create Empty`
2. Name it: `Stage_6_DarknessZoneBuilder`
3. Position: (0, 0, 0)

### 2. Add Builder Component
1. Select the GameObject
2. Add Component: `DarknessZoneBuilder`
3. Configure settings (or use defaults):
   - **Number of Segments**: 15 (path length)
   - **Segment Length**: 8m (each segment)
   - **Path Width**: 2m (narrower = harder)
   - **Randomize Path**: ✓ (creates turns and curves)
   - **Add Path Lights**: ✓ (dim lights along path)

### 3. Build the Zone
1. Right-click `DarknessZoneBuilder` component
2. Select: `Build Darkness Zone`
3. Wait for: "✅ Darkness Zone built successfully!"

### 4. Test
1. Enter Play Mode
2. Notice the darkness effect
3. Navigate the narrow path carefully
4. Fall off = respawn at spawn point
5. Reach end platform = exit door activates

---

## Manual Setup (Custom Zone)

### Components Needed
1. **DarknessZone** (manages lighting effect)
2. **Path Segments** (narrow walkway)
3. **Kill Zone** (beneath path)
4. **Exit Platform** (at end)
5. **Exit Door** (to Stage 7)

### Step 1: Create DarknessZone Manager
1. Create Empty: `GameObject > Create Empty`
2. Name: `DarknessZone_Manager`
3. Add Component: `DarknessZone`
4. Configure:
   - **Enable Darkness Effect**: ✓
   - **Ambient Color**: Very dark (0.05, 0.05, 0.08)
   - **Fog Density**: 0.08
   - **Require Flashlight**: ✓
   - **Add Player Light**: ✓ (small light around player)

### Step 2: Build Path Segments
1. Create Cube: `GameObject > 3D Object > Cube`
2. Name: `PathSegment_1`
3. Scale: (2, 0.5, 8) - width, height, length
4. Position: Start position (0, 0, 0)
5. Material: Dark gray color
6. Repeat for each segment, positioning end-to-end
7. Add turns by rotating segments

**Tips for Path Design:**
- Keep width at 2m or less for challenge
- Add slight turns (15-45 degrees)
- Place segments carefully to align
- Test player can navigate without falling

### Step 3: Create Kill Zone
1. Create Cube: `GameObject > 3D Object > Cube`
2. Name: `KillZone`
3. Scale: (100, 1, 100) - large enough to catch falls
4. Position: Y = -10 (below path)
5. Add Component: `KillZone`
6. BoxCollider: Set **Is Trigger**: ✓
7. Material: Nearly invisible (very dark, semi-transparent)

### Step 4: Create Exit Platform
1. Create Cube at end of path
2. Name: `ExitPlatform`
3. Scale: (6, 0.5, 6) - landing area
4. Material: Cyan or glowing color
5. Add Component: `ExitTrigger`
6. BoxCollider: Set **Is Trigger**: ✓

### Step 5: Create Exit Door
1. Create Cube or use Door Prefab
2. Name: `ExitDoor_ToStage7`
3. Position: Beyond exit platform
4. Add Component: `DoorTrigger`
5. Configure:
   - **Door Type**: Scene Transition
   - **Target Scene Name**: "Stage_7_Final"
6. BoxCollider: Set **Is Trigger**: ✓
7. **Disable GameObject** (activates when player reaches end)

### Step 6: Add Lights (Optional)
1. Create Point Light: `GameObject > Light > Point Light`
2. Position: Above path segment
3. Configure:
   - **Range**: 8
   - **Intensity**: 0.8 (dim)
   - **Color**: Cool blue (0.5, 0.7, 1)
   - **Shadows**: None (performance)
4. Duplicate along path every 2-3 segments

---

## How It Works

### Darkness Effect
When DarknessZone activates:
1. Sets ambient light to very dark (0.05, 0.05, 0.08)
2. Enables exponential squared fog (density 0.08)
3. Creates a small point light on player (range 5m)
4. Checks if player has flashlight item

**Visual Result**: Almost pitch black, can only see immediate surroundings

### Player Navigation
1. Player spawns at beginning of path
2. Must use flashlight to see path ahead
3. Can only see ~5-10m ahead in darkness
4. Carefully walk along narrow path
5. Fall off path = trigger kill zone = respawn

### Exit Activation
1. Player reaches exit platform at end
2. `ExitTrigger` detects player
3. Calls `DarknessZone.ActivateExit()`
4. Exit door appears
5. Player enters door to Stage 7

---

## Flashlight Requirement

### Why Required?
- Stage 6 is extremely dark
- Default player light only illuminates 5m
- Flashlight extends vision to 15-20m
- Makes navigation much easier

### How to Check
DarknessZone automatically checks:
```csharp
bool hasFlashlight = playerInventory.HasItem("Flashlight");
```

If player doesn't have flashlight:
- ⚠️ Warning appears in console
- Stage is still playable but much harder
- Reminds player to get flashlight from Stage 5

### Flashlight Location
Player should have picked up flashlight in **Stage 5: Maze** (placed by MazeBuilder).

---

## Customization

### Difficulty Levels

**Easy Mode:**
- Path Width: 3m
- Segment Length: 10m
- Number of Segments: 10
- Randomize Path: ☐ (straight)
- Add Path Lights: ✓ (many)
- Light Intensity: 1.2

**Medium Mode (Default):**
- Path Width: 2m
- Segment Length: 8m
- Number of Segments: 15
- Randomize Path: ✓
- Add Path Lights: ✓ (some)
- Light Intensity: 0.8

**Hard Mode:**
- Path Width: 1.5m
- Segment Length: 6m
- Number of Segments: 20
- Randomize Path: ✓
- Add Path Lights: ☐ (none)
- Light Intensity: 0.5

**Extreme Mode:**
- Path Width: 1m
- No path lights
- Very short segments (4m)
- Many curves
- Player light range reduced to 3m

### Darkness Intensity

Modify in DarknessZone component:

**Lighter (more forgiving):**
```
Ambient Color: (0.1, 0.1, 0.15)
Fog Density: 0.05
Player Light Range: 8m
```

**Darker (more challenging):**
```
Ambient Color: (0.02, 0.02, 0.04)
Fog Density: 0.12
Player Light Range: 3m
```

### Path Variations

**Straight Path (Easy):**
- Set `Randomize Path`: ☐
- All segments straight
- No turns or curves

**Winding Path (Medium):**
- Set `Randomize Path`: ✓
- 60% straight, 30% curved, 10% obstacles

**Maze-like Path (Hard):**
- Manually place segments
- Create 90-degree turns
- Add switchbacks
- Create narrow passages

### Adding Atmosphere

**Flickering Lights:**
Create script to make path lights flicker randomly:
```csharp
float flicker = Mathf.PerlinNoise(Time.time * 5f, 0f);
light.intensity = baseIntensity * (0.7f + flicker * 0.3f);
```

**Wind Sound:**
Add ambient AudioSource with wind/breeze sound.

**Particle Effects:**
Add dust or mist particles drifting across path.

---

## Testing Commands

### DarknessZone Context Menu
Right-click `DarknessZone` component:
- **Apply Darkness**: Manually applies darkness effect
- **Restore Normal Lighting**: Restores original lighting (for testing)

### Debug Logs
Enable `Show Debug Logs` to see:
- `🌑 Darkness Zone initialized`
- `⚠️ Player doesn't have Flashlight!`
- `✅ Player has Flashlight`
- `💡 Player light created`
- `✅ Reached end of Darkness Zone!`
- `✅ Darkness Zone complete! Exit activated.`

---

## Integration with Game Flow

### Scene Transition
**From Stage 5 (Maze):**
Player exits maze → Enters Stage 6 darkness zone

**To Stage 7 (Final Room):**
Player reaches end of path → Exit door appears → Enters final room

### Prerequisites
- **Required Item**: Flashlight (from Stage 5)
- **Optional**: None

### Next Stage
After Stage 6, player proceeds to **Stage 7: Final Room** (Amulet door slot).

---

## Troubleshooting

### Issue: Scene isn't dark
**Solution:**
1. Check `DarknessZone` component exists
2. Enable `Enable Darkness Effect`
3. Enter Play Mode (darkness applies at runtime)
4. Try context menu: "Apply Darkness"

### Issue: Player keeps falling through path
**Solution:**
1. Ensure path segments have BoxColliders
2. Check colliders are NOT triggers
3. Verify path segments connect properly (no gaps)
4. Test in Play Mode, not Edit Mode

### Issue: Kill zone doesn't respawn player
**Solution:**
1. Check `KillZone` component exists
2. Verify BoxCollider has `Is Trigger`: ✓
3. Check player has tag "Player"
4. Ensure `SpawnPoint` exists in scene

### Issue: Exit door doesn't appear
**Solution:**
1. Check exit platform has `ExitTrigger` component
2. Verify DarknessZone reference is set
3. Enable debug logs to see activation message
4. Make sure exit door is initially disabled

### Issue: Can't see anything at all
**Solution:**
1. Enable `Add Player Light` in DarknessZone
2. Increase `Player Light Range` (try 8m)
3. Add some path lights for guidance
4. Check player has flashlight from Stage 5

### Issue: Path lights are too bright
**Solution:**
1. Reduce `Light Intensity` (try 0.5)
2. Reduce `Light Range` (try 6m)
3. Change color to dimmer blue/purple
4. Set `Shadows`: None for better performance

---

## Architecture

### Scripts
- **DarknessZone.cs**: Manages darkness effect and flashlight check
- **PathSegment.cs**: Individual path piece (unused if using builder)
- **DarknessZoneBuilder.cs**: Auto-generates entire zone
- **ExitTrigger.cs**: Activates exit when player reaches end

### Lighting System
- Stores original RenderSettings
- Applies very dark ambient + fog
- Creates player point light
- Restores original settings when destroyed

### Dependencies
- **KillZone.cs**: For respawn when falling
- **SpawnPoint.cs**: For spawn location
- **DoorTrigger.cs**: For exit door
- **Inventory.cs**: For flashlight check

---

## Scene Hierarchy Example

```
Stage_6_DarknessZone
├── NarrowPath
│   ├── PathSegment_0
│   │   └── PathLight
│   ├── PathSegment_1
│   ├── PathSegment_2
│   │   └── PathLight
│   ├── ... (15 segments total)
│   └── PathSegment_14
├── KillZone
├── ExitPlatform
├── ExitDoor_ToStage7 (disabled)
└── SpawnPoint_Stage6

DarknessZone_Manager
└── (DarknessZone component)
```

---

## Performance Notes
- Disable shadows on path lights (performance)
- Use exponential squared fog (more efficient)
- Limit number of lights (2-5 total)
- Simple path geometry
- Suitable for all platforms

---

## Atmosphere Tips

**For Maximum Tension:**
1. Make path width 1.5m or less
2. Use only player light (no path lights)
3. Add ambient wind/creaking sounds
4. Randomize path heavily
5. Make it long (20+ segments)

**For Moderate Challenge:**
1. Use default settings (2m width)
2. Add occasional path lights
3. Mix straight and curved segments
4. Length of 12-15 segments

---

## Next Steps
1. Build Stage 6 using builder or manual setup
2. Test navigation difficulty
3. Adjust path width and lighting for desired challenge
4. Ensure flashlight is available in Stage 5
5. Proceed to **Stage 7: Final Room**

---

**Good luck navigating the darkness!** 🌑🔦
