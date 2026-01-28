using UnityEngine;
using System.Collections;

namespace InternationalKarate.Gameplay
{
    /// <summary>
    /// Manages the sensei sprite and speech bubbles
    /// Displays messages like "BEGIN", "FULL POINT", "HALF POINT", score values, etc.
    /// </summary>
    public class SenseiManager : MonoBehaviour
    {
        public static SenseiManager Instance { get; private set; }

        [Header("Sensei Sprite")]
        [Tooltip("SpriteRenderer for the sensei character")]
        public SpriteRenderer senseiSprite;

        [Header("Speech Bubble")]
        [Tooltip("SpeechBubbleController component")]
        public SpeechBubbleController speechBubbleController;

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
            // Speech bubble starts hidden by default in SpeechBubbleController
        }

        /// <summary>
        /// Show "BEGIN" message at match start
        /// </summary>
        public void ShowBegin()
        {
            ShowMessage("BEGIN");
        }

        /// <summary>
        /// Show "FULL POINT" message
        /// </summary>
        public void ShowFullPoint()
        {
            ShowMessage("FULL\nPOINT");
        }

        /// <summary>
        /// Show "HALF POINT" message
        /// </summary>
        public void ShowHalfPoint()
        {
            ShowMessage("HALF\nPOINT");
        }

        /// <summary>
        /// Show score value (800, 1000, etc.)
        /// </summary>
        public void ShowScore(int score)
        {
            ShowMessage(score.ToString());
        }

        /// <summary>
        /// Show "TIME" message when time runs out
        /// </summary>
        public void ShowTime()
        {
            ShowMessage("TIME");
        }

        /// <summary>
        /// Show "DRAW" message for tied matches
        /// </summary>
        public void ShowDraw()
        {
            ShowMessage("DRAW");
        }

        /// <summary>
        /// Show "YOU WIN" message
        /// </summary>
        public void ShowYouWin()
        {
            ShowMessage("YOU\nWIN");
        }

        /// <summary>
        /// Show "YOU LOSE" message
        /// </summary>
        public void ShowYouLose()
        {
            ShowMessage("YOU\nLOSE");
        }

        /// <summary>
        /// Show "MATCH OVER" message
        /// </summary>
        public void ShowMatchOver()
        {
            ShowMessage("MATCH\nOVER");
        }

        /// <summary>
        /// Show belt color announcement (WHITE, RED, BROWN, BLACK, etc.)
        /// Used at the start of a match to announce opponent difficulty
        /// </summary>
        public void ShowBeltColor(string beltColor)
        {
            ShowMessage(beltColor.ToUpper());
        }

        /// <summary>
        /// Show a custom message in the speech bubble
        /// </summary>
        public void ShowMessage(string message)
        {
            if (speechBubbleController == null)
            {
                Debug.LogWarning("SpeechBubbleController not assigned!");
                return;
            }

            speechBubbleController.ShowMessage(message);
        }

        /// <summary>
        /// Hide the speech bubble immediately
        /// </summary>
        public void HideMessage()
        {
            if (speechBubbleController != null)
            {
                speechBubbleController.HideImmediately();
            }
        }

        /// <summary>
        /// Show the sensei sprite
        /// </summary>
        public void ShowSensei()
        {
            if (senseiSprite != null)
            {
                senseiSprite.enabled = true;
            }
        }

        /// <summary>
        /// Hide the sensei sprite
        /// </summary>
        public void HideSensei()
        {
            if (senseiSprite != null)
            {
                senseiSprite.enabled = false;
            }
        }
    }
}
