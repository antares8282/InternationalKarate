# International Karate - System Architecture

## System Overview

```
┌─────────────────────────────────────────────────────────────┐
│                      GAME SCENE                              │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────┐              ┌──────────────┐             │
│  │   Fighter 1  │◄────────────►│   Fighter 2  │             │
│  │  (Player)    │    Combat    │  (Player/AI) │             │
│  └──────┬───────┘              └──────┬───────┘             │
│         │                              │                      │
│         │         ┌──────────┐        │                      │
│         └────────►│  Match   │◄───────┘                      │
│                   │ Manager  │                               │
│                   └────┬─────┘                               │
│                        │                                      │
│                        │ Events                              │
│                        ▼                                      │
│                   ┌─────────┐                                │
│    ┌──────────────┤ Match UI├──────────┐                    │
│    │              └─────────┘           │                    │
│    │                                     │                    │
│  ┌─▼─────┐  ┌────────┐  ┌──────┐    ┌─▼────┐               │
│  │ Score │  │ Timer  │  │ Info │    │Winner│               │
│  └───────┘  └────────┘  └──────┘    └──────┘               │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

## Component Hierarchy

```
Fighter GameObject
│
├─── SpriteRenderer          [Displays character sprite]
├─── Animator               [Plays animations]
├─── BoxCollider2D          [Hit detection]
├─── Rigidbody2D            [Physics (Kinematic)]
│
├─── FighterController      [Main controller]
│    ├─ Fighter identity
│    ├─ Facing direction
│    ├─ Movement logic
│    └─ State management
│
├─── MoveExecutor           [Combat execution]
│    ├─ Move timing
│    ├─ Hit detection
│    ├─ Block checking
│    └─ Uses MoveData ───┐
│                         │
└─── PlayerInput           │    [Input handling]
     └─ Keyboard controls  │
                           │
                           ▼
                    ┌──────────────┐
                    │   MoveData   │ (ScriptableObject)
                    ├──────────────┤
                    │ • Move type  │
                    │ • Animation  │
                    │ • Timings    │
                    │ • Hitbox     │
                    │ • Points     │
                    │ • Sounds     │
                    └──────────────┘
```

## Data Flow - Combat System

```
Player Input                     Fighter 1                    Fighter 2
     │                               │                            │
     │ Press Attack Key              │                            │
     ├──────────────────────────────►│                            │
     │                               │                            │
     │                          Execute Move                      │
     │                               │                            │
     │                          Play Animation                    │
     │                               │                            │
     │                          Wait (Execution Time)             │
     │                               │                            │
     │                          Perform Hit Detection             │
     │                               ├───────────────────────────►│
     │                               │    Check Collision         │
     │                               │                            │
     │                               │◄───────────────────────────┤
     │                               │    Is Hit? / Is Blocked?   │
     │                               │                            │
     │                          Notify MatchManager               │
     │                               │                            │
     │                               ▼                            │
     │                         MatchManager                       │
     │                               │                            │
     │                          Award Points                      │
     │                               │                            │
     │                          Fire Events                       │
     │                               │                            │
     │                         ┌─────┴─────┐                     │
     │                         │           │                     │
     │                         ▼           ▼                     │
     │                    Update UI    Check Win Condition       │
     │                                      │                     │
     │                                      │                     │
     │                         ┌────────────┴───────────┐        │
     │                         │                        │        │
     │                         ▼                        ▼        │
     │                   Continue Round           End Round      │
     │                         │                        │        │
     │                         │                  Show Winner    │
     │                    Reset Positions         Restart Option │
     │                         │                                 │
     │                    Resume Combat                          │
```

## Event System Flow

```
MatchManager (Publisher)          MatchUI (Subscriber)
     │                                   │
     │ OnScoreChanged ───────────────────┼──► Update Score Display
     │                                   │
     │ OnTimerUpdated ───────────────────┼──► Update Timer Display
     │                                   │
     │ OnRoundStart ─────────────────────┼──► Show "FIGHT!" Message
     │                                   │
     │ OnRoundEnd ───────────────────────┼──► Clear Round Info
     │                                   │
     │ OnMatchWon ───────────────────────┼──► Show Winner
     │                                   │    Show Restart Button
```

## Scene Setup Diagram

```
                    Main Camera
                    [Orthographic]
                    [CameraController]
                         │
           ┌─────────────┼─────────────┐
           │             │             │
       Track F1      Track F2      Adjust Zoom
           │             │
           ▼             ▼
    ┌──────────┐  ┌──────────┐
    │ Fighter1 │  │ Fighter2 │
    │  @ -4,0  │  │  @ +4,0  │
    │ Facing → │  │ ← Facing │
    └──────────┘  └──────────┘
           │             │
           └─────┬───────┘
                 │
                 ▼
         ┌──────────────┐
         │ MatchManager │
         │  (Singleton) │
         └──────┬───────┘
                │
                ▼
           ┌─────────┐
           │MatchUI  │
           └─────────┘
```

## State Machine - Round Flow

```
     [IDLE]
        │
        │ StartMatch()
        ▼
   [MATCH_ACTIVE]
        │
        │ StartNewRound()
        ▼
   [ROUND_ACTIVE] ◄──────────────┐
        │                         │
        │ Timer Running          │
        │ Combat Active          │
        │                         │
        ├──► Hit Detected        │
        │         │               │
        │         ▼               │
        │    Award Points        │
        │         │               │
        │         ├─ < 2 points? ─┘
        │         │    Reset & Continue
        │         │
        │         ├─ >= 2 points?
        │         │
        ▼         ▼
   [ROUND_END]
        │
        ├──► Determine Winner
        │
        ▼
   [MATCH_WON]
        │
        ├──► Show Winner
        │
        │ Restart?
        │
        ├─ Yes ──► [MATCH_ACTIVE]
        │
        └─ No ───► [IDLE]
```

## Fighter State Machine

```
        [IDLE]
          │
          │
    ┌─────┼─────┬─────────┬──────────┐
    │     │     │         │          │
    ▼     ▼     ▼         ▼          ▼
[WALKING] [ATTACKING] [BLOCKING] [HIT] [TURNING]
    │     │     │         │          │
    │     │     │         │          │
    └─────┴─────┴─────────┴──────────┘
          │
          ▼
        [IDLE]

Transitions:
• IDLE → WALKING: Move input detected
• IDLE → ATTACKING: Attack input detected
• IDLE → BLOCKING: Block input detected
• IDLE → TURNING: Turn input detected
• ANY → HIT: Received hit from opponent
• ALL → IDLE: Animation complete
```

## Combat Timing Diagram

```
Time →
│
├─ T0: Input Received
│      │
│      └─► Trigger Animation
│
├─ T1: Execution Time (e.g., 0.3s)
│      │
│      ├─► Hitbox Active
│      │
│      └─► Perform Hit Detection
│             │
│             ├─ Hit? → Award Points
│             │
│             └─ Miss? → Continue
│
├─ T2: Recovery Time (e.g., 0.2s)
│      │
│      └─► Return to Idle
│
├─ T3: Ready for Next Input
│
```

## Hitbox Detection System

```
Fighter 1 Position                Fighter 2 Position
       │                                 │
       │     Hitbox                      │
       │   ┌─────────┐                  │
       │   │         │                  │
       ├───┤◄─offset─┤                  │
       │   │  size   │                  │
       │   └────┬────┘                  │
       │        │                        │
       │        │ Check Overlap          │
       │        └───────────────────────►│
       │                                 │
       │                     ┌───────────┤
       │                     │ Collider  │
       │                     └───────────┤
       │                                 │
       │        Overlap? ────────────────┘
       │            │
       │            ├─ Yes → Check Block
       │            │           │
       │            │           ├─ Blocked → No Points
       │            │           │
       │            │           └─ Not Blocked → Award Points
       │            │
       │            └─ No → Miss
```

## Modular Architecture Benefits

```
┌────────────────────────────────────────────┐
│         MODULARITY ADVANTAGES               │
├────────────────────────────────────────────┤
│                                             │
│  FighterController                          │
│    ↓                                        │
│  Can be controlled by:                      │
│    • PlayerInput ──────────► Local Player  │
│    • AIController ─────────► CPU Opponent  │
│    • NetworkInput ─────────► Online Player │
│    • ReplayInput ──────────► Replay System │
│                                             │
│  MoveData (ScriptableObject)                │
│    ↓                                        │
│  Can be:                                    │
│    • Swapped at runtime ───► Different     │
│    • Modified by power-ups ► Abilities     │
│    • Character-specific ───► Unique Moves  │
│    • Difficulty-scaled ────► Balancing     │
│                                             │
│  MatchManager (Events)                      │
│    ↓                                        │
│  Can notify:                                │
│    • MatchUI ──────────────► Score Display │
│    • AchievementSystem ────► Unlocks       │
│    • StatisticsTracker ────► Analytics     │
│    • AudioManager ─────────► Sound Effects │
│                                             │
└────────────────────────────────────────────┘
```

## Extension Points

```
Current System              Future Extensions
     │                            │
     ├─ FighterController         ├─ Add Special Moves
     │                            ├─ Add Combo System
     │                            └─ Add Stamina/Energy
     │
     ├─ MoveExecutor              ├─ Add Juggle System
     │                            ├─ Add Counter Attacks
     │                            └─ Add Throw System
     │
     ├─ MatchManager              ├─ Add Tournament Mode
     │                            ├─ Add Best of 3 Rounds
     │                            └─ Add Character Select
     │
     ├─ CameraController          ├─ Add Zoom on Hit
     │                            ├─ Add Screen Shake
     │                            └─ Add Slow Motion
     │
     └─ MatchUI                   ├─ Add Health Bars (optional)
                                  ├─ Add Combo Counter
                                  └─ Add Character Portraits
```

## Performance Considerations

```
Optimization Strategy
│
├─ Object Pooling
│   ├─ Particle Effects
│   ├─ Sound Effects
│   └─ UI Elements
│
├─ Sprite Atlases
│   ├─ Character Sprites
│   └─ UI Sprites
│
├─ Audio Optimization
│   ├─ Compressed Music (Vorbis)
│   └─ Uncompressed SFX (PCM)
│
├─ Update Optimization
│   ├─ Event-driven UI (not Update)
│   ├─ Physics set to Kinematic
│   └─ Minimal per-frame checks
│
└─ Memory Management
    ├─ ScriptableObject reuse
    ├─ Shared materials
    └─ Texture compression
```

---

This architecture provides a solid foundation for a classic 2D fighting game while remaining flexible for future enhancements.
