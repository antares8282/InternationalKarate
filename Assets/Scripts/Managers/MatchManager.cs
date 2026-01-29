using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using InternationalKarate.Combat;
using InternationalKarate.Characters;
using InternationalKarate.Gameplay;
using InternationalKarate.UI;

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
        public float resetDelay = 2f; // Delay after hit before reset
        public float greetDuration = 1.0f; // Duration of greet animation

        [Header("Point Values")]
        public const int FULL_POINT_SCORE = 800;
        public const int HALF_POINT_SCORE = 400;

        [Header("Fighter References")]
        public FighterController fighter1;
        public FighterController fighter2;

        [Header("Health UI References")]
        public HealthCircles player1Health;
        public HealthCircles player2Health;

        [Header("Background Manager")]
        public BackgroundManager backgroundManager;

        [Header("Sensei Reference")]
        public SenseiManager senseiManager;

        [Header("Match State")]
        public bool isMatchActive = false;
        public bool isRoundActive = false;
        private float currentRoundTime;

        [Header("Score Tracking")]
        private int player1Score = 0;
        private int player2Score = 0;

        [Header("Events")]
        public UnityEvent<int, int> OnScoreChanged; // playerNumber, newScore
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
            // Find SenseiManager if not assigned
            if (senseiManager == null)
                senseiManager = SenseiManager.Instance;

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
            player1Score = 0;
            player2Score = 0;

            OnScoreChanged?.Invoke(1, player1Score);
            OnScoreChanged?.Invoke(2, player2Score);

            // Reset health
            if (player1Health != null) player1Health.ResetHealth();
            if (player2Health != null) player2Health.ResetHealth();

            // Randomize background at start of match
            if (backgroundManager != null)
            {
                backgroundManager.SetRandomBackground();
            }

            StartCoroutine(MatchIntroSequence());
        }

        private IEnumerator MatchIntroSequence()
        {
            // Reset fighters to start positions
            fighter1?.ResetToStartPosition();
            fighter2?.ResetToStartPosition();

            // Ensure fighters face each other
            fighter1?.SetFacingDirection(true);
            fighter2?.SetFacingDirection(false);

            // Disable player control during intro
            SetPlayersControlled(false);

            // Play greet animation on both fighters
            if (fighter1 != null)
            {
                var animator1 = fighter1.GetComponent<Animator>();
                if (animator1 != null)
                {
                    animator1.speed = 1f;
                    animator1.Play("Greet", 0, 0f);
                }
            }
            if (fighter2 != null)
            {
                var animator2 = fighter2.GetComponent<Animator>();
                var spriteRenderer2 = fighter2.GetComponent<SpriteRenderer>();
                if (animator2 != null)
                {
                    animator2.speed = 1f;
                    animator2.Play("Greet", 0, 0f);
                }
                // Player2 needs flipX=false to face left (toward Player1)
                if (spriteRenderer2 != null)
                {
                    spriteRenderer2.flipX = false;
                }
            }

            // Show "BEGIN" message
            if (senseiManager != null)
                senseiManager.ShowBegin();

            // Wait for greet animation
            yield return new WaitForSeconds(greetDuration);

            // Hide message
            if (senseiManager != null)
                senseiManager.HideMessage();

            // Reset animators to idle
            if (fighter1 != null)
            {
                var animator1 = fighter1.GetComponent<Animator>();
                if (animator1 != null)
                {
                    animator1.speed = 1f;
                    animator1.Play("Idle", 0, 0f);
                }
            }
            if (fighter2 != null)
            {
                var animator2 = fighter2.GetComponent<Animator>();
                if (animator2 != null)
                {
                    animator2.speed = 1f;
                    animator2.Play("Idle", 0, 0f);
                }
            }

            // Enable player control
            SetPlayersControlled(true);

            StartNewRound();
        }

        private void SetPlayersControlled(bool controlled)
        {
            if (fighter1 != null) fighter1.isPlayerControlled = controlled;
            if (fighter2 != null) fighter2.isPlayerControlled = controlled;
        }

        public void StartNewRound()
        {
            currentRoundTime = roundDuration;
            isRoundActive = true;

            OnRoundStart?.Invoke();
            Debug.Log("Round started!");
        }

        private void UpdateRoundTimer()
        {
            currentRoundTime -= Time.deltaTime;
            OnTimerUpdated?.Invoke(currentRoundTime);

            if (currentRoundTime <= 0f)
            {
                StartCoroutine(HandleMatchEnd());
            }
        }

        /// <summary>
        /// Called when a fighter successfully hits the opponent
        /// </summary>
        public void OnFighterHit(int attackingPlayerNumber, string moveName)
        {
            if (!isRoundActive)
                return;

            // Pause the round during hit sequence
            isRoundActive = false;

            // Determine point value based on move name
            bool isFullPoint = IsFullPointMove(moveName);
            int points = isFullPoint ? FULL_POINT_SCORE : HALF_POINT_SCORE;

            // Update score
            if (attackingPlayerNumber == 1)
            {
                player1Score += points;
                OnScoreChanged?.Invoke(1, player1Score);

                // Reduce opponent health (full point = 2, half point = 1)
                if (player2Health != null)
                    player2Health.TakeDamage(isFullPoint ? 2 : 1);

                Debug.Log($"Player 1 scored {points} points! Total: {player1Score}");
            }
            else if (attackingPlayerNumber == 2)
            {
                player2Score += points;
                OnScoreChanged?.Invoke(2, player2Score);

                // Reduce opponent health
                if (player1Health != null)
                    player1Health.TakeDamage(isFullPoint ? 2 : 1);

                Debug.Log($"Player 2 scored {points} points! Total: {player2Score}");
            }

            // Check for KO (health = 0)
            bool isKO = (player1Health != null && player1Health.GetCurrentHealth() <= 0) ||
                        (player2Health != null && player2Health.GetCurrentHealth() <= 0);

            // Start hit sequence (freeze, show bubbles, restart)
            StartCoroutine(HandleHitSequence(isFullPoint, points, isKO));
        }

        private bool IsFullPointMove(string moveName)
        {
            // Full point moves: HighPunch, HighKick, RoundHouse, FlyingKick, GroinPunch
            return moveName == "HighPunch" ||
                   moveName == "HighKick" ||
                   moveName == "RoundHouse" ||
                   moveName == "FlyingKick" ||
                   moveName == "GroinPunch";
        }

        private IEnumerator HandleHitSequence(bool isFullPoint, int points, bool isKO)
        {
            // Disable player control
            SetPlayersControlled(false);

            // Wait for hurt animation to finish
            yield return new WaitForSeconds(1.5f);

            // Freeze both fighters on last frame
            if (fighter1 != null)
            {
                var animator1 = fighter1.GetComponent<Animator>();
                if (animator1 != null) animator1.speed = 0f;
            }
            if (fighter2 != null)
            {
                var animator2 = fighter2.GetComponent<Animator>();
                if (animator2 != null) animator2.speed = 0f;
            }

            // Show point bubbles with longer display time
            if (senseiManager != null)
            {
                if (isFullPoint)
                    senseiManager.ShowFullPoint();
                else
                    senseiManager.ShowHalfPoint();

                yield return new WaitForSeconds(1.5f);

                senseiManager.ShowScore(points);

                yield return new WaitForSeconds(1.5f);

                senseiManager.HideMessage();
            }

            yield return new WaitForSeconds(0.5f);

            // Check if match is over
            if (isKO)
            {
                StartCoroutine(HandleMatchEnd());
            }
            else
            {
                // Restart round
                RestartRound();
            }
        }

        private void RestartRound()
        {
            // Reset fighters to start positions
            fighter1?.ResetToStartPosition();
            fighter2?.ResetToStartPosition();

            // Reset animator speeds
            if (fighter1 != null)
            {
                var animator1 = fighter1.GetComponent<Animator>();
                if (animator1 != null) animator1.speed = 1f;
            }
            if (fighter2 != null)
            {
                var animator2 = fighter2.GetComponent<Animator>();
                if (animator2 != null) animator2.speed = 1f;
            }

            // Re-enable controls and resume round
            SetPlayersControlled(true);
            isRoundActive = true;
        }

        private IEnumerator HandleMatchEnd()
        {
            isRoundActive = false;
            SetPlayersControlled(false);

            OnRoundEnd?.Invoke();

            // Show "MATCH OVER"
            if (senseiManager != null)
                senseiManager.ShowMatchOver();

            yield return new WaitForSeconds(2f);

            // Determine winner based on health
            int p1Health = player1Health != null ? player1Health.GetCurrentHealth() : 0;
            int p2Health = player2Health != null ? player2Health.GetCurrentHealth() : 0;

            if (p1Health > p2Health)
            {
                // Player 1 wins
                if (senseiManager != null)
                    senseiManager.ShowYouWin();
                OnMatchWon?.Invoke(1);
                Debug.Log("Player 1 wins!");
            }
            else if (p2Health > p1Health)
            {
                // Player 2 wins
                if (senseiManager != null)
                    senseiManager.ShowYouLose();
                OnMatchWon?.Invoke(2);
                Debug.Log("Player 2 wins!");
            }
            else
            {
                // Draw
                if (senseiManager != null)
                    senseiManager.ShowDraw();
                Debug.Log("Draw!");
            }

            isMatchActive = false;
        }

        public void RestartMatch()
        {
            StopAllCoroutines();
            StartMatch();
        }

        public int GetPlayer1Score() => player1Score;
        public int GetPlayer2Score() => player2Score;
        public float GetRoundTimeRemaining() => currentRoundTime;
    }
}
