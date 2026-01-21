using UnityEngine;
using InternationalKarate.Combat;

namespace InternationalKarate.Managers
{
    /// <summary>
    /// Debug utilities for testing and development
    /// </summary>
    public class DebugManager : MonoBehaviour
    {
        [Header("Debug Settings")]
        public bool enableDebugMode = true;
        public bool showHitboxes = true;
        public bool showPlayerPositions = true;
        public bool logCombatEvents = true;

        [Header("Quick Test Keys")]
        public KeyCode restartMatchKey = KeyCode.R;
        public KeyCode togglePauseKey = KeyCode.P;
        public KeyCode player1WinKey = KeyCode.Alpha1;
        public KeyCode player2WinKey = KeyCode.Alpha2;

        private bool isPaused = false;

        private void Update()
        {
            if (!enableDebugMode)
                return;

            HandleDebugInput();
        }

        private void HandleDebugInput()
        {
            // Restart match
            if (UnityEngine.Input.GetKeyDown(restartMatchKey))
            {
                Debug.Log("[DEBUG] Restarting match...");
                MatchManager.Instance?.RestartMatch();
            }

            // Toggle pause
            if (UnityEngine.Input.GetKeyDown(togglePauseKey))
            {
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0f : 1f;
                Debug.Log($"[DEBUG] Game {(isPaused ? "PAUSED" : "RESUMED")}");
            }

            // Force Player 1 win (for testing)
            if (UnityEngine.Input.GetKeyDown(player1WinKey))
            {
                Debug.Log("[DEBUG] Forcing Player 1 win...");
                MatchManager.Instance?.OnFighterHit(1, PointValue.Full);
                MatchManager.Instance?.OnFighterHit(1, PointValue.Full);
            }

            // Force Player 2 win (for testing)
            if (UnityEngine.Input.GetKeyDown(player2WinKey))
            {
                Debug.Log("[DEBUG] Forcing Player 2 win...");
                MatchManager.Instance?.OnFighterHit(2, PointValue.Full);
                MatchManager.Instance?.OnFighterHit(2, PointValue.Full);
            }
        }

        private void OnGUI()
        {
            if (!enableDebugMode)
                return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("=== DEBUG MODE ===");
            GUILayout.Label($"FPS: {(1f / Time.deltaTime):F1}");

            if (MatchManager.Instance != null)
            {
                GUILayout.Label($"P1 Score: {MatchManager.Instance.GetPlayer1Score()}");
                GUILayout.Label($"P2 Score: {MatchManager.Instance.GetPlayer2Score()}");
                GUILayout.Label($"Time: {MatchManager.Instance.GetRoundTimeRemaining():F1}s");
                GUILayout.Label($"Round Active: {MatchManager.Instance.isRoundActive}");
            }

            GUILayout.Label($"Paused: {isPaused}");
            GUILayout.Label("---");
            GUILayout.Label($"R - Restart | P - Pause");
            GUILayout.Label($"1/2 - Force Win P1/P2");

            GUILayout.EndArea();
        }

        /// <summary>
        /// Log combat event for debugging
        /// </summary>
        public static void LogCombatEvent(string message)
        {
            DebugManager instance = FindObjectOfType<DebugManager>();
            if (instance != null && instance.logCombatEvents)
            {
                Debug.Log($"[COMBAT] {message}");
            }
        }

        private void OnDestroy()
        {
            // Reset time scale when debug manager is destroyed
            Time.timeScale = 1f;
        }
    }
}
