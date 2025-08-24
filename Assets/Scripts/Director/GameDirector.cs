using UnityEngine;
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
    [SerializeField] PlayableAsset mainPlayableScene;

    [Header("UI")]
    [SerializeField] GameObject defaultUI;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }

        enemyDamageManager = enemy.GetComponent<DamageManager>();
        playerDamageManager = player.GetComponent<DamageManager>();
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

        // Disbale the default UI
        defaultUI.SetActive(false);

        // Set positions and rotation to the origin
        player.transform.SetPositionAndRotation(playerPosition, playerRotation);
        enemy.transform.SetPositionAndRotation(enemyPosition, enemyRotation);

        // Play the timeline asset
        director.Play();
    }

    public void StartPlayableScene()
    {
        if (mainPlayableScene != null)
            director.Play(mainPlayableScene);
    }
}
