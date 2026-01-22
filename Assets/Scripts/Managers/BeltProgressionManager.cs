using UnityEngine;
using InternationalKarate.UI;

namespace InternationalKarate.Managers
{
    /// <summary>
    /// Manages belt progression and difficulty levels (15 levels total)
    /// Belt color indicates AI difficulty
    /// </summary>
    public class BeltProgressionManager : MonoBehaviour
    {
        public static BeltProgressionManager Instance { get; private set; }

        [Header("Current Level")]
        [SerializeField] private int currentLevel = 1; // 1-15

        // Belt progression: 15 levels with increasing difficulty
        private readonly string[] beltColors = new string[]
        {
            "WHITE",    // Level 1 - Easiest
            "WHITE",    // Level 2
            "YELLOW",   // Level 3
            "YELLOW",   // Level 4
            "ORANGE",   // Level 5
            "GREEN",    // Level 6
            "GREEN",    // Level 7
            "BLUE",     // Level 8
            "BLUE",     // Level 9
            "PURPLE",   // Level 10
            "BROWN",    // Level 11
            "BROWN",    // Level 12
            "RED",      // Level 13
            "BLACK",    // Level 14
            "BLACK"     // Level 15 - Hardest
        };

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
            UpdateLevelDisplay();
        }

        /// <summary>
        /// Get the belt color for a specific level (1-15)
        /// </summary>
        public string GetBeltColor(int level)
        {
            level = Mathf.Clamp(level, 1, 15);
            return beltColors[level - 1];
        }

        /// <summary>
        /// Get the current belt color
        /// </summary>
        public string GetCurrentBeltColor()
        {
            return GetBeltColor(currentLevel);
        }

        /// <summary>
        /// Get the current level number
        /// </summary>
        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        /// <summary>
        /// Set the current level (1-15) and update UI
        /// </summary>
        public void SetLevel(int level)
        {
            currentLevel = Mathf.Clamp(level, 1, 15);
            UpdateLevelDisplay();
        }

        /// <summary>
        /// Advance to the next level
        /// </summary>
        public void AdvanceLevel()
        {
            if (currentLevel < 15)
            {
                currentLevel++;
                UpdateLevelDisplay();
                Debug.Log($"Advanced to Level {currentLevel} - {GetCurrentBeltColor()} BELT");
            }
            else
            {
                Debug.Log("Already at maximum level (15 - BLACK BELT)");
            }
        }

        /// <summary>
        /// Reset to level 1
        /// </summary>
        public void ResetToLevel1()
        {
            currentLevel = 1;
            UpdateLevelDisplay();
        }

        /// <summary>
        /// Update the HUD display with current level and belt
        /// </summary>
        private void UpdateLevelDisplay()
        {
            if (UIManager.Instance != null)
            {
                // Update level text (L01, L02, etc.)
                string levelText = $"L{currentLevel:D2}";
                UIManager.Instance.SetLevelNumber(levelText);

                // Update belt color text
                string beltColor = GetCurrentBeltColor();
                UIManager.Instance.SetBeltLevel(beltColor);

                Debug.Log($"Level Display Updated: {levelText} - {beltColor} BELT");
            }
        }

        /// <summary>
        /// Get AI difficulty multiplier based on belt level (1.0 to 3.0)
        /// Used to scale AI reaction time, aggression, etc.
        /// </summary>
        public float GetDifficultyMultiplier()
        {
            // Linear scaling from 1.0 (easiest) to 3.0 (hardest)
            return 1.0f + ((currentLevel - 1) / 14.0f) * 2.0f;
        }

        /// <summary>
        /// Check if player has reached the final level
        /// </summary>
        public bool IsMaxLevel()
        {
            return currentLevel >= 15;
        }
    }
}
