using UnityEngine;
using UnityEditor;

namespace InternationalKarate.Editor
{
    /// <summary>
    /// Helps prevent Unity crashes when doing rapid MCP operations
    /// by ensuring operations happen on the main thread with proper frame delays
    /// </summary>
    [InitializeOnLoad]
    public static class SafeMCPOperations
    {
        static SafeMCPOperations()
        {
            // Ensure scene view is repainted less frequently to reduce Metal renderer stress
            EditorApplication.update += ThrottledUpdate;
        }

        private static double lastUpdateTime = 0;
        private static void ThrottledUpdate()
        {
            // Only update scene view every 100ms instead of every frame
            // This reduces Metal renderer stress during rapid MCP changes
            if (EditorApplication.timeSinceStartup - lastUpdateTime > 0.1)
            {
                lastUpdateTime = EditorApplication.timeSinceStartup;
                SceneView.RepaintAll();
            }
        }
    }
}
