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
        public TextMeshProUGUI player1ScoreText;
        public TextMeshProUGUI player2ScoreText;

        [Header("Health Bars")]
        public Image player1HealthBar;
        public Image player2HealthBar;

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
            if (playerNumber == 1 && player1ScoreText != null)
            {
                player1ScoreText.text = score.ToString("F1");
            }
            else if (playerNumber == 2 && player2ScoreText != null)
            {
                player2ScoreText.text = score.ToString("F1");
            }
        }

        /// <summary>
        /// Update health bar fill amount
        /// </summary>
        public void UpdateHealth(int playerNumber, float healthPercent)
        {
            if (playerNumber == 1 && player1HealthBar != null)
            {
                player1HealthBar.fillAmount = healthPercent;
            }
            else if (playerNumber == 2 && player2HealthBar != null)
            {
                player2HealthBar.fillAmount = healthPercent;
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
                beltText.text = $"{beltColor.ToUpper()}\nBELT";
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
