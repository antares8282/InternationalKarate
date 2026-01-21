using UnityEngine;

namespace InternationalKarate.Combat
{
    /// <summary>
    /// Scriptable Object containing data for each move
    /// </summary>
    [CreateAssetMenu(fileName = "New Move", menuName = "International Karate/Move Data")]
    public class MoveData : ScriptableObject
    {
        [Header("Move Identification")]
        public MoveType moveType;
        public string moveName;
        public string animationTrigger;

        [Header("Move Properties")]
        public HitZone hitZone;
        public PointValue pointValue;
        public float damage;
        public float range;
        public float executionTime;
        public float recoveryTime;

        [Header("Hit Detection")]
        public Vector2 hitboxOffset;
        public Vector2 hitboxSize;

        [Header("Movement")]
        public bool canMoveForward;
        public float forwardDistance;

        [Header("Audio")]
        public AudioClip executionSound;
        public AudioClip hitSound;
        public AudioClip missSound;
    }
}
