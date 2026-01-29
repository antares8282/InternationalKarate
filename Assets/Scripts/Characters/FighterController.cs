using UnityEngine;
using InternationalKarate.Combat;

namespace InternationalKarate.Characters
{
    /// <summary>
    /// Main controller for fighter characters
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class FighterController : MonoBehaviour
    {
        [Header("Fighter Info")]
        public string fighterName;
        public bool isPlayerControlled = true;
        public int playerNumber = 1; // 1 or 2

        [Header("Components")]
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private MoveExecutor moveExecutor;

        [Header("State")]
        public bool isFacingRight = true;
        public bool isExecutingMove = false;
        public MoveType currentMove = MoveType.None;

        [Header("Position")]
        public Vector3 startPosition;
        public float moveSpeed = 3f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            moveExecutor = GetComponent<MoveExecutor>();

            if (moveExecutor == null)
            {
                moveExecutor = gameObject.AddComponent<MoveExecutor>();
            }

            startPosition = transform.position;
        }

        private void Start()
        {
            SetFacingDirection(isFacingRight);
        }

        /// <summary>
        /// Execute a specific move
        /// </summary>
        public bool ExecuteMove(MoveType move)
        {
            if (isExecutingMove)
                return false;

            return moveExecutor.ExecuteMove(move);
        }

        /// <summary>
        /// Set the facing direction of the fighter
        /// </summary>
        public void SetFacingDirection(bool facingRight)
        {
            isFacingRight = facingRight;
            spriteRenderer.flipX = !facingRight;
        }

        /// <summary>
        /// Turn around to face the opposite direction
        /// </summary>
        public void TurnAround()
        {
            SetFacingDirection(!isFacingRight);
        }

        /// <summary>
        /// Move fighter horizontally
        /// </summary>
        public void Move(float direction)
        {
            if (isExecutingMove)
                return;

            float moveDir = isFacingRight ? direction : -direction;
            transform.position += new Vector3(moveDir * moveSpeed * Time.deltaTime, 0, 0);

            animator.SetBool("isWalking", Mathf.Abs(direction) > 0.1f);
        }

        /// <summary>
        /// Reset fighter to starting position
        /// </summary>
        public void ResetToStartPosition()
        {
            transform.position = startPosition;
            isExecutingMove = false;
            currentMove = MoveType.None;
            animator.Play("Idle", 0, 0f);
        }

        /// <summary>
        /// Called when this fighter is hit
        /// </summary>
        public void OnHit(PointValue points)
        {
            animator.Play("Hurt", 0, 0f);
            isExecutingMove = false;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw starting position
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPosition, 0.2f);
        }
    }
}
