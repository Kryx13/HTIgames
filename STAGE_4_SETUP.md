# Stage 4: Riddle Room - Setup Guide

## Overview
Stage 4 is a puzzle room where players must solve riddles displayed on stone tablets (steles). When enough riddles are solved, an exit platform appears and the door to Stage 5 opens.

## Features
- **Interactive Steles**: Stone tablets that display riddles
- **Answer System**: UI-based answer input with feedback
- **Hint System**: Hints appear after wrong attempts
- **Sequence Tracking**: Manager tracks which riddles are solved
- **Auto-Generation**: Builder script creates entire room automatically

---

## Quick Setup (Auto-Build)

### 1. Create Empty GameObject
1. In Unity, create empty GameObject: `GameObject > Create Empty`
2. Name it: `Stage_4_RiddleRoomBuilder`
3. Position: (0, 0, 0)

### 2. Add Builder Component
1. Select the GameObject
2. Add Component: `RiddleRoomBuilder`
3. Configure settings (or use defaults):
   - **Number of Riddles**: 3 (default)
   - **Riddles Required**: 3 (how many must be solved)
   - **Room Size**: (20, 10, 20)
   - **Spawn Position**: (0, 1, -8)
   - **Exit Platform Position**: (0, 0, 15)

### 3. Build the Room
1. Right-click `RiddleRoomBuilder` component
2. Select: `Build Riddle Room`
3. Wait for: "✅ Riddle Room built successfully!"

### 4. Add RiddleUI
1. Create empty GameObject: `RiddleUI_Manager`
2. Add Component: `RiddleUI`
3. Enable: `Auto Create UI`
4. The UI will auto-create when scene starts

### 5. Test
1. Enter Play Mode
2. Walk to a stele
3. Press **E** to interact
4. Type answer in UI
5. Click "Submit" or press Enter

---

## Manual Setup (Custom Room)

### Components Needed
1. **Steles** (riddle tablets)
2. **RiddleManager** (tracks progress)
3. **RiddleUI** (answer input)
4. **Exit Platform** (appears when solved)
5. **Exit Door** (to next stage)

### Step 1: Create Stele
1. Create Cube: `GameObject > 3D Object > Cube`
2. Name: `Stele_1`
3. Scale: (1, 2, 0.5)
4. Add Component: `Stele`
5. Configure:
   - **Riddle Number**: 1
   - **Riddle Question**: "Your riddle here..."
   - **Correct Answer**: "answer"
   - **Alternative Answers**: (optional variants)
   - **Hint**: "Your hint..."

### Step 2: Add More Steles
1. Duplicate `Stele_1` for each riddle
2. Rename: `Stele_2`, `Stele_3`, etc.
3. Position them around the room
4. Update each riddle's content

### Step 3: Create RiddleManager
1. Create Empty: `GameObject > Create Empty`
2. Name: `RiddleManager`
3. Add Component: `RiddleManager`
4. Configure:
   - **Riddles**: Drag all Stele GameObjects into array
   - **Riddles Required**: 3 (or desired count)
   - **Exit Platform**: Reference to platform object
   - **Exit Door**: Reference to door object

### Step 4: Create Exit Platform
1. Create Cube: `GameObject > 3D Object > Cube`
2. Name: `ExitPlatform`
3. Scale: (5, 0.5, 5)
4. Position: Where players will land
5. Material: Cyan or glowing material
6. **Disable GameObject** (will activate when riddles solved)

### Step 5: Create Exit Door
1. Create Cube or use Door Prefab
2. Name: `ExitDoor_ToStage5`
3. Add Component: `DoorTrigger`
4. Configure:
   - **Door Type**: Scene Transition
   - **Target Scene Name**: "Stage_5_Maze"
5. Add BoxCollider, set **Is Trigger**: ✓
6. **Disable GameObject** (will activate when riddles solved)

### Step 6: Add RiddleUI
1. Create Empty: `RiddleUI_Manager`
2. Add Component: `RiddleUI`
3. Enable: `Auto Create UI` (or manually create panel)

---

## Predefined Riddles

The builder includes 5 pre-made riddles:

### Riddle 1
- **Question**: "I speak without a mouth and hear without ears. I have no body, but come alive with wind. What am I?"
- **Answer**: "echo"
- **Alternatives**: "an echo"
- **Hint**: "Think about sound bouncing off walls..."

### Riddle 2
- **Question**: "The more you take, the more you leave behind. What am I?"
- **Answer**: "footsteps"
- **Alternatives**: "steps", "footprints"
- **Hint**: "Think about walking..."

### Riddle 3
- **Question**: "I have cities, but no houses. I have mountains, but no trees. I have water, but no fish. What am I?"
- **Answer**: "map"
- **Alternatives**: "a map"
- **Hint**: "You use me to navigate..."

### Riddle 4
- **Question**: "What has keys but no locks, space but no room, and you can enter but can't go inside?"
- **Answer**: "keyboard"
- **Alternatives**: "a keyboard"
- **Hint**: "You're probably using one right now..."

### Riddle 5
- **Question**: "I am taken from a mine and shut up in a wooden case, from which I am never released. What am I?"
- **Answer**: "pencil lead"
- **Alternatives**: "lead", "graphite", "pencil"
- **Hint**: "Think about writing instruments..."

---

## How It Works

### Player Interaction Flow
1. Player approaches stele (within 3 units)
2. Press **E** to interact
3. UI panel appears with riddle question
4. Player types answer
5. Clicks "Submit" or presses Enter
6. Feedback appears:
   - **Correct**: "Correct! Well done!" (green)
   - **Incorrect**: "Incorrect. Try again!" (red)
7. After wrong attempts, hint appears in riddle text
8. When riddle solved, stele turns green

### Manager Tracking
- `RiddleManager` listens to each stele's `onRiddleSolved` event
- Counts solved riddles
- When `riddlesSolved >= riddlesRequired`:
  - Activates exit platform
  - Flashes platform (cyan)
  - Activates exit door
  - Plays completion sound
  - Debug: "🎯 RIDDLE ROOM COMPLETE! Exit unlocked."

### Visual Feedback
- **Unsolved Stele**: Gray color
- **Solved Stele**: Green color
- **Exit Platform**: Cyan flash animation (5 flashes)
- **Riddle Text**: Displays above each stele

---

## Testing Commands

### Stele Context Menu
Right-click `Stele` component:
- **Force Solve**: Instantly solves riddle (testing)
- **Reset Riddle**: Resets riddle to unsolved state

### RiddleManager Context Menu
Right-click `RiddleManager` component:
- **Reset Room**: Resets all riddles and exits

### Debug Logs
Enable `Show Debug Logs` in components to see:
- `📜 Stele X initialized`
- `📜 Riddle X: [question]`
- `✅ Riddle X SOLVED!`
- `❌ Wrong answer! Attempts: X`
- `📊 Riddles solved: X/Y`
- `🎯 RIDDLE ROOM COMPLETE!`

---

## Customization

### Adding Custom Riddles
1. Select a Stele GameObject
2. In Inspector, modify:
   - **Riddle Question**: Your custom question
   - **Correct Answer**: The answer (case-insensitive)
   - **Alternative Answers**: Other acceptable answers
   - **Hint**: Help text after wrong attempts
3. Configure hint settings:
   - **Show Hint After Attempts**: ✓
   - **Attempts Before Hint**: 2

### Changing Difficulty
- **Easy**: `riddlesRequired = 2` (solve 2 of 3)
- **Medium**: `riddlesRequired = 3` (solve 3 of 3)
- **Hard**: `riddlesRequired = 5` (solve all 5)

### Visual Customization
- **Stele Material**: Assign custom stone texture
- **Exit Platform Material**: Glowing or neon material
- **Colors**: Modify in Stele component:
  - `Unsolved Color`: Default gray
  - `Solved Color`: Default green

---

## Integration with Game Flow

### Scene Transition
When player enters exit door:
1. `DoorTrigger` detects player
2. Loads: `Stage_5_Maze`
3. Spawns player at `SpawnPoint_Stage5`

### Prerequisites
None - Stage 4 is accessible after completing Stage 3.

### Next Stage
After Stage 4, player proceeds to **Stage 5: Destructible Maze**.

---

## Troubleshooting

### Issue: Riddle UI doesn't appear
**Solution**:
1. Check `RiddleUI` component exists in scene
2. Enable `Auto Create UI`
3. Ensure Canvas is in scene

### Issue: Steles don't respond to interaction
**Solution**:
1. Check player has tag "Player"
2. Verify `Interaction Range` in Stele (default: 3 units)
3. Check `Interact Key` (default: E)

### Issue: Exit doesn't appear after solving riddles
**Solution**:
1. Check `RiddleManager` has steles assigned
2. Verify `Riddles Required` <= number of steles
3. Check exit objects are assigned in RiddleManager
4. Use context menu: "Reset Room" and try again

### Issue: Answers not accepting
**Solution**:
1. Answers are case-insensitive ("Echo" = "echo")
2. Check spelling in `Correct Answer` field
3. Add variants to `Alternative Answers`
4. Enable debug logs to see what's being compared

---

## Architecture

### Scripts
- **Stele.cs**: Individual riddle tablet with interaction
- **RiddleManager.cs**: Tracks all riddles and progression
- **RiddleUI.cs**: UI panel for answer input
- **RiddleRoomBuilder.cs**: Auto-generates entire room

### Events
- `Stele.onRiddleSolved`: Fired when riddle solved
- `RiddleManager` listens to all stele events

### Dependencies
- **TextMeshPro**: For riddle text and UI
- **DoorTrigger**: For exit door scene transition
- **SpawnPoint**: For player spawn location

---

## Scene Hierarchy Example

```
Stage_4_RiddleRoom
├── Floor
├── Steles
│   ├── Stele_1
│   │   └── RiddleText
│   ├── Stele_2
│   │   └── RiddleText
│   └── Stele_3
│       └── RiddleText
├── ExitPlatform (disabled)
├── ExitDoor_ToStage5 (disabled)
└── SpawnPoint_Stage4

RiddleUI_Manager
└── (RiddleUI component with auto-created Canvas)

Canvas
└── RiddlePanel
    ├── RiddleNumberText
    ├── RiddleQuestionText
    ├── AnswerInputField
    ├── SubmitButton
    └── FeedbackText
```

---

## Performance Notes
- Riddle checks only run when player is nearby
- UI only creates when needed
- No physics-heavy objects
- Suitable for all platforms

---

## Next Steps
1. Build Stage 4 using builder or manual setup
2. Test all riddles work correctly
3. Customize riddles for your game
4. Proceed to **Stage 6: Darkness Zone** (Stage 5 already built)

---

**Good luck with your riddles!** 📜✨
