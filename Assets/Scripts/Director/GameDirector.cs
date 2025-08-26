using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class GameDirector : MonoBehaviour
{
    public static GameDirector instance;
    [SerializeField] GameObject enemy;
    [SerializeField] DamageManager player;

    [Header("Damage managers")]
    DamageManager enemyDamageManager;
    DamageManager playerDamageManager;

    [Header("Health thresholds")]
    float threshOne = 0.75f;
    float threshTwo = 0.5f;
    float threshThree = 0.25f;
    float threshFour = 0.1f;
    bool passedThreshOne = false;
    bool passedThreshTwo = false;
    bool passedThreshThree = false;
    bool passedThreshFour = false;

    [Header("Future Sight Positioning")]
    [SerializeField] Vector3 enemyPosition;
    [SerializeField] Quaternion enemyRotation;
    [SerializeField] Vector3 playerPosition;
    [SerializeField] Quaternion playerRotation;

    [Header("Timeline assets")]
    [SerializeField] PlayableDirector director;
    Playable currentPlayable;
    [SerializeField] PlayableAsset mainPlayableScene;
    [SerializeField] PlayableAsset[] failureTimeline;
    MeshTrail[] trails;

    [Header("UI")]
    [SerializeField] GameObject defaultUI;
    [SerializeField] GameObject inputPromptUI;
    RectTransform inputPromptRect;
    [SerializeField] TextMeshProUGUI inputPromptText;
    [SerializeField] Image inputPromptOutline;
    float inputPromptTimer = 2f;
    float timer = 0f;

    [Header("Input Prompts")]
    int currentInputPrompt = 0;
    bool isExpectingInput = false;
    string[] controllerInputPrompts = new string[]
    {
    };
    string[] keyboardInputPrompts = new string[]
    {
        "E",
        "Q",
        "R",
        "P",
        "J",
    };
    [SerializeField]
    Vector2[] promptPositions = new Vector2[]
    {
        new Vector2(0, -200),
        new Vector2(0, 200),
        new Vector2(-300, 0),
        new Vector2(300, 0),
        new Vector2(0, 0),
    };
    [SerializeField] float[] promptTimeScales;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }

        enemyDamageManager = enemy.GetComponent<DamageManager>();
        playerDamageManager = player.GetComponent<DamageManager>();
        inputPromptRect = inputPromptUI.GetComponent<RectTransform>();

        // Ensure the prompt UI is in disabled at the start
        if (inputPromptUI.activeSelf == true)
            inputPromptUI.SetActive(false);

        // Find all mesh trail scripts and disable them
        trails = FindObjectsByType<MeshTrail>(FindObjectsSortMode.None);
        // todo: disable mesh trail scripts
    }

    void Update()
    {
        if (isExpectingInput)
        {
            // Check for input
            if (Input.GetKeyDown(keyboardInputPrompts[currentInputPrompt - 1].ToLower()))
            {
                ResetPrompt();
                return;
            }

            timer += Time.deltaTime;
            if (timer >= inputPromptTimer)
            {
                ResetPrompt();

                // Move to the failure timeline
                director.Play(failureTimeline[currentInputPrompt - 1]);

                // todo: add a failure global volume
            }
            else
            {
                // Update the outline fill amount
                inputPromptOutline.fillAmount = 1 - (timer / inputPromptTimer);
            }
        }
    }

    public void CheckHealth(bool isPlayer)
    {
        if (isPlayer)
            CheckPlayerHealth();
        else
            CheckEnemyHealth();
    }

    void CheckPlayerHealth()
    {
        float playerHealth = playerDamageManager.currentHealth;
        if (playerHealth < 0f)
        {
            // todo: trigger death
            // todo: show UI
            // todo: disable character and player controllers
        }
    }
    void CheckEnemyHealth()
    {
        float currentHealth = enemyDamageManager.currentHealth;
        float maxHealth = enemyDamageManager.maxHealth;
        float normalizedHealth = currentHealth / maxHealth;

        if (normalizedHealth < threshOne && !passedThreshOne)
        {
            passedThreshOne = true;
            // todo: unlock new special
            // todo: increase attack rate and or reduce wait time
        }
        else if (normalizedHealth < threshTwo && !passedThreshTwo)
        {
            passedThreshTwo = true;
            // todo: unlock new special
            // todo: increase attack rate and or reduce wait time
            FutureSight();
        }
        else if (normalizedHealth < threshThree & !passedThreshThree)
        {
            passedThreshThree = true;
            // todo: unlock new special
            // todo: increase attack rate and or reduce wait time

        }
        else if (normalizedHealth < threshFour && !passedThreshFour)
        {
            passedThreshFour = true;
            FutureSight();
        }
    }

    void FutureSight()
    {
        // todo: disable player controller
        // todo: apply some sort of volume transition or add a black screen before playing the timeline asset

        // Disable the default UI
        defaultUI.SetActive(false);

        // Set positions and rotation to the origin
        player.transform.SetPositionAndRotation(playerPosition, playerRotation);
        enemy.transform.SetPositionAndRotation(enemyPosition, enemyRotation);

        // Play the timeline asset
        director.Play();

        // Enable mesh trail scripts
        foreach (MeshTrail trail in trails)
        {
            trail.enabled = true;
        }

    }

    public void StartPlayableScene()
    {
        // Disable mesh trail scripts
        foreach (MeshTrail trail in trails)
        {
            trail.enabled = false;
        }

        if (mainPlayableScene != null)
        {
            director.Play(mainPlayableScene);
            currentPlayable = director.playableGraph.GetRootPlayable(0);
        }
    }

    public void ShowNextInputPrompt()
    {
        currentInputPrompt++;
        isExpectingInput = true;
        inputPromptUI.SetActive(true);
        inputPromptText.text = keyboardInputPrompts[currentInputPrompt - 1];
        inputPromptRect.anchoredPosition = promptPositions[currentInputPrompt - 1];

        // Slow down director playback speed
        float desiredSpeed = promptTimeScales[currentInputPrompt - 1] / inputPromptTimer;
        currentPlayable.SetSpeed(desiredSpeed);
    }

    void ResetPrompt()
    {
        isExpectingInput = false;
        inputPromptUI.SetActive(false);
        timer = 0f;
        currentPlayable.SetSpeed(1f);
    }
}
