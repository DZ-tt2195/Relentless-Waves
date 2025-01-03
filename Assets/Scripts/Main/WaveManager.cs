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
    Queue<Resupply> resupplyQueue = new();

    [Foldout("UI", true)]
    [SerializeField] Slider waveSlider;
    [SerializeField] TMP_Text waveCounter;
    int currentWave = -1;
    [SerializeField] Slider enemySlider;
    [SerializeField] TMP_Text enemyCounter;
    [SerializeField] TMP_Text endingText;

    private void Awake()
    {
        instance = this;

        listOfWaves = Resources.LoadAll<Wave>("Waves");
        enemiesToSpawn = Resources.LoadAll<BaseEnemy>("Enemies");

        InvokeRepeating(nameof(SpawnResupply), 1f, 2.5f);
        NewWave();
    }

    void SpawnResupply()
    {
        Resupply resupply = (resupplyQueue.Count > 0) ? resupplyQueue.Dequeue() : Instantiate(resupplyPrefab);
        resupply.transform.position = new Vector2(Random.Range(-7f, 7f), 4f);
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

            EndGame("You Won!");
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

    public void EndGame(string text)
    {
        if (!endingText.transform.parent.gameObject.activeSelf)
        {
            endingText.transform.parent.gameObject.SetActive(true);
            endingText.text = text;
        }
    }
}