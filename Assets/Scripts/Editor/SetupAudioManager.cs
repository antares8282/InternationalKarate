using UnityEngine;
using UnityEditor;
using InternationalKarate.Audio;

public class SetupAudioManager : Editor
{
    [MenuItem("Tools/Setup AudioManager")]
    public static void Setup()
    {
        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in scene!");
            return;
        }

        var so = new SerializedObject(audioManager);

        // Get the AudioSources on the GameObject
        var audioSources = audioManager.GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            so.FindProperty("musicSource").objectReferenceValue = audioSources[0];
            so.FindProperty("sfxSource").objectReferenceValue = audioSources[1];
        }

        // Background Music
        var bgMusic = so.FindProperty("backgroundMusic");
        bgMusic.arraySize = 2;
        bgMusic.GetArrayElementAtIndex(0).objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music/BackGroundMusic.wav");
        bgMusic.GetArrayElementAtIndex(1).objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music/BackGroundMusic2.wav");

        // Sound Effects
        so.FindProperty("tune3").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/tune3.wav");
        so.FindProperty("tune4").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/tune4.wav");
        so.FindProperty("tune5").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/tune5.wav");
        so.FindProperty("tune6").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/tune6.wav");
        so.FindProperty("tune7").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/tune7.wav");
        so.FindProperty("tune8").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/tune8.wav");

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(audioManager);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(audioManager.gameObject.scene);
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        // Verify assignments
        var verify = new SerializedObject(audioManager);
        var bg = verify.FindProperty("backgroundMusic");
        Debug.Log($"BG Music 0: {(bg.GetArrayElementAtIndex(0).objectReferenceValue != null ? "SET" : "NULL")}");
        Debug.Log($"BG Music 1: {(bg.GetArrayElementAtIndex(1).objectReferenceValue != null ? "SET" : "NULL")}");
        Debug.Log($"Tune3: {(verify.FindProperty("tune3").objectReferenceValue != null ? "SET" : "NULL")}");

        Debug.Log("AudioManager setup complete and scene saved!");
    }
}
