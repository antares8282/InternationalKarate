using UnityEngine;

namespace InternationalKarate.Managers
{
    /// <summary>
    /// Manages background selection and randomization
    /// </summary>
    public class BackgroundManager : MonoBehaviour
    {
        [Header("Background Settings")]
        [Tooltip("All available background sprites")]
        public Sprite[] backgrounds;

        [Tooltip("Randomize background on match start")]
        public bool randomizeOnStart = true;

        [Tooltip("Reference to the background SpriteRenderer")]
        public SpriteRenderer backgroundRenderer;

        [Header("Current Background")]
        [SerializeField] private int currentBackgroundIndex = 0;

        private void Start()
        {
            if (randomizeOnStart)
            {
                SetRandomBackground();
            }
            else if (backgrounds != null && backgrounds.Length > 0)
            {
                SetBackground(0);
            }
        }

        /// <summary>
        /// Set a random background from the available backgrounds
        /// </summary>
        public void SetRandomBackground()
        {
            if (backgrounds == null || backgrounds.Length == 0)
            {
                Debug.LogWarning("No backgrounds assigned to BackgroundManager!");
                return;
            }

            int randomIndex = Random.Range(0, backgrounds.Length);
            SetBackground(randomIndex);
        }

        /// <summary>
        /// Set a specific background by index
        /// </summary>
        public void SetBackground(int index)
        {
            if (backgrounds == null || backgrounds.Length == 0)
            {
                Debug.LogWarning("No backgrounds assigned to BackgroundManager!");
                return;
            }

            if (index < 0 || index >= backgrounds.Length)
            {
                Debug.LogWarning($"Background index {index} out of range! Using index 0.");
                index = 0;
            }

            currentBackgroundIndex = index;

            if (backgroundRenderer != null)
            {
                backgroundRenderer.sprite = backgrounds[index];
                Debug.Log($"Background changed to: {backgrounds[index].name}");
            }
            else
            {
                Debug.LogWarning("Background SpriteRenderer not assigned!");
            }
        }

        /// <summary>
        /// Get the current background index
        /// </summary>
        public int GetCurrentBackgroundIndex()
        {
            return currentBackgroundIndex;
        }

        /// <summary>
        /// Cycle to the next background (for testing/preview)
        /// </summary>
        public void NextBackground()
        {
            if (backgrounds == null || backgrounds.Length == 0)
                return;

            int nextIndex = (currentBackgroundIndex + 1) % backgrounds.Length;
            SetBackground(nextIndex);
        }

        /// <summary>
        /// Cycle to the previous background (for testing/preview)
        /// </summary>
        public void PreviousBackground()
        {
            if (backgrounds == null || backgrounds.Length == 0)
                return;

            int prevIndex = currentBackgroundIndex - 1;
            if (prevIndex < 0)
                prevIndex = backgrounds.Length - 1;

            SetBackground(prevIndex);
        }

    }
}
