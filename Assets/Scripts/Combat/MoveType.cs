using UnityEngine;

namespace InternationalKarate.Combat
{
    /// <summary>
    /// Defines all possible karate moves in the game
    /// </summary>
    public enum MoveType
    {
        None,

        // Basic Punches
        HighPunch,
        MidPunch,
        LowPunch,

        // Basic Kicks
        HighKick,
        MidKick,
        LowKick,
        RoundKick,

        // Special Moves
        LegSweep,
        JumpKick,

        // Defensive
        BlockHigh,
        BlockMid,
        BlockLow,

        // Turn Around Moves
        TurnPunch,
        TurnKick,
        TurnSweep
    }

    /// <summary>
    /// Defines the hit zone for attacks
    /// </summary>
    public enum HitZone
    {
        High,   // Head area
        Mid,    // Body area
        Low     // Legs area
    }

    /// <summary>
    /// Points awarded for successful hits
    /// </summary>
    public enum PointValue
    {
        None = 0,
        Half = 1,
        Full = 2
    }
}
