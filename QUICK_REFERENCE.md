# International Karate - Quick Reference

## Files Created

### Core Scripts (10 files)

#### Combat System
1. `MoveType.cs` - Enums for moves, zones, points
2. `MoveData.cs` - ScriptableObject for move configuration
3. `MoveExecutor.cs` - Move execution and hit detection

#### Characters
4. `FighterController.cs` - Main fighter controller

#### Input
5. `PlayerInput.cs` - Keyboard input handling

#### Managers
6. `MatchManager.cs` - Match flow and scoring
7. `GameManager.cs` - Game state management
8. `CameraController.cs` - 2D camera tracking
9. `DebugManager.cs` - Debug utilities

#### UI
10. `MatchUI.cs` - UI display controller

## Class Relationships

```
MatchManager (Singleton)
    ├── Manages → FighterController (x2)
    │   ├── Has → MoveExecutor
    │   │   └── Uses → MoveData (ScriptableObject)
    │   └── Has → PlayerInput
    └── Notifies → MatchUI (via Events)

CameraController
    └── Tracks → FighterController (x2)

GameManager (Singleton, DontDestroyOnLoad)
    └── Handles audio, scenes, settings
```

## Key Methods

### FighterController
```csharp
bool ExecuteMove(MoveType move)           // Execute an attack
void SetFacingDirection(bool facingRight) // Set facing
void TurnAround()                         // Turn 180 degrees
void Move(float direction)                // Move left/right
void ResetToStartPosition()               // Reset after hit
void OnHit(PointValue points)             // Called when hit
```

### MatchManager
```csharp
void StartMatch()                                    // Begin new match
void OnFighterHit(int playerNumber, PointValue pts) // Register hit
int GetPlayer1Score()                                // Get P1 score (max: 99999)
int GetPlayer2Score()                                // Get P2 score (max: 99999)
void RestartMatch()                                  // Restart
```

### MoveExecutor
```csharp
bool ExecuteMove(MoveType moveType) // Execute move with timing
```

## Unity Events (MatchManager)

```csharp
OnScoreChanged(int playerNum, int score)    // Score updated (max: 99999)
OnRoundWon(int playerNum)                   // Round ended
OnMatchWon(int playerNum)                   // Match ended
OnTimerUpdated(float timeRemaining)         // Timer tick
OnRoundStart()                              // Round begins
OnRoundEnd()                                // Round ends
```

## MoveData Configuration Template

```
Move Type: [Enum selection]
Animation Trigger: "AnimationName"
Hit Zone: High/Mid/Low
Point Value: Half/Full
Execution Time: 0.3 (time to hit)
Recovery Time: 0.2 (time to recover)
Hitbox Offset: (1.5, 0.5)
Hitbox Size: (1.0, 0.8)
```

## Typical Move Timings

| Move Type | Execution | Recovery | Points |
|-----------|-----------|----------|--------|
| High Punch | 0.25s | 0.3s | Half |
| Mid Punch | 0.25s | 0.3s | Half |
| Low Punch | 0.3s | 0.3s | Half |
| High Kick | 0.35s | 0.4s | Full |
| Mid Kick | 0.3s | 0.4s | Full |
| Low Kick | 0.35s | 0.4s | Full |
| Leg Sweep | 0.4s | 0.5s | Full |
| Jump Kick | 0.5s | 0.6s | Full |

## Animation Parameters Needed

### Triggers
- Idle
- Hit
- HighPunch
- MidPunch
- LowPunch
- HighKick
- MidKick
- LowKick
- LegSweep
- BlockHigh
- BlockMid
- BlockLow

### Bools
- IsWalking

## Fighter Setup Checklist

```
GameObject: Fighter
├── Transform (Position: Set per player)
├── SpriteRenderer (Sprite: Character sprite)
├── Animator (Controller: FighterAnimator)
├── BoxCollider2D (Is Trigger: ✓)
├── Rigidbody2D (Body Type: Kinematic)
├── FighterController
│   ├── Fighter Name: "Player 1"
│   ├── Is Player Controlled: ✓
│   ├── Player Number: 1
│   └── Is Facing Right: ✓
├── MoveExecutor
│   └── Available Moves: [All MoveData assets]
└── PlayerInput
    └── Player Number: 1
```

## Scene Setup Checklist

```
Scene: GameScene
├── Main Camera
│   ├── Transform: (0, 0, -10)
│   ├── Projection: Orthographic
│   ├── Size: 6
│   └── CameraController
│       ├── Fighter1: [Assign]
│       ├── Fighter2: [Assign]
│       └── Boundaries: Set limits
│
├── Background (Sprite)
│   └── Transform: (0, 0, 1)
│
├── Ground (Collider)
│   └── Transform: (0, -3, 0)
│
├── Fighter1 (Prefab)
│   └── Transform: (-4, -1.5, 0)
│
├── Fighter2 (Prefab)
│   └── Transform: (4, -1.5, 0)
│
├── MatchManager
│   ├── Fighter1: [Assign]
│   ├── Fighter2: [Assign]
│   ├── Round Duration: 60
│   └── Points To Win: 2
│
├── DebugManager (Optional)
│   └── Enable Debug Mode: ✓
│
└── UI Canvas
    ├── CanvasScaler (1920x1080)
    ├── Player1Score (TextMeshPro)
    ├── Player2Score (TextMeshPro)
    ├── Timer (TextMeshPro)
    ├── RoundStartPanel
    ├── RoundEndPanel
    └── MatchUI
        └── [Assign all UI references]
```

## Common Issues & Fixes

| Issue | Cause | Fix |
|-------|-------|-----|
| Fighters not moving | Rigidbody not Kinematic | Set Body Type to Kinematic |
| Hits not detected | Colliders not triggers | Set BoxCollider2D Is Trigger ✓ |
| Animations not playing | Trigger names mismatch | Match MoveData.animationTrigger to Animator |
| UI not updating | Missing event listeners | Assign OnScoreChanged etc. in MatchUI |
| Score shows "0.0" | TextMeshPro not assigned | Assign UI references in MatchUI |
| Camera not following | Transforms not assigned | Assign Fighter1/2 to CameraController |

## Debug Hotkeys

```
R - Restart match
P - Pause/Resume
1 - Force Player 1 win (add 2 points)
2 - Force Player 2 win (add 2 points)
```

## Extending the System

### Adding a New Move
1. Add enum to `MoveType.cs`
2. Create MoveData asset
3. Add animation to Animator Controller
4. Add trigger in Animator parameters
5. Add to fighter's Available Moves array
6. (Optional) Add input binding in PlayerInput

### Creating an AI Opponent
1. Create `AIController.cs` in Scripts/Characters/
2. Replace PlayerInput with AIController
3. Implement decision-making logic
4. Use `fighter.ExecuteMove()` to attack

### Adding Particle Effects
1. Create particle system prefab
2. Reference in MoveData
3. Instantiate in MoveExecutor.PerformMove()
4. Use Object Pooling for performance

## Performance Notes

- **Target**: 60 FPS
- **Colliders**: Use minimal BoxCollider2D
- **Animations**: Keep frame counts reasonable (6-12 frames)
- **Audio**: Use compressed formats for music
- **Sprites**: Use sprite atlases to reduce draw calls

## Testing Strategy

1. **Movement**: Test both players move correctly
2. **Attacks**: Each move hits at right timing
3. **Blocking**: Blocks prevent hits in correct zones
4. **Scoring**: Half/full points awarded correctly
5. **Win Condition**: Match ends at 2 points
6. **Timer**: Round ends when timer hits 0
7. **Reset**: Fighters return to start after hit
8. **Turn Around**: Players can face opposite direction

---

**Pro Tip**: Start with just 3 moves (High Punch, Mid Kick, Block) to test the system, then add remaining moves once core mechanics work.
