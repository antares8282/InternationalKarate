using UnityEngine;
using System.Collections.Generic;

namespace InternationalKarate.UI
{
    /// <summary>
    /// Displays numbers using individual digit sprites
    /// Matches the original International Karate pixel art style
    /// </summary>
    public class SpriteNumberDisplay : MonoBehaviour
    {
        [Header("Digit Sprites")]
        [Tooltip("Array of digit sprites (0-9) in order")]
        public Sprite[] digitSprites = new Sprite[10];

        [Header("Display Settings")]
        [Tooltip("Spacing between digits in pixels")]
        public float digitSpacing = 2f;

        [Tooltip("Scale of the digit sprites")]
        public float digitScale = 1f;

        [Tooltip("Alignment: Left, Center, or Right")]
        public TextAlignment alignment = TextAlignment.Left;

        [Header("Current Value")]
        [SerializeField] private float currentValue = 0f;

        private List<GameObject> digitObjects = new List<GameObject>();

        public enum TextAlignment
        {
            Left,
            Center,
            Right
        }

        private void Start()
        {
            // Display initial value
            UpdateDisplay(currentValue);
        }

        /// <summary>
        /// Set the displayed number value
        /// </summary>
        public void SetValue(float value)
        {
            currentValue = value;
            UpdateDisplay(value);
        }

        /// <summary>
        /// Get the current displayed value
        /// </summary>
        public float GetValue()
        {
            return currentValue;
        }

        /// <summary>
        /// Update the visual display of the number
        /// </summary>
        private void UpdateDisplay(float value)
        {
            // Clear existing digit objects
            ClearDigits();

            // Format the number (remove decimal if it's .0)
            string numberString;
            if (value % 1 == 0)
            {
                // Whole number - no decimal
                numberString = ((int)value).ToString();
            }
            else
            {
                // Has decimal - show one decimal place
                numberString = value.ToString("F1");
            }

            // Create digit sprites
            float totalWidth = CalculateTotalWidth(numberString);
            float startX = CalculateStartX(totalWidth);
            float currentX = startX;

            for (int i = 0; i < numberString.Length; i++)
            {
                char c = numberString[i];

                if (c == '.')
                {
                    // Handle decimal point (you can add a decimal sprite if you have one)
                    // For now, just add spacing
                    currentX += digitSpacing * 0.5f;
                }
                else if (char.IsDigit(c))
                {
                    int digit = c - '0'; // Convert char to int

                    if (digit >= 0 && digit < digitSprites.Length && digitSprites[digit] != null)
                    {
                        GameObject digitObj = CreateDigitObject(digitSprites[digit], currentX);
                        digitObjects.Add(digitObj);

                        // Move to next position
                        float spriteWidth = digitSprites[digit].bounds.size.x * digitScale;
                        currentX += spriteWidth + digitSpacing;
                    }
                }
            }
        }

        /// <summary>
        /// Create a GameObject for a single digit sprite
        /// </summary>
        private GameObject CreateDigitObject(Sprite sprite, float xPosition)
        {
            GameObject digitObj = new GameObject("Digit");
            digitObj.transform.SetParent(transform);
            digitObj.transform.localPosition = new Vector3(xPosition, 0, 0);
            digitObj.transform.localScale = Vector3.one * digitScale;

            SpriteRenderer renderer = digitObj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 100; // Render on top

            return digitObj;
        }

        /// <summary>
        /// Calculate total width of the number display
        /// </summary>
        private float CalculateTotalWidth(string numberString)
        {
            float width = 0f;

            for (int i = 0; i < numberString.Length; i++)
            {
                char c = numberString[i];

                if (c == '.')
                {
                    width += digitSpacing * 0.5f;
                }
                else if (char.IsDigit(c))
                {
                    int digit = c - '0';
                    if (digit >= 0 && digit < digitSprites.Length && digitSprites[digit] != null)
                    {
                        width += digitSprites[digit].bounds.size.x * digitScale;
                        if (i < numberString.Length - 1)
                        {
                            width += digitSpacing;
                        }
                    }
                }
            }

            return width;
        }

        /// <summary>
        /// Calculate starting X position based on alignment
        /// </summary>
        private float CalculateStartX(float totalWidth)
        {
            switch (alignment)
            {
                case TextAlignment.Left:
                    return 0f;
                case TextAlignment.Center:
                    return -totalWidth / 2f;
                case TextAlignment.Right:
                    return -totalWidth;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Clear all digit objects
        /// </summary>
        private void ClearDigits()
        {
            foreach (GameObject obj in digitObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            digitObjects.Clear();
        }

        private void OnDestroy()
        {
            ClearDigits();
        }
    }
}
