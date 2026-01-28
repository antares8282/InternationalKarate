using UnityEngine;
using InternationalKarate.Combat;
using UnityInput = UnityEngine.Input;

namespace InternationalKarate.Characters
{
    /// <summary>
    /// Handles keyboard input for a player-controlled fighter
    /// </summary>
    [RequireComponent(typeof(FighterController))]
    public class PlayerInput : MonoBehaviour
    {
        private FighterController fighter;
        private Animator animator;

        [Header("Input Settings")]
        public bool useWASD = true; // Player 1 uses WASD
        public bool useArrowKeys = false; // Player 2 uses Arrow Keys

        private void Awake()
        {
            fighter = GetComponent<FighterController>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!fighter.isPlayerControlled)
                return;

            HandleMovement();
            HandleAttacks();
            HandleJump();
        }

        private void HandleMovement()
        {
            float horizontal = 0f;

            if (useWASD)
            {
                if (UnityInput.GetKey(KeyCode.A)) horizontal = -1f;
                if (UnityInput.GetKey(KeyCode.D)) horizontal = 1f;
            }

            if (useArrowKeys)
            {
                if (UnityInput.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
                if (UnityInput.GetKey(KeyCode.RightArrow)) horizontal = 1f;
            }

            fighter.Move(horizontal);
            animator.SetBool("isWalking", Mathf.Abs(horizontal) > 0.1f);
        }

        private void HandleJump()
        {
            bool jumpPressed = false;

            if (useWASD && UnityInput.GetKeyDown(KeyCode.W))
                jumpPressed = true;

            if (useArrowKeys && UnityInput.GetKeyDown(KeyCode.UpArrow))
                jumpPressed = true;

            if (jumpPressed && !fighter.isExecutingMove)
            {
                animator.SetBool("isJumping", true);
                // Reset jump after a delay
                Invoke(nameof(ResetJump), 0.5f);
            }
        }

        private void ResetJump()
        {
            animator.SetBool("isJumping", false);
        }

        private void HandleAttacks()
        {
            // High Kick - Q or NumPad 7
            if (useWASD && UnityInput.GetKeyDown(KeyCode.Q))
            {
                TriggerAttack(MoveType.HighKick);
            }
            if (useArrowKeys && UnityInput.GetKeyDown(KeyCode.Keypad7))
            {
                TriggerAttack(MoveType.HighKick);
            }

            // Low Kick - E or NumPad 9
            if (useWASD && UnityInput.GetKeyDown(KeyCode.E))
            {
                TriggerAttack(MoveType.LowKick);
            }
            if (useArrowKeys && UnityInput.GetKeyDown(KeyCode.Keypad9))
            {
                TriggerAttack(MoveType.LowKick);
            }

            // High Punch - R or NumPad 8
            if (useWASD && UnityInput.GetKeyDown(KeyCode.R))
            {
                TriggerAttack(MoveType.HighPunch);
            }
            if (useArrowKeys && UnityInput.GetKeyDown(KeyCode.Keypad8))
            {
                TriggerAttack(MoveType.HighPunch);
            }

            // Flying Kick - Space or NumPad 0
            if (useWASD && UnityInput.GetKeyDown(KeyCode.Space))
            {
                TriggerAttack(MoveType.JumpKick);
            }
            if (useArrowKeys && UnityInput.GetKeyDown(KeyCode.Keypad0))
            {
                TriggerAttack(MoveType.JumpKick);
            }
        }

        private void TriggerAttack(MoveType moveType)
        {
            if (!fighter.isExecutingMove)
            {
                fighter.ExecuteMove(moveType);
            }
        }
    }
}
