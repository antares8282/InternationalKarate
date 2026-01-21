using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using InternationalKarate.Combat;
using InternationalKarate.Characters;

namespace InternationalKarate.Managers
{
    /// <summary>
    /// Manages the match flow, scoring, and round progression
    /// </summary>
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance { get; private set; }

        [Header("Match Settings")]
        public float roundDuration = 60f; // seconds
        public int pointsToWin = 2; // Full points needed to win
        public float resetDelay = 2f; // Delay after hit before reset

        [Header("Fighter References")]
        public FighterController fighter1;
        public FighterController fighter2;

        [Header("Background Manager")]
        public BackgroundManager backgroundManager;

        [Header("Match State")]
        public bool isMatchActive = false;
        public bool isRoundActive = false;
        private float currentRoundTime;

        [Header("Score Tracking")]
        private float player1Score = 0f; // Using float to handle half-points (0.5, 1.0, 1.5, 2.0)
        private float player2Score = 0f;

        [Header("Events")]
        public UnityEvent<int, float> OnScoreChanged; // playerNumber, newScore
        public UnityEvent<int> OnRoundWon; // playerNumber
        public UnityEvent<int> OnMatchWon; // playerNumber
        public UnityEvent<float> OnTimerUpdated; // timeRemaining
        public UnityEvent OnRoundStart;
        public UnityEvent OnRoundEnd;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartMatch();
        }

        private void Update()
        {
            if (isRoundActive)
            {
                UpdateRoundTimer();
            }
        }

        public void StartMatch()
        {
            isMatchActive = true;
            player1Score = 0f;
            player2Score = 0f;

            OnScoreChanged?.Invoke(1, player1Score);
            OnScoreChanged?.Invoke(2, player2Score);

            // Randomize background at start of match
            if (backgroundManager != null)
            {
                backgroundManager.SetRandomBackground();
            }

            StartNewRound();
        }

        public void StartNewRound()
        {
            currentRoundTime = roundDuration;
            isRoundActive = true;

            // Reset fighters to start positions
            fighter1?.ResetToStartPosition();
            fighter2?.ResetToStartPosition();

            // Ensure fighters face each other
            fighter1?.SetFacingDirection(true);
            fighter2?.SetFacingDirection(false);

            OnRoundStart?.Invoke();
            Debug.Log("Round started!");
        }

        private void UpdateRoundTimer()
        {
            currentRoundTime -= Time.deltaTime;
            OnTimerUpdated?.Invoke(currentRoundTime);

            if (currentRoundTime <= 0f)
            {
                EndRound(0); // Time out - no winner
            }
        }

        /// <summary>
        /// Called when a fighter successfully hits the opponent
        /// </summary>
        public void OnFighterHit(int attackingPlayerNumber, PointValue pointValue)
        {
            if (!isRoundActive)
                return;

            float points = pointValue == PointValue.Half ? 0.5f : (pointValue == PointValue.Full ? 1f : 0f);

            if (attackingPlayerNumber == 1)
            {
                player1Score += points;
                OnScoreChanged?.Invoke(1, player1Score);
                Debug.Log($"Player 1 scored {points} points! Total: {player1Score}");
            }
            else if (attackingPlayerNumber == 2)
            {
                player2Score += points;
                OnScoreChanged?.Invoke(2, player2Score);
                Debug.Log($"Player 2 scored {points} points! Total: {player2Score}");
            }

            // Check for round winner
            if (player1Score >= pointsToWin)
            {
                StartCoroutine(HandleHitAndReset(1));
            }
            else if (player2Score >= pointsToWin)
            {
                StartCoroutine(HandleHitAndReset(2));
            }
            else
            {
                // Continue round after brief pause
                StartCoroutine(HandleHitAndReset(0));
            }
        }

        private IEnumerator HandleHitAndReset(int winningPlayer)
        {
            isRoundActive = false;

            // Wait for hit animation to play
            yield return new WaitForSeconds(resetDelay);

            if (winningPlayer > 0)
            {
                EndRound(winningPlayer);
            }
            else
            {
                // Reset positions and continue
                fighter1?.ResetToStartPosition();
                fighter2?.ResetToStartPosition();
                isRoundActive = true;
            }
        }

        private void EndRound(int winningPlayer)
        {
            isRoundActive = false;
            OnRoundEnd?.Invoke();

            if (winningPlayer == 1)
            {
                Debug.Log("Player 1 wins the round!");
                OnRoundWon?.Invoke(1);
                OnMatchWon?.Invoke(1);
            }
            else if (winningPlayer == 2)
            {
                Debug.Log("Player 2 wins the round!");
                OnRoundWon?.Invoke(2);
                OnMatchWon?.Invoke(2);
            }
            else
            {
                Debug.Log("Round ended - Time out!");
                // Could determine winner by current score
                if (player1Score > player2Score)
                {
                    OnRoundWon?.Invoke(1);
                    OnMatchWon?.Invoke(1);
                }
                else if (player2Score > player1Score)
                {
                    OnRoundWon?.Invoke(2);
                    OnMatchWon?.Invoke(2);
                }
                else
                {
                    Debug.Log("Draw!");
                }
            }
        }

        public void RestartMatch()
        {
            StartMatch();
        }

        public float GetPlayer1Score() => player1Score;
        public float GetPlayer2Score() => player2Score;
        public float GetRoundTimeRemaining() => currentRoundTime;
    }
}
