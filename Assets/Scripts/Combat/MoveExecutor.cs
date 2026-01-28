using System.Collections;
using UnityEngine;

namespace InternationalKarate.Combat
{
    /// <summary>
    /// Handles the execution of moves including timing and animation
    /// </summary>
    public class MoveExecutor : MonoBehaviour
    {
        [Header("Move Library")]
        public MoveData[] availableMoves;

        [Header("References")]
        private Animator animator;
        private Characters.FighterController fighter;

        [Header("State")]
        private bool isExecuting = false;
        private MoveData currentMoveData;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            fighter = GetComponent<Characters.FighterController>();
        }

        /// <summary>
        /// Execute a specific move
        /// </summary>
        public bool ExecuteMove(MoveType moveType)
        {
            if (isExecuting)
                return false;

            MoveData moveData = GetMoveData(moveType);
            if (moveData == null)
            {
                Debug.LogWarning($"Move data not found for {moveType}");
                return false;
            }

            StartCoroutine(PerformMove(moveData));
            return true;
        }

        private IEnumerator PerformMove(MoveData moveData)
        {
            isExecuting = true;
            fighter.isExecutingMove = true;
            fighter.currentMove = moveData.moveType;
            currentMoveData = moveData;

            // Trigger animation
            if (!string.IsNullOrEmpty(moveData.animationTrigger))
            {
                animator.SetTrigger(moveData.animationTrigger);
            }

            // Play execution sound
            if (moveData.executionSound != null)
            {
                AudioSource.PlayClipAtPoint(moveData.executionSound, transform.position);
            }

            // Move forward if applicable
            if (moveData.canMoveForward)
            {
                float direction = fighter.isFacingRight ? 1f : -1f;
                transform.position += new Vector3(direction * moveData.forwardDistance, 0, 0);
            }

            // Wait for execution time (when hit detection occurs)
            yield return new WaitForSeconds(moveData.executionTime);

            // Perform hit detection
            PerformHitDetection(moveData);

            // Wait for recovery time
            yield return new WaitForSeconds(moveData.recoveryTime);

            // Reset state
            isExecuting = false;
            fighter.isExecutingMove = false;
            fighter.currentMove = MoveType.None;
            currentMoveData = null;
        }

        private void PerformHitDetection(MoveData moveData)
        {
            // Get hitbox position
            Vector2 hitboxCenter = GetHitboxCenter(moveData);

            // Check for hits
            Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter, moveData.hitboxSize, 0f);

            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject)
                    continue;

                Characters.FighterController opponent = hit.GetComponent<Characters.FighterController>();
                if (opponent != null)
                {
                    // Check if opponent is blocking
                    bool isBlocked = CheckIfBlocked(opponent, moveData);

                    if (!isBlocked)
                    {
                        // Register hit
                        OnSuccessfulHit(opponent, moveData);
                    }
                    else
                    {
                        // Hit was blocked
                        OnBlockedHit(opponent, moveData);
                    }
                }
            }
        }

        private Vector2 GetHitboxCenter(MoveData moveData)
        {
            Vector2 center = transform.position;
            Vector2 offset = moveData.hitboxOffset;

            if (!fighter.isFacingRight)
            {
                offset.x *= -1;
            }

            return center + offset;
        }

        private bool CheckIfBlocked(Characters.FighterController opponent, MoveData moveData)
        {
            // Check if opponent is currently blocking
            if (!opponent.isExecutingMove)
                return false;

            // Check if block type matches attack zone
            if (opponent.currentMove == MoveType.BlockHigh && moveData.hitZone == HitZone.High)
                return true;
            if (opponent.currentMove == MoveType.BlockMid && moveData.hitZone == HitZone.Mid)
                return true;
            if (opponent.currentMove == MoveType.BlockLow && moveData.hitZone == HitZone.Low)
                return true;

            return false;
        }

        private void OnSuccessfulHit(Characters.FighterController opponent, MoveData moveData)
        {
            // Play hit sound
            if (moveData.hitSound != null)
            {
                AudioSource.PlayClipAtPoint(moveData.hitSound, transform.position);
            }

            // Notify opponent
            opponent.OnHit(moveData.pointValue);

            // Notify match manager about the hit
            Managers.MatchManager.Instance?.OnFighterHit(fighter.playerNumber, moveData.pointValue);
        }

        private void OnBlockedHit(Characters.FighterController opponent, MoveData moveData)
        {
            // Play block sound or different hit sound
            Debug.Log($"{opponent.fighterName} blocked the attack!");
        }

        private MoveData GetMoveData(MoveType moveType)
        {
            if (availableMoves == null || availableMoves.Length == 0)
                return null;

            foreach (var move in availableMoves)
            {
                if (move != null && move.moveType == moveType)
                    return move;
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            if (currentMoveData != null && isExecuting)
            {
                // Draw hitbox
                Gizmos.color = Color.red;
                Vector2 hitboxCenter = GetHitboxCenter(currentMoveData);
                Gizmos.DrawWireCube(hitboxCenter, currentMoveData.hitboxSize);
            }
        }
    }
}
