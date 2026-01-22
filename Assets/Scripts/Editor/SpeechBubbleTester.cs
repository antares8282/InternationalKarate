using UnityEngine;
using InternationalKarate.Gameplay;

namespace InternationalKarate.Editor
{
    /// <summary>
    /// Test script for speech bubble messages
    /// Attach to any GameObject in the scene to test different messages
    /// </summary>
    public class SpeechBubbleTester : MonoBehaviour
    {
        [Header("Test Controls")]
        [Tooltip("Press these keys during Play mode to test messages")]
        public KeyCode testBegin = KeyCode.Alpha1;
        public KeyCode testHalfPoint = KeyCode.Alpha2;
        public KeyCode testFullPoint = KeyCode.Alpha3;
        public KeyCode testMatchOver = KeyCode.Alpha4;
        public KeyCode testWhiteBelt = KeyCode.Alpha5;
        public KeyCode testRedBelt = KeyCode.Alpha6;
        public KeyCode testBlackBelt = KeyCode.Alpha7;
        public KeyCode testTime = KeyCode.Alpha8;
        public KeyCode testYouWin = KeyCode.Alpha9;
        public KeyCode testYouLose = KeyCode.Alpha0;
        public KeyCode hideMessage = KeyCode.H;

        [Header("Settings")]
        [Tooltip("Show instructions in console on start")]
        public bool showInstructions = true;

        private void Start()
        {
            if (showInstructions)
            {
                Debug.Log("=== SPEECH BUBBLE TESTER ===");
                Debug.Log("Press number keys to test messages:");
                Debug.Log($"  {testBegin} - BEGIN");
                Debug.Log($"  {testHalfPoint} - HALF POINT");
                Debug.Log($"  {testFullPoint} - FULL POINT");
                Debug.Log($"  {testMatchOver} - MATCH OVER");
                Debug.Log($"  {testWhiteBelt} - WHITE (belt)");
                Debug.Log($"  {testRedBelt} - RED (belt)");
                Debug.Log($"  {testBlackBelt} - BLACK (belt)");
                Debug.Log($"  {testTime} - TIME");
                Debug.Log($"  {testYouWin} - YOU WIN");
                Debug.Log($"  {testYouLose} - YOU LOSE");
                Debug.Log($"  {hideMessage} - Hide bubble");
                Debug.Log("===========================");
            }
        }

        private void Update()
        {
            if (SenseiManager.Instance == null)
            {
                return;
            }

            // Test BEGIN
            if (Input.GetKeyDown(testBegin))
            {
                Debug.Log("Testing: BEGIN");
                SenseiManager.Instance.ShowBegin();
            }

            // Test HALF POINT
            if (Input.GetKeyDown(testHalfPoint))
            {
                Debug.Log("Testing: HALF POINT");
                SenseiManager.Instance.ShowHalfPoint();
            }

            // Test FULL POINT
            if (Input.GetKeyDown(testFullPoint))
            {
                Debug.Log("Testing: FULL POINT");
                SenseiManager.Instance.ShowFullPoint();
            }

            // Test MATCH OVER
            if (Input.GetKeyDown(testMatchOver))
            {
                Debug.Log("Testing: MATCH OVER");
                SenseiManager.Instance.ShowMatchOver();
            }

            // Test WHITE belt
            if (Input.GetKeyDown(testWhiteBelt))
            {
                Debug.Log("Testing: WHITE belt");
                SenseiManager.Instance.ShowBeltColor("WHITE");
            }

            // Test RED belt
            if (Input.GetKeyDown(testRedBelt))
            {
                Debug.Log("Testing: RED belt");
                SenseiManager.Instance.ShowBeltColor("RED");
            }

            // Test BLACK belt
            if (Input.GetKeyDown(testBlackBelt))
            {
                Debug.Log("Testing: BLACK belt");
                SenseiManager.Instance.ShowBeltColor("BLACK");
            }

            // Test TIME
            if (Input.GetKeyDown(testTime))
            {
                Debug.Log("Testing: TIME");
                SenseiManager.Instance.ShowTime();
            }

            // Test YOU WIN
            if (Input.GetKeyDown(testYouWin))
            {
                Debug.Log("Testing: YOU WIN");
                SenseiManager.Instance.ShowYouWin();
            }

            // Test YOU LOSE
            if (Input.GetKeyDown(testYouLose))
            {
                Debug.Log("Testing: YOU LOSE");
                SenseiManager.Instance.ShowYouLose();
            }

            // Hide message
            if (Input.GetKeyDown(hideMessage))
            {
                Debug.Log("Hiding speech bubble");
                SenseiManager.Instance.HideMessage();
            }
        }

        private void OnGUI()
        {
            if (SenseiManager.Instance == null)
            {
                GUI.Label(new Rect(10, 10, 500, 30), "⚠ SenseiManager not found in scene!");
                return;
            }

            // Show on-screen instructions
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = Color.yellow;

            float y = 10;
            GUI.Label(new Rect(10, y, 400, 25), "SPEECH BUBBLE TESTER", style);

            style.fontSize = 12;
            style.normal.textColor = Color.white;
            y += 30;

            GUI.Label(new Rect(10, y, 400, 20), $"1 - BEGIN", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"2 - HALF POINT", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"3 - FULL POINT", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"4 - MATCH OVER", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"5 - WHITE belt", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"6 - RED belt", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"7 - BLACK belt", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"8 - TIME", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"9 - YOU WIN", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"0 - YOU LOSE", style); y += 20;
            GUI.Label(new Rect(10, y, 400, 20), $"H - Hide bubble", style);
        }
    }
}
