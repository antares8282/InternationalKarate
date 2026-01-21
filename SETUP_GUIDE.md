# International Karate - Unity 2D Fighting Game
## Setup Guide

This project is inspired by the classic International Karate game, implementing a 1v1 karate fighting system with authentic mechanics.

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Characters/         # Fighter controllers and behavior
│   ├── Combat/            # Combat system, moves, hit detection
│   ├── Input/             # Player input handling
│   ├── Managers/          # Game, Match, and Camera managers
│   └── UI/                # UI controllers
├── Sprites/
│   ├── Characters/        # Character sprite sheets
│   └── Backgrounds/       # Background images
├── Animations/            # Animation controllers and clips
├── Prefabs/              # Reusable game objects
└── Audio/
    ├── Music/            # Background music
    └── SFX/              # Sound effects
```

---

## Game Mechanics (Based on International Karate)

### Core Features
- **Point-based scoring**: Half-points (0.5) and full-points (1.0)
- **2 points to win** a round
- **Pause and reset** after each successful hit
- **60-second rounds** (configurable)
- **No auto-turn**: Players must execute turn-around moves
- **Variety of moves**: High/Mid/Low punches, kicks, sweeps, blocks

### Controls

#### Player 1 (Keyboard)
- **Movement**: A (left), D (right)
- **Punches**: W (high), S (mid), X (low)
- **Kicks**: Q (mid), E (high), Z (low)
- **Defense**: Left Shift (block)
- **Turn Around**: C

#### Player 2 (Keyboard)
- **Movement**: Left/Right arrows
- **Punches**: Up (high), Down (mid), Period (low)
- **Kicks**: Right Shift (mid), Right Ctrl (high), Comma (low)
- **Defense**: Slash (block)
- **Turn Around**: M

---

## Setup Instructions

### 1. Import Your Assets

#### Character Sprites
1. Place your character sprite sheets in `Assets/Sprites/Characters/`
2. Set sprite mode to "Multiple" if using sprite sheets
3. Use the Sprite Editor to slice your animations
4. Set Pixels Per Unit based on your sprite resolution (typically 100)

#### Background Sprites
1. Place background images in `Assets/Sprites/Backgrounds/`
2. Set sprite mode to "Single"
3. Adjust import settings for appropriate quality

#### Audio Files
1. Place music files in `Assets/Audio/Music/`
2. Place sound effects in `Assets/Audio/SFX/`
3. Set audio clip settings:
   - Music: Compressed in memory, Vorbis format
   - SFX: Decompress on load, PCM format

### 2. Create Move Data Assets

1. In Unity, right-click in Project window
2. Navigate to `Create > International Karate > Move Data`
3. Create moves for each attack type:
   - High Punch, Mid Punch, Low Punch
   - High Kick, Mid Kick, Low Kick
   - Leg Sweep, Jump Kick
   - Block High, Block Mid, Block Low

4. Configure each MoveData asset:
   ```
   Move Type: [Select from dropdown]
   Move Name: Descriptive name
   Animation Trigger: Name matching your animator parameter
   Hit Zone: High/Mid/Low
   Point Value: Half/Full
   Execution Time: Time until hit detection (e.g., 0.3)
   Recovery Time: Time to return to idle (e.g., 0.2)
   Hitbox Offset: Position relative to fighter
   Hitbox Size: Width and height of attack area
   ```

### 3. Set Up Fighter Animations

1. Create an Animator Controller for your fighter:
   - Right-click > Create > Animator Controller
   - Name it "FighterAnimator"

2. Add Animation Parameters:
   - **Triggers**: Idle, Hit, HighPunch, MidPunch, LowPunch, HighKick, MidKick, LowKick, etc.
   - **Bool**: IsWalking

3. Create Animation Clips from your sprites:
   - Select sprite frames in order
   - Drag into Scene/Hierarchy to create animation
   - Save animation clip

4. Set up State Machine:
   - Idle as default state
   - Create states for each move
   - Add transitions with corresponding triggers

### 4. Create Fighter Prefabs

1. Create an empty GameObject in the scene
2. Name it "Fighter"
3. Add components:
   - SpriteRenderer
   - Animator (assign your Animator Controller)
   - BoxCollider2D (set as Trigger)
   - Rigidbody2D (set to Kinematic)
   - FighterController script
   - MoveExecutor script
   - PlayerInput script

4. Configure FighterController:
   ```
   Fighter Name: Player name
   Is Player Controlled: True
   Player Number: 1 or 2
   Is Facing Right: True for P1, False for P2
   ```

5. Configure MoveExecutor:
   - Assign all MoveData assets to "Available Moves" array

6. Configure PlayerInput:
   - Set Player Number (1 or 2)
   - Adjust key bindings if desired

7. Save as prefab in `Assets/Prefabs/`

### 5. Set Up the Game Scene

1. **Main Camera**:
   - Add CameraController script
   - Set Position: (0, 0, -10)
   - Projection: Orthographic
   - Size: 6

2. **Background**:
   - Create Sprite GameObject
   - Assign background sprite
   - Position at (0, 0, 1)
   - Scale to fill camera view

3. **Ground**:
   - Create empty GameObject named "Ground"
   - Position at (0, -3, 0)
   - Add BoxCollider2D (not trigger)

4. **Fighters**:
   - Drag Fighter prefab into scene (create two instances)
   - Position Player 1 at (-4, -1.5, 0)
   - Position Player 2 at (4, -1.5, 0)
   - Ensure Player 1 faces right, Player 2 faces left

5. **Match Manager**:
   - Create empty GameObject named "MatchManager"
   - Add MatchManager script
   - Assign Fighter1 and Fighter2 references
   - Configure:
     ```
     Round Duration: 60
     Points To Win: 2
     Reset Delay: 2
     ```

6. **UI Canvas**:
   - Create UI > Canvas
   - Add CanvasScaler (set to Scale with Screen Size)
   - Reference resolution: 1920x1080

7. **UI Elements** (as children of Canvas):
   - **Player 1 Score**: TextMeshPro (top-left)
   - **Player 2 Score**: TextMeshPro (top-right)
   - **Timer**: TextMeshPro (top-center)
   - **Round Info Panel**: Panel with TextMeshPro for "FIGHT!" message
   - **Round End Panel**: Panel with winner text and restart button

8. **UI Manager**:
   - Add MatchUI script to Canvas
   - Assign all UI element references

9. **Camera Setup**:
   - Assign Fighter1 and Fighter2 transforms to CameraController
   - Adjust boundaries to match your scene

### 6. Testing

1. Press Play in Unity Editor
2. Test both players' controls
3. Verify hit detection works
4. Check scoring system
5. Test round win/loss conditions
6. Verify timer countdown

---

## Creating Move Data - Detailed Example

### High Punch Configuration
```
Move Type: HighPunch
Move Name: "High Punch"
Animation Trigger: "HighPunch"

Hit Zone: High
Point Value: Half
Damage: 0.5
Range: 1.5
Execution Time: 0.25
Recovery Time: 0.3

Hitbox Offset: (1.2, 0.8)
Hitbox Size: (0.8, 0.6)

Can Move Forward: true
Forward Distance: 0.5

Execution Sound: [Assign punch whoosh sound]
Hit Sound: [Assign impact sound]
```

### Low Sweep Configuration
```
Move Type: LegSweep
Move Name: "Leg Sweep"
Animation Trigger: "LegSweep"

Hit Zone: Low
Point Value: Full
Damage: 1.0
Range: 1.8
Execution Time: 0.4
Recovery Time: 0.5

Hitbox Offset: (1.5, -0.5)
Hitbox Size: (1.2, 0.4)

Can Move Forward: false
Forward Distance: 0

Execution Sound: [Assign sweep sound]
Hit Sound: [Assign impact sound]
```

---

## Troubleshooting

### Fighters Not Moving
- Check if Rigidbody2D is set to Kinematic
- Verify PlayerInput script is enabled
- Check key bindings match your input

### Hits Not Detecting
- Ensure both fighters have BoxCollider2D set as Trigger
- Verify MoveData hitbox settings
- Check MatchManager is assigned in scene
- Verify fighter layer settings allow collision

### Animations Not Playing
- Check Animator Controller is assigned
- Verify animation triggers match MoveData
- Ensure animation clips are properly created
- Check Animator parameters are set up

### UI Not Updating
- Verify MatchUI script references are assigned
- Check MatchManager events are firing
- Ensure Canvas is set to Screen Space

---

## Next Steps

### Phase 1 - Polish
- Add particle effects for hits
- Implement screen shake on impact
- Add combo system
- Create victory/defeat animations

### Phase 2 - AI Opponent
- Create AIController script
- Implement decision-making system
- Add difficulty levels (white belt to black belt)
- Pattern recognition and counter-attacks

### Phase 3 - Bonus Games
- Board breaking mini-game
- Timing-based challenges
- Rhythm challenges between matches

### Phase 4 - Game Modes
- Tournament mode
- Practice mode
- Training mode with move list

---

## Performance Tips

1. Use sprite atlases to reduce draw calls
2. Pool audio sources for sound effects
3. Optimize particle systems
4. Use object pooling for frequently created objects
5. Profile regularly using Unity Profiler

---

## References

- [International Karate on Wikipedia](https://en.wikipedia.org/wiki/International_Karate)
- [Classic gameplay video](https://www.youtube.com/watch?v=6BOXR008V2I)

Sources:
- [International Karate - Wikipedia](https://en.wikipedia.org/wiki/International_Karate)
- [International Karate - The Best 8-bit Fighting Game? - 80s Heaven](https://80sheaven.com/international-karate-the-best-8-bit-fighting-game/)
