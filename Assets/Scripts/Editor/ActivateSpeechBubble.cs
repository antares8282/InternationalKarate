using UnityEngine;
using UnityEditor;

public class ActivateSpeechBubble
{
    [MenuItem("Tools/Activate Speech Bubble")]
    static void Activate()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            var bubble = canvas.transform.Find("SenseiSpeechBubble");
            if (bubble != null)
            {
                bubble.gameObject.SetActive(true);
                Debug.Log("Activated SenseiSpeechBubble");
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
            else
            {
                Debug.LogError("SenseiSpeechBubble not found under Canvas");
            }
        }
        else
        {
            Debug.LogError("Canvas not found");
        }
    }
}
