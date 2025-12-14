using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class WaveManager : MonoBehaviour
{

#region Setup

    public static WaveManager instance;
    List<BaseEnemy> allEnemies = new();

    [Foldout("Prefabs", true)]
    [SerializeField] Resupply resupplyPrefab;
    [SerializeField] HealthPack healthPack;
    [SerializeField] JuggleBall jugglePrefab;
    Queue<Resupply> resupplyQueue = new();

    [Foldout("UI", true)]
    public Camera mainCamera;
    [SerializeField] Slider waveSlider;
    [SerializeField] TMP_Text waveCounter;
    public int currentWave { get; private set; }
    [SerializeField] Slider enemySlider;
    [SerializeField] TMP_Text enemyCounter;
    [SerializeField] TMP_Text endingText;
    [SerializeField] TMP_Text tutorialText;

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

        InvokeRepeating(nameof(SpawnResupply), 1f, 2.25f);
        currentWave = PrefManager.GetStartWave()-1;
        for (int i = 0; i<currentWave; i++)
            CreateJuggleBall();

        NewWave();
    }

    #endregion

#region Gameplay

    void SpawnResupply()
    {
        if (PrefManager.GetInfinity() == 0)
        {
            Resupply resupply = (resupplyQueue.Count > 0) ? resupplyQueue.Dequeue() : Instantiate(resupplyPrefab);
            resupply.transform.position = new(Random.Range(minX + 0.5f, maxX - 0.5f), maxY);
            resupply.gameObject.SetActive(true);
        }
    }

    public void ReturnResupply(Resupply resupply)
    {
        resupplyQueue.Enqueue(resupply);
        resupply.gameObject.SetActive(false);
    }

    void NewWave()
    {
        Level currentLevel = Translator.inst.CurrentLevel();

        if (currentWave < currentLevel.listOfWaves.Count() || currentLevel.endless)
        {
            CreateJuggleBall();
            if (currentWave >= 1)
            {
                HealthPack pack = Instantiate(healthPack);
                pack.transform.position = new(Random.Range(minX + 0.5f, maxX - 0.5f), maxY);
            }

            foreach (Collection collection in currentLevel.listOfWaves[Mathf.Min(currentLevel.listOfWaves.Count, currentWave)].enemies)
                CreateEnemy(collection.position, collection.toCreate);

            waveSlider.value = (currentWave + 1) / (float)currentLevel.listOfWaves.Count;

            if (!currentLevel.endless)
            {
                waveCounter.text = AutoTranslate.Wave((currentWave+1).ToString(), currentLevel.listOfWaves.Count.ToString());
                tutorialText.text = AutoTranslate.DoEnum(currentLevel.listOfWaves[currentWave].tutorialKey);;
            }
            else
            {
                waveCounter.text = AutoTranslate.Wave((currentWave+1).ToString(), "\u221E");
                tutorialText.text = "";
            }
        }
        else
        {
            Bullet[] allBullets = FindObjectsByType<Bullet>(FindObjectsSortMode.None);
            foreach (Bullet bullet in allBullets) Destroy(bullet.gameObject);
            JuggleBall[] allBalls = FindObjectsByType<JuggleBall>(FindObjectsSortMode.None);
            foreach (JuggleBall ball in allBalls) Destroy(ball.gameObject);

            (int missedBullets, int tookDamage) = Player.instance.PlayerStats();

            int score = (int)(PrefManager.GetDifficulty() * 100) - missedBullets - tookDamage;
            if (PrefManager.GetJuggle() == 1)
                score += 15;
            if (PrefManager.GetInfinity() == 1)
                score -= 15;

            string endText = AutoTranslate.DoEnum(ToTranslate.Victory);
            if (PrefManager.GetStartWave() > 1)
                endText += AutoTranslate.Skipped_Ahead(PrefManager.GetStartWave().ToString());
            else if (score > PrefManager.GetScore(currentLevel.levelName.ToString()))
                PrefManager.SetScore(currentLevel.levelName.ToString(), score);
            EndGame(endText, new(missedBullets, tookDamage), score);
        }
    }

    void CreateJuggleBall()
    {
        if (PrefManager.GetJuggle() == 1)
        {
            JuggleBall newBall = Instantiate(jugglePrefab);
            newBall.transform.position = new(Random.Range(minX + 0.5f, maxX - 0.5f), maxY);
        }
    }

    void CreateEnemy(Vector2 start, BaseEnemy prefab)
    {
        BaseEnemy enemy = Instantiate(prefab != null ? prefab : Translator.inst.RandomEnemy());
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
            enemyCounter.text = AutoTranslate.Enemies(currentEnemies.ToString(), allEnemies.Count().ToString());

            if (currentEnemies == 0)
            {
                for (int i = allEnemies.Count - 1; i >= 0; i--)
                    Destroy(allEnemies[i].gameObject);
                currentWave++;
                NewWave();
            }
        }
    }

    public void EndGame(string text, (int missedBullets, int tookDamage) stats, int score)
    {
        if (!endingText.transform.parent.gameObject.activeSelf)
        {
            endingText.transform.parent.gameObject.SetActive(true);
            endingText.text = $"{text}\n\n" +
                $"{AutoTranslate.Bullets_Missed(stats.missedBullets.ToString())}" +
                $"\n{AutoTranslate.Health_Lost(stats.tookDamage.ToString())}\n";
            if (score > 0)
                endingText.text += $"\n{AutoTranslate.Score(score.ToString())}";
        }
    }

    #endregion

}