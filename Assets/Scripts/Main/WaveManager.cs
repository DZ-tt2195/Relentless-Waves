using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    List<BaseEnemy> allEnemies = new();
    Wave[] listOfWaves;
    BaseEnemy[] enemiesToSpawn;

    [Foldout("Prefabs", true)]
    [SerializeField] Resupply resupplyPrefab;
    [SerializeField] HealthPack healthPack;
    Queue<Resupply> resupplyQueue = new();

    [Foldout("UI", true)]
    public Camera mainCamera;
    [SerializeField] Slider waveSlider;
    [SerializeField] TMP_Text waveCounter;
    int currentWave = -1;
    [SerializeField] Slider enemySlider;
    [SerializeField] TMP_Text enemyCounter;
    [SerializeField] TMP_Text endingText;

    public static float minX { get; private set; }
    public static float maxX { get; private set; }
    public static float minY { get; private set; }
    public static float maxY { get; private set; }

    private void Awake()
    {
        instance = this;

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        minX = mainCamera.transform.position.x - cameraWidth / 2f;
        maxX = mainCamera.transform.position.x + cameraWidth / 2f;
        minY = mainCamera.transform.position.y - cameraHeight / 2f;
        maxY = 3.5f;

        listOfWaves = Resources.LoadAll<Wave>("Waves");
        enemiesToSpawn = Resources.LoadAll<BaseEnemy>("Enemies");

        InvokeRepeating(nameof(SpawnResupply), 1f, 2.25f);
        NewWave();
    }

    void SpawnResupply()
    {
        Resupply resupply = (resupplyQueue.Count > 0) ? resupplyQueue.Dequeue() : Instantiate(resupplyPrefab);
        resupply.transform.position = new(Random.Range(minX + 0.5f, maxX - 0.5f), maxY);
        resupply.gameObject.SetActive(true);
    }

    public void ReturnResupply(Resupply resupply)
    {
        resupplyQueue.Enqueue(resupply);
        resupply.gameObject.SetActive(false);
    }

    void NewWave()
    {
        currentWave++;
        if (currentWave < listOfWaves.Count())
        {
            if (currentWave >= 1)
            {
                HealthPack pack = Instantiate(healthPack);
                pack.transform.position = new(Random.Range(minX + 0.5f, maxX - 0.5f), maxY);
            }
            foreach (Vector2 vector in listOfWaves[currentWave].enemySpawns)
                CreateEnemy(vector, enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)]);
            waveSlider.value = (currentWave + 1) / (float)listOfWaves.Length;
            waveCounter.text = $"Wave {currentWave + 1} / {listOfWaves.Length}";
        }
        else
        {
            Bullet[] bullets = FindObjectsByType<Bullet>(FindObjectsSortMode.None);
            foreach (Bullet bullet in bullets)
                Destroy(bullet.gameObject);

            (int missedBullets, int tookDamage) = Player.instance.PlayerStats();
            EndGame($"You Won!", new(missedBullets, tookDamage));

            float currentDifficulty = PlayerPrefs.GetFloat("Difficulty");
            float bestDifficulty = PlayerPrefs.HasKey("Best Difficulty") ? PlayerPrefs.GetFloat("Best Difficulty") : 0f;
            int currentScore = missedBullets + tookDamage;
            int bestScore = PlayerPrefs.GetInt("Best Bullet") + PlayerPrefs.GetInt("Best Damage");

            if (currentDifficulty > bestDifficulty)
            {
                PlayerPrefs.SetFloat("Best Difficulty", currentDifficulty);
                PlayerPrefs.SetInt("Best Bullet", missedBullets);
                PlayerPrefs.SetInt("Best Damage", tookDamage);
            }
            else if (currentDifficulty == bestDifficulty && currentScore < bestScore)
            {
                PlayerPrefs.SetInt("Best Bullet", missedBullets);
                PlayerPrefs.SetInt("Best Damage", tookDamage);
            }
        }
    }

    public void CreateEnemy(Vector2 start, BaseEnemy prefab)
    {
        BaseEnemy enemy = Instantiate(prefab);
        enemy.EnemySetup();
        enemy.transform.position = start;
        allEnemies.Add(enemy);
    }

    private void Update()
    {
        allEnemies.RemoveAll(enemy => enemy == null);
        int currentEnemies = 0;
        if (allEnemies.Count > 0)
        {
            foreach (BaseEnemy enemy in allEnemies)
            {
                if (enemy.health != 0)
                    currentEnemies++;
            }

            enemySlider.value = (float)currentEnemies / allEnemies.Count;
            enemyCounter.text = $"Enemies: {currentEnemies} / {allEnemies.Count}";

            if (currentEnemies == 0)
            {
                for (int i = allEnemies.Count - 1; i >= 0; i--)
                    Destroy(allEnemies[i].gameObject);
                NewWave();
            }
        }
    }

    public void EndGame(string text, (int missedBullets, int tookDamage) stats)
    {
        if (!endingText.transform.parent.gameObject.activeSelf)
        {
            endingText.transform.parent.gameObject.SetActive(true);
            endingText.text = $"{text}\n\nMissed {stats.missedBullets} bullets\nTook {stats.tookDamage} damage";
        }
    }
}