using UnityEngine;

namespace InternationalKarate.Combat
{
    public enum BodyZone
    {
        Head,
        Torso,
        Feet,
        Shin,
        LeftFoot,
        RightFoot
    }

    /// <summary>
    /// Attach to player GameObjects. Creates hurtboxes (areas that can be hit)
    /// and provides methods to check hitbox collisions during attacks.
    /// </summary>
    public class HitboxSystem : MonoBehaviour
    {
        [Header("Player Settings")]
        public int playerNumber = 1;

        [Header("Hurtbox Offsets (relative to player position)")]
        public Vector2 headOffset = new Vector2(0f, 1.8f);
        public Vector2 headSize = new Vector2(1.0f, 0.7f);

        public Vector2 torsoOffset = new Vector2(0f, 1.1f);
        public Vector2 torsoSize = new Vector2(0.8f, 0.9f);

        public Vector2 feetOffset = new Vector2(0f, 0.3f);
        public Vector2 feetSize = new Vector2(1.0f, 0.6f);

        public Vector2 shinOffset = new Vector2(0f, 0.5f);
        public Vector2 shinSize = new Vector2(1.2f, 0.5f);

        public Vector2 leftFootOffset = new Vector2(-0.3f, 0.15f);
        public Vector2 leftFootSize = new Vector2(0.5f, 0.4f);

        public Vector2 rightFootOffset = new Vector2(0.3f, 0.15f);
        public Vector2 rightFootSize = new Vector2(0.5f, 0.4f);

        [Header("Debug")]
        public bool showDebugGizmos = true;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Get the world-space bounds of a body zone
        /// </summary>
        public Bounds GetHurtbox(BodyZone zone)
        {
            Vector2 offset;
            Vector2 size;

            switch (zone)
            {
                case BodyZone.Head:
                    offset = headOffset;
                    size = headSize;
                    break;
                case BodyZone.Torso:
                    offset = torsoOffset;
                    size = torsoSize;
                    break;
                case BodyZone.Feet:
                    offset = feetOffset;
                    size = feetSize;
                    break;
                case BodyZone.Shin:
                    offset = shinOffset;
                    size = shinSize;
                    break;
                case BodyZone.LeftFoot:
                    offset = leftFootOffset;
                    size = leftFootSize;
                    break;
                case BodyZone.RightFoot:
                    offset = rightFootOffset;
                    size = rightFootSize;
                    break;
                default:
                    offset = torsoOffset;
                    size = torsoSize;
                    break;
            }

            // Flip offset if sprite is flipped
            float flipMultiplier = GetFlipMultiplier();
            Vector3 worldOffset = new Vector3(offset.x * flipMultiplier, offset.y, 0f);
            Vector3 center = transform.position + worldOffset;

            return new Bounds(center, new Vector3(size.x, size.y, 0.1f));
        }

        private float GetFlipMultiplier()
        {
            if (spriteRenderer != null && spriteRenderer.flipX)
                return -1f;
            return 1f;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;

            // Head - Red
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            DrawHurtboxGizmo(headOffset, headSize);

            // Torso - Yellow
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            DrawHurtboxGizmo(torsoOffset, torsoSize);

            // Feet - Blue
            Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
            DrawHurtboxGizmo(feetOffset, feetSize);
        }

        private void DrawHurtboxGizmo(Vector2 offset, Vector2 size)
        {
            float flipMultiplier = 1f;
            if (Application.isPlaying && spriteRenderer != null && spriteRenderer.flipX)
                flipMultiplier = -1f;

            Vector3 worldOffset = new Vector3(offset.x * flipMultiplier, offset.y, 0f);
            Vector3 center = transform.position + worldOffset;
            Gizmos.DrawCube(center, new Vector3(size.x, size.y, 0.1f));
        }
    }

    /// <summary>
    /// Static helper to get hitbox data for each attack move
    /// </summary>
    public static class AttackData
    {
        /// <summary>
        /// Get the target zone that this attack aims for
        /// </summary>
        public static BodyZone GetTargetZone(string moveName)
        {
            switch (moveName)
            {
                // Head attacks
                case "HighPunch":
                case "HighKick":
                case "RoundHouse":
                case "FlyingKick":
                    return BodyZone.Head;

                // Torso attacks
                case "LowKick":
                case "GroinPunch":
                    return BodyZone.Torso;

                // Shin attack
                case "AnkleKick":
                    return BodyZone.Shin;

                // Feet attack (uses left/right foot check)
                case "CrouchKick":
                    return BodyZone.Feet;

                default:
                    return BodyZone.Torso;
            }
        }

        /// <summary>
        /// Get the hitbox offset and size for an attack (relative to attacker)
        /// </summary>
        public static void GetHitbox(string moveName, bool facingRight, out Vector2 offset, out Vector2 size)
        {
            float dir = facingRight ? 1f : -1f;

            switch (moveName)
            {
                case "HighPunch":
                    offset = new Vector2(1.2f * dir, 1.7f);
                    size = new Vector2(0.8f, 0.5f);
                    break;

                case "HighKick":
                    offset = new Vector2(1.4f * dir, 1.8f);
                    size = new Vector2(1.0f, 0.5f);
                    break;

                case "RoundHouse":
                    offset = new Vector2(1.5f * dir, 1.6f);
                    size = new Vector2(1.2f, 0.6f);
                    break;

                case "FlyingKick":
                    offset = new Vector2(1.8f * dir, 1.5f);
                    size = new Vector2(1.5f, 0.7f);
                    break;

                case "LowKick":
                    offset = new Vector2(1.3f * dir, 1.0f);
                    size = new Vector2(1.0f, 0.6f);
                    break;

                case "GroinPunch":
                    offset = new Vector2(1.0f * dir, 1.0f);
                    size = new Vector2(0.7f, 0.5f);
                    break;

                case "AnkleKick":
                    offset = new Vector2(1.2f * dir, 0.3f);
                    size = new Vector2(1.0f, 0.4f);
                    break;

                case "CrouchKick":
                    offset = new Vector2(1.4f * dir, 0.4f);
                    size = new Vector2(1.2f, 0.5f);
                    break;

                default:
                    offset = new Vector2(1.0f * dir, 1.0f);
                    size = new Vector2(0.8f, 0.5f);
                    break;
            }
        }

        /// <summary>
        /// Get the normalized time range when the attack hitbox is active
        /// </summary>
        public static void GetActiveFrames(string moveName, out float startTime, out float endTime)
        {
            switch (moveName)
            {
                case "HighPunch":
                case "GroinPunch":
                    startTime = 0.3f;
                    endTime = 0.6f;
                    break;

                case "HighKick":
                case "LowKick":
                    startTime = 0.35f;
                    endTime = 0.65f;
                    break;

                case "RoundHouse":
                    startTime = 0.4f;
                    endTime = 0.7f;
                    break;

                case "FlyingKick":
                    startTime = 0.3f;
                    endTime = 0.8f;
                    break;

                case "AnkleKick":
                case "CrouchKick":
                    startTime = 0.3f;
                    endTime = 0.6f;
                    break;

                default:
                    startTime = 0.3f;
                    endTime = 0.6f;
                    break;
            }
        }

        /// <summary>
        /// Check if attack hitbox overlaps with target hurtbox
        /// </summary>
        public static bool CheckHit(Vector3 attackerPos, string moveName, bool facingRight, HitboxSystem target)
        {
            GetHitbox(moveName, facingRight, out Vector2 offset, out Vector2 size);

            Vector3 hitboxCenter = attackerPos + new Vector3(offset.x, offset.y, 0f);
            Bounds hitbox = new Bounds(hitboxCenter, new Vector3(size.x, size.y, 0.1f));

            // CrouchKick checks both left and right foot
            if (moveName == "CrouchKick")
            {
                Bounds leftFoot = target.GetHurtbox(BodyZone.LeftFoot);
                Bounds rightFoot = target.GetHurtbox(BodyZone.RightFoot);
                return hitbox.Intersects(leftFoot) || hitbox.Intersects(rightFoot);
            }

            BodyZone targetZone = GetTargetZone(moveName);
            Bounds hurtbox = target.GetHurtbox(targetZone);

            return hitbox.Intersects(hurtbox);
        }
    }
}
