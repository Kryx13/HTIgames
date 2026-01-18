# Stage 7: Final Room - Setup Guide

## Overview
Stage 7 is the final room of the game. Players must place the **Ancient Amulet** (collected from Stage 0) into a door slot to open the final door and complete the game. This is the victory room!

## Features
- **Amulet Door Slot**: Interactive slot that requires Ancient Amulet
- **Final Door**: Opens upward when Amulet is placed
- **Victory Trigger**: Completes the game when player passes through
- **Decorative Elements**: Pedestal, torches, atmospheric lighting
- **Auto-Generation**: Builder creates entire room automatically

---

## Quick Setup (Auto-Build)

### 1. Create Empty GameObject
1. In Unity, create empty GameObject: `GameObject > Create Empty`
2. Name it: `Stage_7_FinalRoomBuilder`
3. Position: (0, 0, 0)

### 2. Add Builder Component
1. Select the GameObject
2. Add Component: `FinalRoomBuilder`
3. Configure settings (or use defaults):
   - **Room Size**: (15, 8, 15)
   - **Door Open Height**: 6m (how far door rises)
   - **Add Pedestal**: ✓
   - **Add Torches**: ✓
   - **Number of Torches**: 4

### 3. Build the Room
1. Right-click `FinalRoomBuilder` component
2. Select: `Build Final Room`
3. Wait for: "✅ Final Room built successfully!"

### 4. Test
1. **IMPORTANT**: Make sure player has picked up Ancient Amulet from Stage 0
2. Enter Play Mode
3. Walk to the Amulet slot (glowing pedestal)
4. Press **E** to place Amulet
5. Watch door open upward
6. Walk through = Game Complete!

---

## Manual Setup (Custom Room)

### Components Needed
1. **AmuletDoorSlot** (the interactive slot)
2. **Final Door** (that opens when Amulet placed)
3. **Spawn Point** (entry)
4. **Victory Zone** (optional, beyond door)
5. **Decorations** (pedestal, torches)

### Step 1: Create Room Floor
1. Create Cube: `GameObject > 3D Object > Cube`
2. Name: `Floor`
3. Scale: (15, 1, 15)
4. Position: (0, -0.5, 0)
5. Material: Stone or dark material

### Step 2: Create Final Door
1. Create Cube: `GameObject > 3D Object > Cube`
2. Name: `FinalDoor`
3. Scale: (4, 5, 0.5) - width, height, thickness
4. Position: (0, 2, 6) - centered, in air
5. Material: Gold or ornate material

**Important**: Don't add any collider or trigger yet - this door just moves upward.

### Step 3: Create Amulet Slot
1. Create Cube: `GameObject > 3D Object > Cube`
2. Name: `AmuletSlot`
3. Scale: (0.8, 0.8, 0.3)
4. Position: (0, 1.5, 4) - in front of door
5. Add Component: `AmuletDoorSlot`
6. Configure:
   - **Required Item Name**: "Ancient Amulet"
   - **Door**: Drag FinalDoor GameObject here
   - **Door Open Height**: 6
   - **Door Open Speed**: 2
   - **Interaction Range**: 3

### Step 4: Create Amulet Visual
1. Create Sphere: `GameObject > 3D Object > Sphere`
2. Name: `AmuletVisual`
3. Make it child of `AmuletSlot`
4. Local Position: (0, 0, -0.2)
5. Local Scale: (0.5, 0.5, 0.1)
6. Material: Gold/Yellow with emission
7. **Disable GameObject** (will show when Amulet placed)
8. In `AmuletDoorSlot`, drag this to **Amulet Visual** field

### Step 5: Create Pedestal (Optional)
1. Create Cylinder: `GameObject > 3D Object > Cylinder`
2. Name: `Pedestal`
3. Scale: (1.5, 0.8, 1.5)
4. Position: Below slot (0, 0.7, 4)
5. Material: Stone color

### Step 6: Add Torches (Optional)
For each torch:
1. Create Cylinder: `GameObject > 3D Object > Cylinder`
2. Name: `Torch_1` (etc.)
3. Scale: (0.2, 2, 0.2)
4. Position: Around room perimeter
5. Material: Dark brown/black
6. Add child Point Light:
   - Range: 8
   - Intensity: 1.5
   - Color: Warm orange (1, 0.7, 0.3)
   - Local Position: (0, 1.2, 0)

**Recommended torch positions** (for 4 torches):
- Front-Left: (-5, 2, -5)
- Front-Right: (5, 2, -5)
- Back-Left: (-5, 2, 5)
- Back-Right: (5, 2, 5)

### Step 7: Create Victory Zone (Optional)
1. Create Cube behind door
2. Name: `VictoryZone`
3. Scale: (5, 3, 3)
4. Position: (0, 1, 10) - beyond door
5. Add Component: `VictoryTrigger`
6. BoxCollider: Set **Is Trigger**: ✓
7. Material: Transparent yellow (alpha: 0.1)

### Step 8: Create Spawn Point
1. Create Empty: `GameObject > Create Empty`
2. Name: `SpawnPoint_Stage7`
3. Position: (0, 1, -5) - room entrance
4. Add Component: `SpawnPoint`

---

## How It Works

### Amulet Requirement Check
When player presses **E** near the slot:
1. `AmuletDoorSlot` checks player's Inventory
2. Looks for item named "Ancient Amulet"
3. If found:
   - Removes Amulet from inventory
   - Shows AmuletVisual on slot
   - Changes slot color to gold
   - Starts door opening animation
4. If not found:
   - Shows warning: "Missing required item"
   - Reminds player to find Amulet in Stage 0

### Door Opening Animation
1. Door moves upward smoothly
2. Rises by `doorOpenHeight` (default: 6m)
3. Speed controlled by `doorOpenSpeed` (default: 2m/s)
4. Plays door opening sound
5. When fully open:
   - Plays victory sound
   - Calls `GameManager.CompleteGame()`

### Game Completion
When `GameManager.CompleteGame()` is called:
1. Sets `IsGameEnded = true`
2. Stops player movement (via PlayerController check)
3. Records final time and collectibles
4. Shows victory UI (if implemented)
5. Optionally saves to leaderboard

### Optional Victory Zone
If player walks through the opened door into VictoryZone:
- Additional victory confirmation
- Backup completion trigger (in case GameManager wasn't called)

---

## Ancient Amulet Location

### Where to Find It
The Ancient Amulet must be placed in **Stage 0: Tutorial Room**.

**Recommended placement:**
1. Use `StageBuilder` to build Stage 0
2. Amulet is automatically placed on a pedestal
3. Player picks it up at the start
4. Must carry it through all stages
5. Finally uses it here in Stage 7

**Alternative**: Place manually using `ItemPickup` component with item name "Ancient Amulet".

---

## Testing

### Test Amulet Placement
1. Ensure player has Amulet in inventory
2. Walk to slot
3. Prompt should appear: "Press E to place Amulet"
4. Press E
5. Check door starts moving upward
6. Wait for door to fully open
7. Verify victory message appears

### Test Without Amulet
1. Remove Amulet from inventory (or don't pick it up)
2. Try to interact with slot
3. Should see warning: "Missing required item"
4. Door should NOT open

### Test Door Animation
1. Place Amulet
2. Watch door rise smoothly
3. Should take ~3 seconds (6m height ÷ 2m/s speed)
4. Door should stop at final position
5. Should play victory sound

### Debug Mode
Enable `Show Debug Logs` in AmuletDoorSlot to see:
- `🚪 Amulet Door Slot initialized`
- `⚠️ Missing required item: Ancient Amulet`
- `✅ AMULET PLACED! Opening final door...`
- `🚪 Door opening...`
- `🎉 DOOR FULLY OPEN! GAME WON!`

---

## Customization

### Door Style

**Faster Opening:**
```
Door Open Speed: 4
```

**Slower, More Dramatic:**
```
Door Open Speed: 1
```

**Higher Rise:**
```
Door Open Height: 10
```

### Slot Appearance

**Change Colors:**
```csharp
Empty Slot Color: (0.3, 0.3, 0.4) - Gray
Filled Slot Color: (1, 0.84, 0) - Gold
```

**Different Interaction Key:**
```csharp
Interact Key: KeyCode.F
Interaction Prompt: "Press F to insert Amulet"
```

### Amulet Visual

**Make it Glow:**
```csharp
Material:
  - Shader: Standard
  - Emission: Enabled
  - Emission Color: Yellow (intensity 0.5)
```

**Rotate Animation:**
Add script to AmuletVisual:
```csharp
void Update()
{
    transform.Rotate(Vector3.up, 50f * Time.deltaTime);
}
```

### Room Atmosphere

**Darker Mood:**
- Reduce torch intensity to 0.8
- Use darker floor material
- Add fog (Lighting > Fog)

**Brighter Victory:**
- Increase torch intensity to 2.5
- Use lighter gold materials
- Add particle effects

---

## Adding Victory Effects

### Particle Explosion
When door opens, spawn particles:
```csharp
// In AmuletDoorSlot.OnDoorFullyOpen()
ParticleSystem particles = Instantiate(victoryParticlesPrefab, door.transform.position, Quaternion.identity);
```

### Screen Flash
Flash screen white briefly:
```csharp
// Use UI Image with Canvas
StartCoroutine(FlashScreen());
```

### Camera Shake
Shake camera when Amulet placed:
```csharp
StartCoroutine(ShakeCamera(0.5f, 0.2f));
```

---

## Integration with Game Flow

### Scene Transition
**From Stage 6 (Darkness Zone):**
Player exits darkness zone → Enters Stage 7 final room

**After Stage 7:**
- Game ends (no further stages)
- Show victory screen
- Return to main menu
- Display final time/score

### Prerequisites
**CRITICAL**: Player MUST have Ancient Amulet from Stage 0.

**If player doesn't have it:**
- They cannot complete the game
- Must restart or load earlier save
- Consider adding a fallback (teleport back to Stage 0?)

---

## Victory Screen Integration

### After Door Opens
When `GameManager.CompleteGame()` is called:

**Option 1: Show UI Panel**
```csharp
victoryPanel.SetActive(true);
finalTimeText.text = $"Time: {gameManager.PlayTime:F1}s";
collectiblesText.text = $"Collectibles: {inventory.ItemCount}/3";
```

**Option 2: Load Victory Scene**
```csharp
SceneManager.LoadScene("VictoryScene");
```

**Option 3: Fade Out**
```csharp
StartCoroutine(FadeToBlack());
```

---

## Context Menu Testing

### AmuletDoorSlot Commands
Right-click `AmuletDoorSlot` component:
- **Force Place Amulet**: Places Amulet without checking inventory (testing only)

**Use this to:**
- Test door opening without playing through game
- Verify victory triggers work
- Check sounds and effects

---

## Troubleshooting

### Issue: Interaction prompt doesn't appear
**Solution:**
1. Check player is within `Interaction Range` (default: 3m)
2. Verify player has tag "Player"
3. Enable `Show Interaction Prompt` in AmuletDoorSlot
4. Check AmuletDoorSlot is active

### Issue: "Missing required item" even though I have Amulet
**Solution:**
1. Check exact item name matches: "Ancient Amulet" (case-sensitive)
2. Verify player has `Inventory` component
3. Check inventory actually contains the item
4. Try debug: `Debug.Log(playerInventory.HasItem("Ancient Amulet"))`

### Issue: Door doesn't open
**Solution:**
1. Check `door` field is assigned in AmuletDoorSlot
2. Verify door GameObject exists and is active
3. Check `Door Open Height` > 0
4. Try "Force Place Amulet" context menu
5. Enable debug logs to see if PlaceAmulet() is called

### Issue: Door opens but game doesn't end
**Solution:**
1. Check GameManager exists in scene
2. Verify GameManager has `CompleteGame()` method
3. Test VictoryZone trigger as backup
4. Enable debug logs to see if OnDoorFullyOpen() is called

### Issue: Player has Amulet but didn't get it from Stage 0
**Solution:**
- Player must actually collect it in Stage 0
- Check ItemPickup is set up correctly
- Verify item name matches exactly
- Test picking up Amulet in Stage 0 first

---

## Architecture

### Scripts
- **AmuletDoorSlot.cs**: Main interaction and door control
- **FinalRoomBuilder.cs**: Auto-generates entire room
- **VictoryTrigger.cs**: Optional backup victory trigger

### Game Flow
1. Player enters Stage 7 with Amulet
2. Approaches slot
3. Presses E to place Amulet
4. Door opens upward
5. GameManager.CompleteGame() called
6. Victory!

### Dependencies
- **GameManager.cs**: For CompleteGame() method
- **Inventory.cs**: For Amulet check
- **ItemPickup.cs**: For Amulet in Stage 0
- **DoorTrigger.cs**: (optional) if using scene transition instead

---

## Scene Hierarchy Example

```
Stage_7_FinalRoom
├── Floor
├── FinalDoor
│   └── DoorSymbol
├── AmuletSlot
│   ├── AmuletVisual (disabled)
│   └── InteractionPrompt
├── Pedestal
├── Torch_1
│   └── TorchLight_1
├── Torch_2
│   └── TorchLight_2
├── Torch_3
│   └── TorchLight_3
├── Torch_4
│   └── TorchLight_4
├── VictoryZone
└── SpawnPoint_Stage7
```

---

## Performance Notes
- Simple geometry (low poly count)
- Limited lights (4-6 total)
- No complex effects
- Suitable for all platforms

---

## Lore/Story Integration

**Atmospheric Description:**
> "You stand before the ancient door, its golden surface etched with mysterious symbols. In the center, a circular indentation waits for something... the Amulet! This is it - the final barrier between you and freedom. Place the Amulet and escape this place once and for all."

**Victory Message:**
> "🎉 The ancient door rumbles open, revealing a blinding light beyond. You step through, finally free from the labyrinth. **VICTORY!** You escaped!"

---

## Next Steps
1. Build Stage 7 using builder or manual setup
2. Ensure Ancient Amulet is in Stage 0
3. Test complete game flow (Stage 0 → Stage 7)
4. Add victory UI/effects
5. Congratulations! Your game is complete! 🎉

---

**You did it! The escape room is ready!** 🚪✨🎊
