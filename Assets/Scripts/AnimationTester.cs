using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InternationalKarate.Audio;

/// <summary>
/// Test script for viewing all Player1 animations in play mode.
/// Each animation plays ONCE completely then returns to Idle.
/// </summary>
public class AnimationTester : MonoBehaviour
{
    [Header("Target")]
    public Animator targetAnimator;
    public Transform targetTransform;
    public SpriteRenderer targetSpriteRenderer;

    [Header("Settings")]
    public float jumpDistance = 6f;
    public float flyingKickDistance = 12f;

    private string currentAnimation = "";
    private bool isPlaying = false;
    private float currentSpeed = 1f;
    private Coroutine playCoroutine;
    private int currentPage = 0;
    private const int TOTAL_PAGES = 2;
    private Vector3 playerStartPosition;

    private Dictionary<string, float> speedMultipliers = new Dictionary<string, float>()
    {
        {"Idle", 1f},
        {"Walking", 0.05f},
        {"Wait", 0.05f},
        {"Greet", 0.05f},
        {"jump", 1f},
        {"MiniJump", 0.35f},
        {"HighPunch", 1f},
        {"GroinPunch", 1f},
        {"HighKick", 1f},
        {"LowKick", 1f},
        {"CrouchKick", 1f},
        {"RoundHouse", 0.5f},
        {"FlyingKick", 0.6f},
        {"AnkleKick", 0.5f},
        {"Hurt", 0.1f},
        {"HurtGroin", 0.1f}
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
    }

    private void Update()
    {
        if (targetAnimator == null) return;

        // Page navigation
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentPage = (currentPage + 1) % TOTAL_PAGES;
        }

        // Flip sprite with F key
        if (Input.GetKeyDown(KeyCode.F) && targetSpriteRenderer != null)
        {
            targetSpriteRenderer.flipX = !targetSpriteRenderer.flipX;
        }

        if (currentPage == 0)
        {
            // Page 1: Basic + Attacks
            if (Input.GetKeyDown(KeyCode.Alpha1)) PlayAnimationOnce("Idle");
            if (Input.GetKeyDown(KeyCode.Alpha2)) PlayAnimationOnce("Walking");
            if (Input.GetKeyDown(KeyCode.Alpha3)) PlayAnimationOnce("Wait");
            if (Input.GetKeyDown(KeyCode.Alpha4)) PlayAnimationOnce("Greet");
            if (Input.GetKeyDown(KeyCode.Alpha5)) PlayAnimationOnce("jump");
            if (Input.GetKeyDown(KeyCode.Alpha6)) PlayAnimationOnce("MiniJump");
            if (Input.GetKeyDown(KeyCode.Q)) PlayAnimationOnce("HighPunch");
            if (Input.GetKeyDown(KeyCode.W)) PlayAnimationOnce("GroinPunch");
            if (Input.GetKeyDown(KeyCode.E)) PlayAnimationOnce("HighKick");
            if (Input.GetKeyDown(KeyCode.R)) PlayAnimationOnce("LowKick");
        }
        else
        {
            // Page 2: More Attacks + Hurt
            if (Input.GetKeyDown(KeyCode.Alpha1)) PlayAnimationOnce("CrouchKick");
            if (Input.GetKeyDown(KeyCode.Alpha2)) PlayAnimationOnce("RoundHouse");
            if (Input.GetKeyDown(KeyCode.Alpha3)) PlayAnimationOnce("FlyingKick");
            if (Input.GetKeyDown(KeyCode.Alpha4)) PlayAnimationOnce("AnkleKick");
            if (Input.GetKeyDown(KeyCode.Alpha5)) PlayAnimationOnce("Hurt");
            if (Input.GetKeyDown(KeyCode.Alpha6)) PlayAnimationOnce("HurtGroin");
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
        float moveDistance = animName == "FlyingKick" ? flyingKickDistance : jumpDistance;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayAttackSound(animName);

        yield return null;
        yield return null;

        AnimatorStateInfo stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
        float timeout = 5f;
        float elapsed = 0f;

        while (stateInfo.normalizedTime < 0.95f && elapsed < timeout)
        {
            if (needsHorizontalMove && targetTransform != null)
            {
                float progress = Mathf.Clamp01(stateInfo.normalizedTime);
                targetTransform.position = startPos + new Vector3(moveDistance * progress, 0f, 0f);
            }

            yield return null;
            elapsed += Time.deltaTime;
            stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
        }

        if (needsHorizontalMove && targetTransform != null)
            targetTransform.position = startPos + new Vector3(moveDistance, 0f, 0f);

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
