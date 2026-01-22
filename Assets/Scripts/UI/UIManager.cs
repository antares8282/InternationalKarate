using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InternationalKarate.Managers;

namespace InternationalKarate.UI
{
    /// <summary>
    /// Manages the game UI elements
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Score Display")]
        public SpriteNumberDisplay player1ScoreDisplay;
        public SpriteNumberDisplay player2ScoreDisplay;

        [Header("Health Display")]
        public HealthCircles player1HealthCircles;
        public HealthCircles player2HealthCircles;

        [Header("Labels and Info")]
        public TextMeshProUGUI player1Label;
        public TextMeshProUGUI levelText; // L01, L02, etc.
        public TextMeshProUGUI beltText; // WHITE BELT, BROWN BELT, etc.
        public TextMeshProUGUI timerText;

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
            // Subscribe to match manager events
            if (MatchManager.Instance != null)
            {
                MatchManager.Instance.OnScoreChanged.AddListener(UpdateScore);
                MatchManager.Instance.OnTimerUpdated.AddListener(UpdateTimer);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (MatchManager.Instance != null)
            {
                MatchManager.Instance.OnScoreChanged.RemoveListener(UpdateScore);
                MatchManager.Instance.OnTimerUpdated.RemoveListener(UpdateTimer);
            }
        }

        /// <summary>
        /// Update player score display
        /// </summary>
        public void UpdateScore(int playerNumber, float score)
        {
            if (playerNumber == 1 && player1ScoreDisplay != null)
            {
                player1ScoreDisplay.SetValue(score);
            }
            else if (playerNumber == 2 && player2ScoreDisplay != null)
            {
                player2ScoreDisplay.SetValue(score);
            }
        }

        /// <summary>
        /// Update health circle display
        /// </summary>
        public void UpdateHealth(int playerNumber, float healthPercent)
        {
            if (playerNumber == 1 && player1HealthCircles != null)
            {
                player1HealthCircles.SetHealthPercent(healthPercent);
            }
            else if (playerNumber == 2 && player2HealthCircles != null)
            {
                player2HealthCircles.SetHealthPercent(healthPercent);
            }
        }

        /// <summary>
        /// Update timer display
        /// </summary>
        public void UpdateTimer(float timeRemaining)
        {
            if (timerText != null)
            {
                int seconds = Mathf.CeilToInt(timeRemaining);
                timerText.text = seconds.ToString();
            }
        }

        /// <summary>
        /// Set the belt text (WHITE BELT, BROWN BELT, BLACK BELT, etc.)
        /// This should match the opponent's robe color and difficulty
        /// Displays on two lines like the original game
        /// </summary>
        public void SetBeltLevel(string beltColor)
        {
            if (beltText != null)
            {
                beltText.text = $"{beltColor.ToUpper()} BELT";
            }
        }

        /// <summary>
        /// Set the level number (L01, L02, etc.)
        /// </summary>
        public void SetLevelNumber(string levelNumber)
        {
            if (levelText != null)
            {
                levelText.text = levelNumber.ToUpper();
            }
        }
    }
}
