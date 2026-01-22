using UnityEngine;
using TMPro;
using System.Collections;

namespace InternationalKarate.UI
{
    /// <summary>
    /// Manages the referee's speech bubble that appears during match events
    /// </summary>
    public class RefereeSpeechBubble : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The TextMeshPro component for the bubble text")]
        public TextMeshProUGUI bubbleText;

        [Header("Display Settings")]
        [Tooltip("How long to display each message")]
        public float displayDuration = 2f;

        private Coroutine displayCoroutine;

        /// <summary>
        /// Show the speech bubble with a message
        /// </summary>
        public void ShowMessage(string message)
        {
            // Stop any existing display
            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }

            // Start new display
            displayCoroutine = StartCoroutine(DisplayMessage(message));
        }

        /// <summary>
        /// Coroutine to display message and then hide
        /// </summary>
        private IEnumerator DisplayMessage(string message)
        {
            // Set text
            if (bubbleText != null)
            {
                bubbleText.text = message;
            }

            // Show bubble
            gameObject.SetActive(true);

            // Wait for duration
            yield return new WaitForSeconds(displayDuration);

            // Hide bubble
            HideBubble();
        }

        /// <summary>
        /// Hide the speech bubble immediately
        /// </summary>
        public void HideBubble()
        {
            gameObject.SetActive(false);
            if (bubbleText != null)
            {
                bubbleText.text = "";
            }
        }

        /// <summary>
        /// Common referee messages
        /// </summary>
        public void ShowHalfPoint()
        {
            ShowMessage("HALF POINT");
        }

        public void ShowFullPoint()
        {
            ShowMessage("FULL POINT");
        }

        public void ShowBegin()
        {
            ShowMessage("BEGIN!");
        }

        public void ShowStop()
        {
            ShowMessage("STOP");
        }

        public void ShowTimeOut()
        {
            ShowMessage("TIME OUT");
        }

        public void ShowWinner(int playerNumber)
        {
            ShowMessage($"PLAYER {playerNumber} WINS!");
        }
    }
}
