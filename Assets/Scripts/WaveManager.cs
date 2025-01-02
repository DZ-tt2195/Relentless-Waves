using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Week1
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager instance;

        List<BaseEnemy> allEnemies = new();
        Wave[] listOfWaves;
        EnemyStat[] listOfEnemies;

        Queue<Bullet> bulletQueue = new();
        Queue<Resupply> resupplyQueue = new();

        [Foldout("Prefabs", true)]
        [SerializeField] Bullet bulletPrefab;
        [SerializeField] BaseEnemy enemyPrefab;
        [SerializeField] Resupply resupplyPrefab;

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

            listOfWaves = Resources.LoadAll<Wave>("Week1/Waves");
            listOfEnemies = Resources.LoadAll<EnemyStat>("Week1/Enemies");

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
                    CreateEnemy(vector, listOfEnemies[Random.Range(0, listOfEnemies.Length)]);
                waveSlider.value = ((currentWave + 1) / (float)listOfWaves.Length);
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

        public void CreateEnemy(Vector2 start, EnemyStat stat)
        {
            BaseEnemy enemy = Instantiate(enemyPrefab);
            enemy.EnemySetup(stat);
            enemy.transform.position = start;
            allEnemies.Add(enemy);
        }

        public void CreateBullet(Entity entity, Color color, Vector3 start, Vector3 scale, Vector3 direction)
        {
            Bullet bullet = (bulletQueue.Count > 0) ? bulletQueue.Dequeue() : Instantiate(bulletPrefab);
            bullet.spriteRenderer.color = color;
            bullet.tag = entity.tag;
            bullet.transform.localScale = scale;
            bullet.transform.position = start;
            bullet.direction = direction;
            bullet.gameObject.SetActive(true);
        }

        public void ReturnBullet(Bullet bullet)
        {
            bulletQueue.Enqueue(bullet);
            bullet.gameObject.SetActive(false);
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
}