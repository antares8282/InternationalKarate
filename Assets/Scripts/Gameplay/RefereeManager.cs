using UnityEngine;
using System.Collections;

namespace InternationalKarate.Gameplay
{
    /// <summary>
    /// Manages the referee sprite and speech bubbles
    /// Displays messages like "BEGIN", "FULL POINT", "HALF POINT", score values, etc.
    /// </summary>
    public class RefereeManager : MonoBehaviour
    {
        public static RefereeManager Instance { get; private set; }

        [Header("Referee Sprite")]
        [Tooltip("SpriteRenderer for the referee character")]
        public SpriteRenderer refereeSprite;

        [Header("Speech Bubble")]
        [Tooltip("Parent GameObject containing the speech bubble")]
        public GameObject speechBubbleObject;

        [Tooltip("TextMeshPro component for the speech bubble text")]
        public TMPro.TextMeshPro speechBubbleText;

        [Header("Display Settings")]
        [Tooltip("How long to show the speech bubble (seconds)")]
        public float displayDuration = 2.0f;

        [Tooltip("Fade in/out duration")]
        public float fadeDuration = 0.3f;

        private Coroutine currentDisplayCoroutine;

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
            // Hide speech bubble initially
            if (speechBubbleObject != null)
            {
                speechBubbleObject.SetActive(false);
            }
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
        /// Show a custom message in the speech bubble
        /// </summary>
        public void ShowMessage(string message, float duration = -1)
        {
            if (speechBubbleObject == null || speechBubbleText == null)
            {
                Debug.LogWarning("Speech bubble components not assigned!");
                return;
            }

            // Stop any existing display coroutine
            if (currentDisplayCoroutine != null)
            {
                StopCoroutine(currentDisplayCoroutine);
            }

            // Use default duration if not specified
            if (duration < 0)
            {
                duration = displayDuration;
            }

            currentDisplayCoroutine = StartCoroutine(DisplayMessageCoroutine(message, duration));
        }

        /// <summary>
        /// Hide the speech bubble immediately
        /// </summary>
        public void HideMessage()
        {
            if (currentDisplayCoroutine != null)
            {
                StopCoroutine(currentDisplayCoroutine);
                currentDisplayCoroutine = null;
            }

            if (speechBubbleObject != null)
            {
                speechBubbleObject.SetActive(false);
            }
        }

        private IEnumerator DisplayMessageCoroutine(string message, float duration)
        {
            // Set the text
            speechBubbleText.text = message;

            // Show the bubble
            speechBubbleObject.SetActive(true);

            // Optional: Fade in (if you want to implement fading)
            // yield return StartCoroutine(FadeIn());

            // Wait for display duration
            yield return new WaitForSeconds(duration);

            // Optional: Fade out (if you want to implement fading)
            // yield return StartCoroutine(FadeOut());

            // Hide the bubble
            speechBubbleObject.SetActive(false);

            currentDisplayCoroutine = null;
        }

        /// <summary>
        /// Show the referee sprite
        /// </summary>
        public void ShowReferee()
        {
            if (refereeSprite != null)
            {
                refereeSprite.enabled = true;
            }
        }

        /// <summary>
        /// Hide the referee sprite
        /// </summary>
        public void HideReferee()
        {
            if (refereeSprite != null)
            {
                refereeSprite.enabled = false;
            }
        }
    }
}
