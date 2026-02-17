using UnityEngine;

[System.Serializable]
public struct EnemyInfo
{
    public GameObject enemyPrefub;
    public Modificator[] modificators;
    public float spawnProbability;
}

public struct Wave
{
    public int waveIndex;
    public int enemiesCount;
    public int enemiesCap;
    public EnemyInfo[] enemies;

    public Wave(int index, int count, int cap, EnemyInfo[] enms)
    {
        waveIndex = index;
        enemiesCount = count;
        enemiesCap = cap;
        enemies = enms;
    }
}

public class WaveManager : MonoBehaviour
{
    [HideInInspector] public static WaveManager instance;

    [Header("Settings")]
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private AudioClip newWaveSFX;
    [Space]
    [SerializeField] private EnemyInfo[] enemiesWave1;
    [SerializeField] private EnemyInfo[] enemiesWave2;
    [SerializeField] private EnemyInfo[] enemiesWave3;
    
    [Header("Spawn Cooldown")]
    [SerializeField] private float spawnCooldownDefault = 2.0f;
    [SerializeField] private float spawnCooldownCap = 0.9f;
    [SerializeField] private float spawnCooldownDecrease = 0.05f;
    private float spawnTimer = 0.0f;
    private float spawnCooldown;

    private int wavesCount = 3;
    private Wave[] waves;
    private int currentWave = 0;
    private int enemiesTotal = 0;
    private int enemiesSpawned = 0;
    private Player player;

    private Transform objectiveTransform;
    private bool isStopped = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        objectiveTransform = FindFirstObjectByType<ObjectiveGirl>().transform;
        player = FindFirstObjectByType<Player>();

        waves = new Wave[wavesCount];
        InitializeWaves();
        enemiesTotal = waves[0].enemiesCount;

        spawnCooldown = spawnCooldownDefault;
        spawnTimer = spawnCooldown;
    }

    private void Start()
    {
        NewWaveVisual();
    }

    private void Update()
    {
        if (!isStopped)
        { 
            HandleSpawnTimer();
        }
    }

    private void HandleSpawnTimer()
    {
        if (spawnTimer > 0.0f)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0.0f)
            {
                spawnCooldown = Mathf.Max(spawnCooldownCap, spawnCooldown - spawnCooldownDecrease);
                spawnTimer = spawnCooldown;
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        Wave wave = GetCurrentWave();
        if (isStopped || enemiesSpawned >= wave.enemiesCount)
        {
            return;
        }
        SpawnRandomEnemy(wave);
        ++enemiesSpawned;
    }

    private GameObject SpawnRandomEnemy(Wave wave)
    {
        int enemyIndex = GetRandomEnemyIndex(ref wave);
        ref EnemyInfo enemyInfo = ref wave.enemies[enemyIndex];
        int randomInt = Random.Range(0, spawnLocations.Length);
        Vector3 position = spawnLocations[randomInt].position;
        
        GameObject enemy = Instantiate(enemyInfo.enemyPrefub, position, Quaternion.identity);
        Character character = enemy.GetComponent<Character>();
        
        if (position.x > objectiveTransform.position.x)
            character.FlipCharacter();

        foreach (Modificator modificator in enemyInfo.modificators)
            ApplyModificator(character, modificator);

        return enemy;
    }

    public void ApplyModificator(Character character, Modificator modificator, bool isPlayer = false)
    {
        if (isPlayer)
        {
            switch (modificator)
            {
                case Modificator.SPEED:
                    character.IncreaseSpeed(2);
                    break;
                case Modificator.HEALTH:
                    character.IncreaseHealth(1);
                    break;
                case Modificator.KNOCKBACK:
                    character.IncreaseKnockback(new Vector2(10.0f, 15.0f));
                    break;
                case Modificator.KNOCKBACK_RES:
                    character.EnableKnockbackResistance(true);
                    break;
                case Modificator.ATTACK:
                    character.IncreaseDamage(1);
                    break;
                case Modificator.ATTACK_SPEED:
                    character.IncreaseAttackSpeed(1.25f);
                    break;
                default:
                    break;
            }

            return;
        }

        switch (modificator)
        {
            case Modificator.SPEED:
                character.IncreaseSpeed(4);
                break;
            case Modificator.HEALTH:
                character.IncreaseHealth(1);
                break;
            case Modificator.KNOCKBACK:
                character.IncreaseKnockback(new Vector2(20.0f, 15.0f));
                break;
            case Modificator.KNOCKBACK_RES:
                character.EnableKnockbackResistance(true);
                break;
            case Modificator.ATTACK:
                character.IncreaseDamage(1);
                break;
            case Modificator.ATTACK_SPEED:
                character.IncreaseAttackSpeed(1.5f);
                break;
            default:
                break;
        }
    }

    private Wave GetCurrentWave()
    {
        ref Wave wave = ref waves[currentWave];
        if (player.GetKillCount() >= enemiesTotal)
        {
            ++currentWave;
            if (currentWave > wavesCount - 1)
            {
                Stop();
                UI.instance.EnableGameOverUI(true, true);
                return wave;
            }
            wave = ref waves[currentWave];
            NewWaveVisual();
            enemiesTotal += wave.enemiesCount;
            spawnCooldown = spawnCooldownDefault;
            enemiesSpawned = 0;
            spawnTimer = spawnCooldown;
        }

        return wave;
    }

    private void NewWaveVisual()
    {
        UI.instance.AnnounceWave(currentWave + 1);
        SoundManager.instance.PlaySFX(ref newWaveSFX);
    }

    private int GetRandomEnemyIndex(ref Wave wave)
    {
        float randomValue = Random.Range(0.0f, 1.0f);
        int enemyIndex = Random.Range(0, wave.enemies.Length);

        int tries = 0;
        int triesCap = 50;
        while (true)
        {
            ++tries;
            if (tries >= triesCap)
            {
                break;
            }

            ref EnemyInfo enemy = ref wave.enemies[enemyIndex];
            if (randomValue < enemy.spawnProbability)
            {
                break;
            }
            else
            {
                enemyIndex = Random.Range(0, wave.enemies.Length);
            }
        }
        return enemyIndex;
    }

    private void InitializeWaves()
    {
        AddWave(new Wave(0, 15, 10, enemiesWave1));
        AddWave(new Wave(1, 20, 10, enemiesWave2));
        AddWave(new Wave(2, 25, 10, enemiesWave3));
    }

    private void AddWave(Wave wave) => waves[wave.waveIndex] = wave;

    private void Stop() => isStopped = true;
}
