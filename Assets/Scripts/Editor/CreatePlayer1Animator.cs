using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

public class CreatePlayer1Animator
{
    [MenuItem("Tools/Create Player1 Animator")]
    public static void CreateAnimator()
    {
        // Load the sprite sheet
        string spritePath = "Assets/Sprites/Characters/Player1.png";
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath);

        List<Sprite> spriteList = new List<Sprite>();
        foreach (Object obj in sprites)
        {
            if (obj is Sprite)
            {
                spriteList.Add(obj as Sprite);
            }
        }

        if (spriteList.Count == 0)
        {
            Debug.LogError("No sprites found in " + spritePath);
            return;
        }

        Debug.Log($"Found {spriteList.Count} sprites");

        // Group sprites by animation name (prefix before underscore)
        Dictionary<string, List<Sprite>> animationGroups = new Dictionary<string, List<Sprite>>();

        foreach (Sprite sprite in spriteList)
        {
            string spriteName = sprite.name;

            // Skip unwanted sprites: "Unused", "Player1_X" pattern, and "Bonus"
            if (spriteName == "Unused" || spriteName.StartsWith("Player1_") || spriteName.StartsWith("Bonus"))
            {
                Debug.Log($"Skipping unwanted sprite: {spriteName}");
                continue;
            }

            int underscoreIndex = spriteName.LastIndexOf('_');

            if (underscoreIndex > 0)
            {
                string animName = spriteName.Substring(0, underscoreIndex);

                if (!animationGroups.ContainsKey(animName))
                {
                    animationGroups[animName] = new List<Sprite>();
                }
                animationGroups[animName].Add(sprite);
            }
        }

        Debug.Log($"Found {animationGroups.Count} animation groups:");
        foreach (var kvp in animationGroups)
        {
            Debug.Log($"  - {kvp.Key}: {kvp.Value.Count} frames");
        }

        // Create Animations folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Animations"))
        {
            AssetDatabase.CreateFolder("Assets", "Animations");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Animations/Player1"))
        {
            AssetDatabase.CreateFolder("Assets/Animations", "Player1");
        }

        // Create animation clips
        List<AnimationClip> clips = new List<AnimationClip>();

        foreach (var group in animationGroups)
        {
            string animName = group.Key;
            List<Sprite> frames = group.Value.OrderBy(s => s.name).ToList();

            AnimationClip clip = new AnimationClip();
            clip.frameRate = 12; // Adjust as needed

            EditorCurveBinding spriteBinding = new EditorCurveBinding();
            spriteBinding.type = typeof(SpriteRenderer);
            spriteBinding.path = "";
            spriteBinding.propertyName = "m_Sprite";

            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frames.Count];
            for (int i = 0; i < frames.Count; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe();
                keyframes[i].time = i / clip.frameRate;
                keyframes[i].value = frames[i];
            }

            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);

            // Set loop settings
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            string clipPath = $"Assets/Animations/Player1/{animName}.anim";

            // Check if asset already exists
            AnimationClip existingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (existingClip != null)
            {
                // Use existing clip instead of creating new one
                clips.Add(existingClip);
                Debug.Log($"Using existing animation clip: {clipPath}");
            }
            else
            {
                AssetDatabase.CreateAsset(clip, clipPath);
                clips.Add(clip);
                Debug.Log($"Created animation clip: {clipPath}");
            }
        }

        // Create Animator Controller
        string controllerPath = "Assets/Animations/Player1/Player1Controller.controller";

        // Check if controller already exists
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        bool controllerExists = controller != null;

        if (!controllerExists)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            Debug.Log("Created new Animator Controller");
        }
        else
        {
            // Clear existing states to rebuild
            AnimatorControllerLayer[] layers = controller.layers;
            if (layers.Length > 0)
            {
                AnimatorStateMachine sm = layers[0].stateMachine;
                sm.states = new ChildAnimatorState[0];
                sm.anyStateTransitions = new AnimatorStateTransition[0];
            }
            Debug.Log("Using existing Animator Controller - rebuilding states");
        }

        // Clear existing parameters
        for (int i = controller.parameters.Length - 1; i >= 0; i--)
        {
            controller.RemoveParameter(i);
        }

        // Add parameters for controlling animations
        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isJumping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("attackTrigger", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("attackType", AnimatorControllerParameterType.Int);

        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine stateMachine = layer.stateMachine;

        // Create states and organize them
        AnimatorState idleState = null;
        AnimatorState walkState = null;
        AnimatorState jumpState = null;
        Dictionary<string, AnimatorState> attackStates = new Dictionary<string, AnimatorState>();

        foreach (AnimationClip clip in clips)
        {
            AnimatorState state = stateMachine.AddState(clip.name);
            state.motion = clip;

            string clipNameLower = clip.name.ToLower();

            // Identify special states
            if (clipNameLower.Contains("idle"))
            {
                idleState = state;
                Debug.Log($"Found idle state: {clip.name}");
            }
            else if (clipNameLower.Contains("walk"))
            {
                walkState = state;
                Debug.Log($"Found walk state: {clip.name}");
            }
            else if (clipNameLower.StartsWith("jump") && !clipNameLower.Contains("mini"))
            {
                jumpState = state;
                Debug.Log($"Found jump state: {clip.name}");
            }
            else if (clipNameLower.Contains("kick") || clipNameLower.Contains("punch"))
            {
                attackStates[clip.name] = state;
            }
        }

        // Set default state to idle
        if (idleState != null)
        {
            stateMachine.defaultState = idleState;
            Debug.Log($"Set default state to: {idleState.name}");
        }
        else
        {
            Debug.LogWarning("No idle state found!");
        }

        // Create transitions: Idle <-> Walk
        Debug.Log($"Creating transitions - Idle: {(idleState != null ? idleState.name : "NULL")}, Walk: {(walkState != null ? walkState.name : "NULL")}, Jump: {(jumpState != null ? jumpState.name : "NULL")}");

        if (idleState != null && walkState != null && idleState != walkState)
        {
            var idleToWalk = idleState.AddTransition(walkState);
            idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "isWalking");
            idleToWalk.hasExitTime = false;
            idleToWalk.duration = 0.1f;

            var walkToIdle = walkState.AddTransition(idleState);
            walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isWalking");
            walkToIdle.hasExitTime = false;
            walkToIdle.duration = 0.1f;
        }

        // Create transitions: Idle/Walk -> Jump -> Idle
        if (jumpState != null && idleState != null)
        {
            var idleToJump = idleState.AddTransition(jumpState);
            idleToJump.AddCondition(AnimatorConditionMode.If, 0, "isJumping");
            idleToJump.hasExitTime = false;
            idleToJump.duration = 0.05f;

            var jumpToIdle = jumpState.AddTransition(idleState);
            jumpToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isJumping");
            jumpToIdle.hasExitTime = true;
            jumpToIdle.exitTime = 0.9f;
            jumpToIdle.duration = 0.1f;

            if (walkState != null)
            {
                var walkToJump = walkState.AddTransition(jumpState);
                walkToJump.AddCondition(AnimatorConditionMode.If, 0, "isJumping");
                walkToJump.hasExitTime = false;
                walkToJump.duration = 0.05f;
            }
        }

        // Create transitions: Any State -> Attacks -> Idle
        foreach (var kvp in attackStates)
        {
            AnimatorState attackState = kvp.Value;

            // Any state to attack
            var anyToAttack = stateMachine.AddAnyStateTransition(attackState);
            anyToAttack.AddCondition(AnimatorConditionMode.If, 0, "attackTrigger");
            anyToAttack.hasExitTime = false;
            anyToAttack.duration = 0.05f;

            // Attack back to idle
            if (idleState != null)
            {
                var attackToIdle = attackState.AddTransition(idleState);
                attackToIdle.hasExitTime = true;
                attackToIdle.exitTime = 0.95f;
                attackToIdle.duration = 0.1f;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Player1 Animator Controller created successfully!");
        Debug.Log("Next steps:");
        Debug.Log("1. Select Player1 GameObject in the scene");
        Debug.Log("2. Add an Animator component if not already present");
        Debug.Log("3. Assign Assets/Animations/Player1/Player1Controller.controller to the Animator");
    }
}
