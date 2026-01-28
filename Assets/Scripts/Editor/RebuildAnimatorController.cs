using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

public class RebuildAnimatorController : Editor
{
    [MenuItem("Tools/Rebuild Player1 Animator (Clean)")]
    public static void Rebuild()
    {
        string controllerPath = "Assets/Animations/Player1/Player1Controller.controller";

        // Create new controller
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Add parameters
        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isJumping", AnimatorControllerParameterType.Bool);

        var rootStateMachine = controller.layers[0].stateMachine;

        // Animation clips dictionary
        var clips = new Dictionary<string, string>
        {
            {"Idle", "Assets/Animations/Player1/Idle.anim"},
            {"Walking", "Assets/Animations/Player1/Walking.anim"},
            {"Wait", "Assets/Animations/Player1/Wait.anim"},
            {"Greet", "Assets/Animations/Player1/Greet.anim"},
            {"jump", "Assets/Animations/Player1/jump.anim"},
            {"MiniJump", "Assets/Animations/Player1/MiniJump.anim"},
            {"HighPunch", "Assets/Animations/Player1/HighPunch.anim"},
            {"GroinPunch", "Assets/Animations/Player1/GroinPunch.anim"},
            {"HighKick", "Assets/Animations/Player1/HighKick.anim"},
            {"LowKick", "Assets/Animations/Player1/LowKick.anim"},
            {"CrouchKick", "Assets/Animations/Player1/CrouchKick.anim"},
            {"RoundHouse", "Assets/Animations/Player1/RoundHouse.anim"},
            {"FlyingKick", "Assets/Animations/Player1/FlyingKick.anim"},
            {"AnkleKick", "Assets/Animations/Player1/AnkleKick.anim"},
            {"Hurt", "Assets/Animations/Player1/Hurt.anim"},
            {"HurtGroin", "Assets/Animations/Player1/HurtGroin.anim"}
        };

        // Create all states
        var states = new Dictionary<string, AnimatorState>();
        float yPos = 0;

        foreach (var kvp in clips)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(kvp.Value);
            if (clip == null)
            {
                Debug.LogWarning($"Could not load clip: {kvp.Value}");
                continue;
            }

            var state = rootStateMachine.AddState(kvp.Key, new Vector3(300, yPos, 0));
            state.motion = clip;
            states[kvp.Key] = state;
            yPos += 50;
        }

        // Set Idle as default
        if (states.ContainsKey("Idle"))
        {
            rootStateMachine.defaultState = states["Idle"];
        }

        // NO automatic transitions - AnimationTester will handle everything programmatically
        // This prevents the weird state jumping issues

        AssetDatabase.SaveAssets();
        Debug.Log("Player1 Animator Controller rebuilt with clean state machine (no auto-transitions)");
    }
}
