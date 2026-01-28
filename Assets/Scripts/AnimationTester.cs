using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InternationalKarate.Audio;
using InternationalKarate.Gameplay;

/// <summary>
/// Test script for viewing all Player1 animations and Sensei speech bubbles.
/// Uses alphabet keys A-Z for all controls (no Tab/pages).
/// </summary>
public class AnimationTester : MonoBehaviour
{
    [Header("Target")]
    public Animator targetAnimator;
    public Transform targetTransform;
    public SpriteRenderer targetSpriteRenderer;

    [Header("Sensei")]
    public SenseiManager senseiManager;

    [Header("Settings")]
    public float jumpDistance = 54f;
    public float jumpHeight = 1.8f;
    public float flyingKickDistance = 2f;
    public float flyingKickHeight = 4f;
    public float miniJumpHeight = 1.65f; // approx 75% of character height

    private string currentAnimation = "";
    private bool isPlaying = false;
    private float currentSpeed = 1f;
    private Coroutine playCoroutine;
    private Vector3 playerStartPosition;

    private Dictionary<string, float> speedMultipliers = new Dictionary<string, float>()
    {
        {"Idle", 1f},
        {"Walking", 0.07f},
        {"Wait", 0.06f},
        {"Greet", 0.06f},
        {"jump", 0.8f},
        {"MiniJump", 0.35f},
        {"HighPunch", 0.5f},
        {"GroinPunch", 0.5f},
        {"HighKick", 0.4f},
        {"LowKick", 1f},
        {"CrouchKick", 0.425f},
        {"RoundHouse", 0.375f},
        {"FlyingKick", 0.6f},
        {"AnkleKick", 0.1f},
        {"Hurt", 0.126f},
        {"HurtGroin", 0.126f}
    };

    private void Start()
    {
        if (targetAnimator == null)
        {
            var player = GameObject.Find("Player1");
            if (player != null)
            {
                targetAnimator = player.GetComponent<Animator>();
                targetTransform = player.transform;
                targetSpriteRenderer = player.GetComponent<SpriteRenderer>();
            }
        }
        else
        {
            if (targetTransform == null) targetTransform = targetAnimator.transform;
            if (targetSpriteRenderer == null) targetSpriteRenderer = targetAnimator.GetComponent<SpriteRenderer>();
        }
        if (targetTransform != null)
            playerStartPosition = targetTransform.position;

        if (senseiManager == null)
            senseiManager = FindObjectOfType<SenseiManager>();

        // Disable PlayerInput to prevent key conflicts during testing
        var playerInput = FindObjectOfType<InternationalKarate.Characters.PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;
    }

    private void Update()
    {
        if (targetAnimator == null) return;

        // === ANIMATIONS (A-P) ===
        // A-F: Basic animations
        if (Input.GetKeyDown(KeyCode.A)) PlayAnimationOnce("Idle");
        if (Input.GetKeyDown(KeyCode.B)) PlayAnimationOnce("Walking");
        if (Input.GetKeyDown(KeyCode.C)) PlayAnimationOnce("Wait");
        if (Input.GetKeyDown(KeyCode.D)) PlayAnimationOnce("Greet");
        if (Input.GetKeyDown(KeyCode.E)) PlayAnimationOnce("jump");
        if (Input.GetKeyDown(KeyCode.F)) PlayAnimationOnce("MiniJump");

        // G-L: Attack animations
        if (Input.GetKeyDown(KeyCode.G)) PlayAnimationOnce("HighPunch");
        if (Input.GetKeyDown(KeyCode.H)) PlayAnimationOnce("GroinPunch");
        if (Input.GetKeyDown(KeyCode.I)) PlayAnimationOnce("HighKick");
        if (Input.GetKeyDown(KeyCode.J)) PlayAnimationOnce("LowKick");
        if (Input.GetKeyDown(KeyCode.K)) PlayAnimationOnce("CrouchKick");
        if (Input.GetKeyDown(KeyCode.L)) PlayAnimationOnce("RoundHouse");

        // M-P: More attacks + hurt
        if (Input.GetKeyDown(KeyCode.M)) PlayAnimationOnce("FlyingKick");
        if (Input.GetKeyDown(KeyCode.N)) PlayAnimationOnce("AnkleKick");
        if (Input.GetKeyDown(KeyCode.O)) PlayAnimationOnce("Hurt");
        if (Input.GetKeyDown(KeyCode.P)) PlayAnimationOnce("HurtGroin");

        // === SENSEI SPEECH BUBBLES (Q-Z) ===
        if (senseiManager != null)
        {
            if (Input.GetKeyDown(KeyCode.Q)) senseiManager.ShowBegin();
            if (Input.GetKeyDown(KeyCode.R)) senseiManager.ShowFullPoint();
            if (Input.GetKeyDown(KeyCode.S)) senseiManager.ShowHalfPoint();
            if (Input.GetKeyDown(KeyCode.T)) senseiManager.ShowScore(800);
            if (Input.GetKeyDown(KeyCode.U)) senseiManager.ShowScore(1000);
            if (Input.GetKeyDown(KeyCode.V)) senseiManager.ShowTime();
            if (Input.GetKeyDown(KeyCode.W)) senseiManager.ShowDraw();
            if (Input.GetKeyDown(KeyCode.X)) senseiManager.ShowYouWin();
            if (Input.GetKeyDown(KeyCode.Y)) senseiManager.ShowYouLose();
            if (Input.GetKeyDown(KeyCode.Z)) senseiManager.ShowMatchOver();
        }

        // === UTILITY ===
        // Space: Flip sprite
        if (Input.GetKeyDown(KeyCode.Space) && targetSpriteRenderer != null)
        {
            targetSpriteRenderer.flipX = !targetSpriteRenderer.flipX;
        }
        // Backspace: Hide speech bubble
        if (Input.GetKeyDown(KeyCode.Backspace) && senseiManager != null)
        {
            senseiManager.HideMessage();
        }
    }

    private void PlayAnimationOnce(string animName)
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            targetAnimator.speed = 1f;
        }
        playCoroutine = StartCoroutine(PlayAndReturnToIdle(animName));
    }

    private IEnumerator PlayAndReturnToIdle(string animName)
    {
        Debug.Log($"Playing: {animName}");
        currentAnimation = animName;
        isPlaying = true;

        currentSpeed = speedMultipliers.ContainsKey(animName) ? speedMultipliers[animName] : 1f;
        targetAnimator.speed = currentSpeed;
        targetAnimator.Play(animName, 0, 0f);

        Vector3 startPos = targetTransform.position;
        bool needsHorizontalMove = (animName == "jump" || animName == "FlyingKick");
        bool needsVerticalMove = (animName == "MiniJump");
        float moveDistance = animName == "FlyingKick" ? flyingKickDistance : jumpDistance;

        // Direction based on sprite flip (feet should match face direction)
        float direction = (targetSpriteRenderer != null && targetSpriteRenderer.flipX) ? -1f : 1f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayAttackSound(animName);

        yield return null;
        yield return null;

        AnimatorStateInfo stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
        float timeout = 5f;
        float elapsed = 0f;

        while (stateInfo.normalizedTime < 0.95f && elapsed < timeout)
        {
            float progress = Mathf.Clamp01(stateInfo.normalizedTime);

            if (needsHorizontalMove && targetTransform != null)
            {
                float height = (animName == "jump") ? jumpHeight : flyingKickHeight;
                float verticalArc = Mathf.Sin(progress * Mathf.PI) * height;
                targetTransform.position = startPos + new Vector3(moveDistance * progress * direction, verticalArc, 0f);
            }

            if (needsVerticalMove && targetTransform != null)
            {
                // Arc motion: up then down
                float verticalProgress = Mathf.Sin(progress * Mathf.PI);
                targetTransform.position = startPos + new Vector3(0f, miniJumpHeight * verticalProgress, 0f);
            }

            yield return null;
            elapsed += Time.deltaTime;
            stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
        }

        if (needsHorizontalMove && targetTransform != null)
            targetTransform.position = startPos + new Vector3(moveDistance * direction, 0f, 0f);

        if (needsVerticalMove && targetTransform != null)
            targetTransform.position = startPos;

        targetAnimator.speed = 1f;
        targetAnimator.Play("Idle", 0, 0f);
        isPlaying = false;
        currentAnimation = "Idle";
        Debug.Log($"Finished: {animName} -> Idle");
    }

    private void OnGUI()
    {
        // GUI disabled - check console for test instructions
    }
}
