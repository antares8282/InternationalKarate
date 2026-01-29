using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InternationalKarate.Audio;
using InternationalKarate.Combat;
using InternationalKarate.Managers;
using UnityInput = UnityEngine.Input;

namespace InternationalKarate.Characters
{
    /// <summary>
    /// Handles keyboard input for a player-controlled fighter.
    /// All combos are SEQUENTIAL: press direction, then Fire within time window.
    /// If no Fire pressed, default move triggers after timeout.
    /// </summary>
    [RequireComponent(typeof(FighterController))]
    [RequireComponent(typeof(HitboxSystem))]
    public class PlayerInput : MonoBehaviour
    {
        private FighterController fighter;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Transform otherPlayer;
        private HitboxSystem hitboxSystem;
        private HitboxSystem opponentHitbox;

        [Header("Input Settings")]
        public bool isPlayer1 = true;
        public float comboWindow = 0.2f; // Time to press Fire after direction

        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float jumpDistance = 9f;
        public float jumpHeight = 1.5f;
        public float flyingKickDistance = 5f;
        public float flyingKickHeight = 4f;
        public float miniJumpHeight = 1.65f;

        [Header("State")]
        private bool isExecutingMove = false;
        private Coroutine currentMoveCoroutine;
        private bool wasWalking = false;
        private int lastWalkFrame = -1;

        // Input buffering for sequential combos
        private float lastUpTime = -1f;
        private float lastDownTime = -1f;
        private float lastForwardTime = -1f;
        private float lastBackTime = -1f;
        private float lastForwardTapTime = -1f; // For double-tap jump
        private float doubleTapWindow = 0.3f;

        private Dictionary<string, float> speedMultipliers = new Dictionary<string, float>()
        {
            {"Idle", 1f},
            {"Walking", 0.105f},
            {"Wait", 0.06f},
            {"Greet", 0.06f},
            {"jump", 0.8f},
            {"MiniJump", 0.35f},
            {"HighPunch", 0.5f},
            {"GroinPunch", 0.5f},
            {"HighKick", 0.4f},
            {"LowKick", 1f},
            {"CrouchKick", 0.425f},
            {"RoundHouse", 0.375f},
            {"FlyingKick", 0.6f},
            {"AnkleKick", 0.2f},
            {"Hurt", 0.095f},
            {"HurtGroin", 0.095f}
        };

        private void Awake()
        {
            fighter = GetComponent<FighterController>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            hitboxSystem = GetComponent<HitboxSystem>();
        }

        private void Start()
        {
            string otherPlayerName = isPlayer1 ? "Player2" : "Player1";
            GameObject other = GameObject.Find(otherPlayerName);
            if (other != null)
            {
                otherPlayer = other.transform;
                opponentHitbox = other.GetComponent<HitboxSystem>();
            }

            if (hitboxSystem != null)
                hitboxSystem.playerNumber = isPlayer1 ? 1 : 2;

            UpdateFacingDirection();
        }

        private void Update()
        {
            if (!fighter.isPlayerControlled)
                return;

            UpdateFacingDirection();

            if (!isExecutingMove)
            {
                HandleMovement();
                HandleAttacks();
                CheckBufferTimeout();
            }
        }

        private void UpdateFacingDirection()
        {
            if (otherPlayer == null) return;

            bool shouldFaceRight = otherPlayer.position.x > transform.position.x;

            if (isPlayer1)
            {
                spriteRenderer.flipX = !shouldFaceRight;
                fighter.isFacingRight = shouldFaceRight;
            }
            else
            {
                spriteRenderer.flipX = shouldFaceRight;
                fighter.isFacingRight = shouldFaceRight;
            }
        }

        // Get key states based on player
        private bool GetUp() => isPlayer1 ? UnityInput.GetKey(KeyCode.W) : UnityInput.GetKey(KeyCode.UpArrow);
        private bool GetDown() => isPlayer1 ? UnityInput.GetKey(KeyCode.S) : UnityInput.GetKey(KeyCode.DownArrow);
        private bool GetBack() => isPlayer1 ? UnityInput.GetKey(KeyCode.A) : UnityInput.GetKey(KeyCode.RightArrow);
        private bool GetForward() => isPlayer1 ? UnityInput.GetKey(KeyCode.D) : UnityInput.GetKey(KeyCode.LeftArrow);

        private bool GetUpDown() => isPlayer1 ? UnityInput.GetKeyDown(KeyCode.W) : UnityInput.GetKeyDown(KeyCode.UpArrow);
        private bool GetDownDown() => isPlayer1 ? UnityInput.GetKeyDown(KeyCode.S) : UnityInput.GetKeyDown(KeyCode.DownArrow);
        private bool GetForwardDown() => isPlayer1 ? UnityInput.GetKeyDown(KeyCode.D) : UnityInput.GetKeyDown(KeyCode.LeftArrow);
        private bool GetBackDown() => isPlayer1 ? UnityInput.GetKeyDown(KeyCode.A) : UnityInput.GetKeyDown(KeyCode.RightArrow);

        private bool GetFire1Down() => isPlayer1 ? UnityInput.GetKeyDown(KeyCode.Y) : UnityInput.GetKeyDown(KeyCode.B);
        private bool GetFire2Down() => isPlayer1 ? UnityInput.GetKeyDown(KeyCode.U) : UnityInput.GetKeyDown(KeyCode.N);

        private void HandleMovement()
        {
            float horizontal = 0f;

            if (GetBack()) horizontal = -1f;
            if (GetForward()) horizontal = 1f;

            bool isWalking = Mathf.Abs(horizontal) > 0.1f;

            if (isWalking)
            {
                animator.speed = speedMultipliers["Walking"];

                if (!wasWalking)
                {
                    animator.Play("Walking", 0, 0f);
                    lastWalkFrame = -1;
                }

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                int totalFrames = 6;
                int currentFrame = Mathf.FloorToInt(stateInfo.normalizedTime * totalFrames) % totalFrames;

                if (currentFrame != lastWalkFrame)
                {
                    float stepDistance = 0.525f;
                    transform.position += new Vector3(horizontal * stepDistance, 0, 0);
                    lastWalkFrame = currentFrame;
                }
            }
            else
            {
                animator.speed = speedMultipliers["Idle"];

                if (wasWalking)
                {
                    animator.Play("Idle", 0, 0f);
                    lastWalkFrame = -1;
                }
            }

            wasWalking = isWalking;
        }

        private void HandleAttacks()
        {
            // Record direction key presses
            if (GetUpDown())
            {
                lastUpTime = Time.time;
            }
            if (GetDownDown())
            {
                lastDownTime = Time.time;
            }
            if (GetForwardDown())
            {
                // Check for jump: W then D (sequential)
                if (lastUpTime > 0 && Time.time - lastUpTime < comboWindow)
                {
                    // Jump: W then D
                    PlayMove("jump");
                    ClearBuffers();
                    return;
                }
                lastForwardTapTime = Time.time;
                lastForwardTime = Time.time;
            }
            if (GetBackDown())
            {
                lastBackTime = Time.time;
            }

            // Check Fire1 (Y for P1, B for P2)
            if (GetFire1Down())
            {
                float now = Time.time;

                // W then Fire1 = HighPunch
                if (lastUpTime > 0 && now - lastUpTime < comboWindow)
                {
                    PlayMove("HighPunch");
                    ClearBuffers();
                    return;
                }
                // S+D then Fire1 = GroinPunch
                if (lastDownTime > 0 && lastForwardTime > 0 &&
                    now - lastDownTime < comboWindow && now - lastForwardTime < comboWindow)
                {
                    PlayMove("GroinPunch");
                    ClearBuffers();
                    return;
                }
                // Fire1 alone = HighPunch
                PlayMove("HighPunch");
                ClearBuffers();
                return;
            }

            // Check Fire2 (U for P1, N for P2)
            if (GetFire2Down())
            {
                float now = Time.time;

                // W+D then Fire2 = FlyingKick
                if (lastUpTime > 0 && lastForwardTime > 0 &&
                    now - lastUpTime < comboWindow && now - lastForwardTime < comboWindow)
                {
                    PlayMove("FlyingKick");
                    ClearBuffers();
                    return;
                }
                // W then Fire2 = HighKick
                if (lastUpTime > 0 && now - lastUpTime < comboWindow)
                {
                    PlayMove("HighKick");
                    ClearBuffers();
                    return;
                }
                // S then Fire2 = CrouchKick
                if (lastDownTime > 0 && now - lastDownTime < comboWindow)
                {
                    PlayMove("CrouchKick");
                    ClearBuffers();
                    return;
                }
                // D then Fire2 = LowKick
                if (lastForwardTime > 0 && now - lastForwardTime < comboWindow)
                {
                    PlayMove("LowKick");
                    ClearBuffers();
                    return;
                }
                // A then Fire2 = RoundHouse
                if (lastBackTime > 0 && now - lastBackTime < comboWindow)
                {
                    PlayMove("RoundHouse");
                    ClearBuffers();
                    return;
                }
                // Fire2 alone = LowKick
                PlayMove("LowKick");
                ClearBuffers();
                return;
            }
        }

        private void CheckBufferTimeout()
        {
            float now = Time.time;

            // W timed out without Fire = MiniJump
            if (lastUpTime > 0 && now - lastUpTime >= comboWindow)
            {
                // Only if not combined with forward (that's for jump)
                if (lastForwardTime < 0 || now - lastForwardTime >= comboWindow)
                {
                    PlayMove("MiniJump");
                    ClearBuffers();
                    return;
                }
            }

            // S timed out without Fire = AnkleKick
            if (lastDownTime > 0 && now - lastDownTime >= comboWindow)
            {
                PlayMove("AnkleKick");
                ClearBuffers();
                return;
            }

            // Clear stale forward/back buffers (they don't have default moves)
            if (lastForwardTime > 0 && now - lastForwardTime >= comboWindow)
            {
                lastForwardTime = -1f;
            }
            if (lastBackTime > 0 && now - lastBackTime >= comboWindow)
            {
                lastBackTime = -1f;
            }
        }

        private void ClearBuffers()
        {
            lastUpTime = -1f;
            lastDownTime = -1f;
            lastForwardTime = -1f;
            lastBackTime = -1f;
            lastForwardTapTime = -1f;
        }

        private void PlayMove(string animName)
        {
            if (isExecutingMove) return;

            if (currentMoveCoroutine != null)
                StopCoroutine(currentMoveCoroutine);

            currentMoveCoroutine = StartCoroutine(ExecuteMove(animName));
        }

        private IEnumerator ExecuteMove(string animName)
        {
            isExecutingMove = true;
            fighter.isExecutingMove = true;

            float speed = speedMultipliers.ContainsKey(animName) ? speedMultipliers[animName] : 1f;
            animator.speed = speed;
            animator.Play(animName, 0, 0f);

            Vector3 startPos = transform.position;
            bool needsHorizontalMove = (animName == "jump" || animName == "FlyingKick");
            bool needsVerticalMove = (animName == "MiniJump");
            float moveDistance = animName == "FlyingKick" ? flyingKickDistance : jumpDistance;

            float direction = fighter.isFacingRight ? 1f : -1f;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayAttackSound(animName);

            // Get hitbox active frames for this attack
            bool isAttack = IsAttackMove(animName);
            AttackData.GetActiveFrames(animName, out float hitStartTime, out float hitEndTime);
            bool hasHit = false;

            yield return null;
            yield return null;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float timeout = 5f;
            float elapsed = 0f;

            while (stateInfo.normalizedTime < 0.95f && elapsed < timeout)
            {
                float progress = Mathf.Clamp01(stateInfo.normalizedTime);

                if (needsHorizontalMove)
                {
                    float height = (animName == "jump") ? jumpHeight : flyingKickHeight;
                    float verticalArc = Mathf.Sin(progress * Mathf.PI) * height;
                    transform.position = startPos + new Vector3(moveDistance * progress * direction, verticalArc, 0f);
                }

                if (needsVerticalMove)
                {
                    float verticalProgress = Mathf.Sin(progress * Mathf.PI);
                    transform.position = startPos + new Vector3(0f, miniJumpHeight * verticalProgress, 0f);
                }

                // Check for hit during active frames
                if (isAttack && !hasHit && opponentHitbox != null)
                {
                    if (progress >= hitStartTime && progress <= hitEndTime)
                    {
                        if (AttackData.CheckHit(transform.position, animName, fighter.isFacingRight, opponentHitbox))
                        {
                            hasHit = true;
                            OnHitOpponent(animName);
                        }
                    }
                }

                yield return null;
                elapsed += Time.deltaTime;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            if (needsHorizontalMove)
                transform.position = startPos + new Vector3(moveDistance * direction, 0f, 0f);

            if (needsVerticalMove)
                transform.position = startPos;

            // Return to Idle
            animator.speed = 1f;
            animator.Play("Idle", 0, 0f);

            isExecutingMove = false;
            fighter.isExecutingMove = false;
        }

        private bool IsAttackMove(string animName)
        {
            return animName == "HighPunch" || animName == "HighKick" ||
                   animName == "LowKick" || animName == "CrouchKick" ||
                   animName == "RoundHouse" || animName == "FlyingKick" ||
                   animName == "AnkleKick" || animName == "GroinPunch";
        }

        private void OnHitOpponent(string moveName)
        {
            int playerNum = isPlayer1 ? 1 : 2;
            Debug.Log($"Player {playerNum} hit with {moveName}!");

            // Notify MatchManager
            if (MatchManager.Instance != null)
            {
                MatchManager.Instance.OnFighterHit(playerNum, moveName);
            }

            // Play opponent hurt animation
            if (otherPlayer != null)
            {
                var opponentAnimator = otherPlayer.GetComponent<Animator>();
                var opponentInput = otherPlayer.GetComponent<PlayerInput>();

                if (opponentAnimator != null && opponentInput != null)
                {
                    // Stop opponent's current action
                    opponentInput.InterruptMove();

                    // Play appropriate hurt animation
                    // HurtGroin only for GroinPunch and LowKick, Hurt for everything else
                    string hurtAnim = (moveName == "GroinPunch" || moveName == "LowKick") ? "HurtGroin" : "Hurt";

                    opponentAnimator.speed = speedMultipliers.ContainsKey(hurtAnim) ? speedMultipliers[hurtAnim] : 0.126f;
                    opponentAnimator.Play(hurtAnim, 0, 0f);
                }
            }
        }

        public void InterruptMove()
        {
            if (currentMoveCoroutine != null)
            {
                StopCoroutine(currentMoveCoroutine);
                currentMoveCoroutine = null;
            }
            isExecutingMove = false;
            fighter.isExecutingMove = false;
        }
    }
}
