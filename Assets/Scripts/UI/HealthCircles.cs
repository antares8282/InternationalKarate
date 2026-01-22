using UnityEngine;
using UnityEngine.UI;

namespace InternationalKarate.UI
{
    /// <summary>
    /// Manages the health display as circles that change color based on health points
    /// Matches the original International Karate game style with 4 circles
    /// </summary>
    public class HealthCircles : MonoBehaviour
    {
        [Header("Circle References")]
        [Tooltip("Array of 4 circle images representing health")]
        public Image[] circles = new Image[4];

        [Header("Color Settings")]
        [Tooltip("Color when circle is full (2 points)")]
        public Color fullColor = new Color(1f, 0.8f, 0f, 1f); // Yellow/Gold

        [Tooltip("Color when circle is half (1 point)")]
        public Color halfColor = new Color(1f, 0.4f, 0f, 1f); // Orange

        [Tooltip("Color when circle is empty (0 points)")]
        public Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark gray

        private int maxHealth = 8; // 4 circles x 2 points each
        private int currentHealth = 8;

        private void Start()
        {
            // Initialize all circles to full health
            UpdateHealthDisplay();
        }

        /// <summary>
        /// Set the health value (0-8 for 4 circles with 2 points each)
        /// </summary>
        public void SetHealth(int health)
        {
            currentHealth = Mathf.Clamp(health, 0, maxHealth);
            UpdateHealthDisplay();
        }

        /// <summary>
        /// Set health based on percentage (0-1)
        /// </summary>
        public void SetHealthPercent(float healthPercent)
        {
            int health = Mathf.RoundToInt(healthPercent * maxHealth);
            SetHealth(health);
        }

        /// <summary>
        /// Update the visual display of all circles based on current health
        /// </summary>
        private void UpdateHealthDisplay()
        {
            for (int i = 0; i < circles.Length; i++)
            {
                if (circles[i] == null) continue;

                // Calculate points for this circle (each circle represents 2 points)
                int pointsForThisCircle = currentHealth - (i * 2);

                if (pointsForThisCircle >= 2)
                {
                    // Full circle (2 points)
                    circles[i].color = fullColor;
                }
                else if (pointsForThisCircle == 1)
                {
                    // Half circle (1 point)
                    circles[i].color = halfColor;
                }
                else
                {
                    // Empty circle (0 points)
                    circles[i].color = emptyColor;
                }
            }
        }

        /// <summary>
        /// Take damage (reduce health by specified amount)
        /// </summary>
        public void TakeDamage(int damage)
        {
            SetHealth(currentHealth - damage);
        }

        /// <summary>
        /// Heal (increase health by specified amount)
        /// </summary>
        public void Heal(int amount)
        {
            SetHealth(currentHealth + amount);
        }

        /// <summary>
        /// Reset to full health
        /// </summary>
        public void ResetHealth()
        {
            SetHealth(maxHealth);
        }

        /// <summary>
        /// Get current health value
        /// </summary>
        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        /// <summary>
        /// Get max health value
        /// </summary>
        public int GetMaxHealth()
        {
            return maxHealth;
        }
    }
}
