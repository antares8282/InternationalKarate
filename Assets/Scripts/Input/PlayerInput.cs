using UnityEngine;
using InternationalKarate.Combat;
using InternationalKarate.Characters;

namespace InternationalKarate.Input
{
    /// <summary>
    /// Handles player input for controlling fighters
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        [Header("Player Configuration")]
        public int playerNumber = 1; // 1 or 2
        public bool useKeyboard = true;

        [Header("Keyboard Controls - Player 1")]
        [SerializeField] private KeyCode p1MoveLeft = KeyCode.A;
        [SerializeField] private KeyCode p1MoveRight = KeyCode.D;
        [SerializeField] private KeyCode p1HighPunch = KeyCode.W;
        [SerializeField] private KeyCode p1MidPunch = KeyCode.S;
        [SerializeField] private KeyCode p1LowPunch = KeyCode.X;
        [SerializeField] private KeyCode p1HighKick = KeyCode.E;
        [SerializeField] private KeyCode p1MidKick = KeyCode.Q;
        [SerializeField] private KeyCode p1LowKick = KeyCode.Z;
        [SerializeField] private KeyCode p1Block = KeyCode.LeftShift;
        [SerializeField] private KeyCode p1TurnAround = KeyCode.C;

        [Header("Keyboard Controls - Player 2")]
        [SerializeField] private KeyCode p2MoveLeft = KeyCode.LeftArrow;
        [SerializeField] private KeyCode p2MoveRight = KeyCode.RightArrow;
        [SerializeField] private KeyCode p2HighPunch = KeyCode.UpArrow;
        [SerializeField] private KeyCode p2MidPunch = KeyCode.DownArrow;
        [SerializeField] private KeyCode p2LowPunch = KeyCode.Period;
        [SerializeField] private KeyCode p2HighKick = KeyCode.RightControl;
        [SerializeField] private KeyCode p2MidKick = KeyCode.RightShift;
        [SerializeField] private KeyCode p2LowKick = KeyCode.Comma;
        [SerializeField] private KeyCode p2Block = KeyCode.Slash;
        [SerializeField] private KeyCode p2TurnAround = KeyCode.M;

        private FighterController fighter;

        private void Awake()
        {
            fighter = GetComponent<FighterController>();
        }

        private void Update()
        {
            if (!useKeyboard || fighter == null)
                return;

            HandleMovement();
            HandleAttacks();
        }

        private void HandleMovement()
        {
            float moveDirection = 0f;

            if (playerNumber == 1)
            {
                if (UnityEngine.Input.GetKey(p1MoveLeft))
                    moveDirection = -1f;
                else if (UnityEngine.Input.GetKey(p1MoveRight))
                    moveDirection = 1f;

                if (UnityEngine.Input.GetKeyDown(p1TurnAround))
                    fighter.TurnAround();
            }
            else if (playerNumber == 2)
            {
                if (UnityEngine.Input.GetKey(p2MoveLeft))
                    moveDirection = -1f;
                else if (UnityEngine.Input.GetKey(p2MoveRight))
                    moveDirection = 1f;

                if (UnityEngine.Input.GetKeyDown(p2TurnAround))
                    fighter.TurnAround();
            }

            if (Mathf.Abs(moveDirection) > 0.01f)
            {
                fighter.Move(moveDirection);
            }
        }

        private void HandleAttacks()
        {
            if (playerNumber == 1)
            {
                if (UnityEngine.Input.GetKeyDown(p1HighPunch))
                    fighter.ExecuteMove(MoveType.HighPunch);
                else if (UnityEngine.Input.GetKeyDown(p1MidPunch))
                    fighter.ExecuteMove(MoveType.MidPunch);
                else if (UnityEngine.Input.GetKeyDown(p1LowPunch))
                    fighter.ExecuteMove(MoveType.LowPunch);
                else if (UnityEngine.Input.GetKeyDown(p1HighKick))
                    fighter.ExecuteMove(MoveType.HighKick);
                else if (UnityEngine.Input.GetKeyDown(p1MidKick))
                    fighter.ExecuteMove(MoveType.MidKick);
                else if (UnityEngine.Input.GetKeyDown(p1LowKick))
                    fighter.ExecuteMove(MoveType.LowKick);
                else if (UnityEngine.Input.GetKeyDown(p1Block))
                    fighter.ExecuteMove(MoveType.BlockMid);
            }
            else if (playerNumber == 2)
            {
                if (UnityEngine.Input.GetKeyDown(p2HighPunch))
                    fighter.ExecuteMove(MoveType.HighPunch);
                else if (UnityEngine.Input.GetKeyDown(p2MidPunch))
                    fighter.ExecuteMove(MoveType.MidPunch);
                else if (UnityEngine.Input.GetKeyDown(p2LowPunch))
                    fighter.ExecuteMove(MoveType.LowPunch);
                else if (UnityEngine.Input.GetKeyDown(p2HighKick))
                    fighter.ExecuteMove(MoveType.HighKick);
                else if (UnityEngine.Input.GetKeyDown(p2MidKick))
                    fighter.ExecuteMove(MoveType.MidKick);
                else if (UnityEngine.Input.GetKeyDown(p2LowKick))
                    fighter.ExecuteMove(MoveType.LowKick);
                else if (UnityEngine.Input.GetKeyDown(p2Block))
                    fighter.ExecuteMove(MoveType.BlockMid);
            }
        }
    }
}
