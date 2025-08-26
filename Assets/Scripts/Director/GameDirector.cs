using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using DG.Tweening;
using System.Collections;
using Unity.Behavior;

public class GameDirector : MonoBehaviour
{
    public static GameDirector instance;
    GameObject enemy;
    GameObject player;
    BehaviorGraphAgent agent;
    [SerializeField] string enemyTag;
    [SerializeField] string playerTag;
    [SerializeField] GameObject blackScreen;

    [Header("Damage managers")]
    DamageManager enemyDamageManager;
    DamageManager playerDamageManager;

    [Header("Player controllers")]
    Movement playerMovement;
    LightningMovement playerLightning;

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
    [SerializeField] Volume failureVolume;

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
    [Header("Damage Settings")]
    float damageToPlayer = 50f;
    float damageToEnemy = 100f;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }

        // Get player and enemy game objects
        enemy = GameObject.FindGameObjectWithTag(enemyTag);
        player = GameObject.FindGameObjectWithTag(playerTag);
        agent = enemy.GetComponent<BehaviorGraphAgent>();

        // Get scripts
        enemyDamageManager = enemy.GetComponent<DamageManager>();
        playerDamageManager = player.GetComponent<DamageManager>();
        playerMovement = player.GetComponent<Movement>();
        playerLightning = player.GetComponent<LightningMovement>();
        inputPromptRect = inputPromptUI.GetComponent<RectTransform>();

        // Ensure the prompt UI is in disabled at the start
        if (inputPromptUI.activeSelf == true)
            inputPromptUI.SetActive(false);

        // Find all mesh trail scripts and disable them
        trails = FindObjectsByType<MeshTrail>(FindObjectsSortMode.None);
        foreach (MeshTrail trail in trails)
        {
            trail.enabled = false;
        }

        // Disable black screen
        blackScreen.SetActive(false);
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

                // Transition to the failure global volume
                if (failureVolume != null)
                    DOTween.To(() => failureVolume.weight, x => failureVolume.weight = x, 1f, 0.5f);

                // Deal damage to the player
                if (playerDamageManager.currentHealth > damageToPlayer)
                    playerDamageManager.currentHealth -= damageToPlayer;
                else
                    playerDamageManager.currentHealth = 0f;
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
            // Disable character and player controllers
            playerMovement.enabled = false;
            playerLightning.enabled = false;
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
        else if (normalizedHealth <= 0f)
        {
            // todo: end game, show victory UI
        }
    }

    void FutureSight()
    {
        // Set the behavior graph flag
        agent.SetVariableValue("isFutureSightActive", true);

        // Disable player controller
        playerMovement.enabled = false;
        playerLightning.enabled = false;

        // Enable black screen
        blackScreen.SetActive(true);

        // Disable the default UI
        defaultUI.SetActive(false);

        // Set positions and rotation to the origin
        player.transform.SetPositionAndRotation(playerPosition, playerRotation);
        enemy.transform.SetPositionAndRotation(enemyPosition, enemyRotation);

        StartCoroutine(PlayCutsceneAfterDelay(1f));
    }

    IEnumerator PlayCutsceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Play the timeline asset
        director.Play();

        // Enable mesh trail scripts
        foreach (MeshTrail trail in trails)
        {
            trail.enabled = true;
        }

        // Disable black screen
        blackScreen.SetActive(false);
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

    public void EndFutureSight()
    {
        // Disable the behavior graph flag
        agent.SetVariableValue("isFutureSightActive", false);

        // Reset input prompt variables
        currentInputPrompt = 0;

        // Re-enable player controller
        playerMovement.enabled = true;
    }

    public void DealDamageToEnemy()
    {
        if (enemyDamageManager.currentHealth > damageToEnemy)
            enemyDamageManager.currentHealth -= damageToEnemy;
        else
            enemyDamageManager.currentHealth = 0f;

        CheckEnemyHealth();
    }

    void ResetPrompt()
    {
        isExpectingInput = false;
        inputPromptUI.SetActive(false);
        timer = 0f;
        currentPlayable.SetSpeed(1f);
    }
    void EndTimelineAsset()
    {
        // Re-enable player controller
        playerMovement.enabled = true;
        playerLightning.enabled = true;

        // Transtition out of the failure global volume
        if (failureVolume != null)
            DOTween.To(() => failureVolume.weight, x => failureVolume.weight = x, 0f, 0.5f);

        // Re-enable the default UI
        defaultUI.SetActive(true);
    }
}
