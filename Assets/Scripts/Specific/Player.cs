using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Collections;
using System.Diagnostics;
using System;
using System.Linq;

public class Player : Entity
{

#region Setup

    public static Player instance;
    public static bool paused = false;

    [Foldout("Player info", true)]
    int currentBullet;
    [SerializeField] int maxBullet;
    [SerializeField] float immuneTime;
    [SerializeField] GameObject blackOutObject;
    public float blackOutTime = 0f;
    int firedBullets;
    int tookDamage;
    Stopwatch gameTimer;

    [Foldout("UI", true)]
    [SerializeField] TMP_Text timerText;
    [SerializeField] Slider bulletSlider;
    [SerializeField] TMP_Text bulletCounter;

    [SerializeField] GameObject pauseScreen;
    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text healthCounter;

    [Foldout("FPS", true)]
    int lastframe = 0;
    int lastupdate = 60;
    float[] framearray = new float[60];

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        instance = this;
        paused = false;
        Time.timeScale = 1f;

        this.tag = "Player";
        currentBullet = maxBullet;
        immuneTime *= 2 - PrefManager.GetDifficulty();

        gameTimer = new Stopwatch();
        gameTimer.Start();
    }

    #endregion

#region Gameplay

    void Update()
    {
        if (health > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
            pauseScreen.SetActive(paused);
            Time.timeScale = (paused) ? 0f : 1f;
            if (paused)
                gameTimer.Stop();
            else
                gameTimer.Start();
        }

        if (!paused)
        {
            if (health > 0)
            {
                FollowMouse();
                ShootBullet();
            }

            if (blackOutTime > 0f)
                blackOutTime -= Time.deltaTime;
            blackOutObject.SetActive(blackOutTime > 0f);

            healthSlider.value = health / (float)maxHealth;
            healthCounter.text = AutoTranslate.Health(health.ToString(), maxHealth.ToString());

            if (PrefManager.GetJuggle() == 0)
            {
                bulletSlider.value = currentBullet / (float)maxBullet;
                bulletCounter.text = AutoTranslate.Bullets(currentBullet.ToString(), maxBullet.ToString());
            }
            else
            {
                bulletSlider.value = (float)maxBullet / (float)maxBullet;
                bulletCounter.text = AutoTranslate.Bullets("\u221E", "\u221E");
            }

            timerText.text = $"{AutoTranslate.Difficulty($"{PrefManager.GetDifficulty()*100:F0}")}\n{MyExtensions.StopwatchTime(gameTimer)}";
            timerText.text += $" | {AutoTranslate.FPS(GetFPS())}";

            string GetFPS()
            {
                framearray[lastframe] = Time.deltaTime;
                lastframe = (lastframe + 1);
                if (lastframe == 60)
                {
                    lastframe = 0;
                    float total = 0;
                    for (int i = 0; i < framearray.Length; i++)
                        total += framearray[i];
                    lastupdate = (int)(framearray.Length / total);
                    return lastupdate.ToString();
                }
                return (lastupdate > Application.targetFrameRate) ? Application.targetFrameRate.ToString() : lastupdate.ToString();
            }
        }
    }

    void ShootBullet()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && currentBullet >= 1)
        {
            if (PrefManager.GetInfinity() == 0) 
                currentBullet--;
            firedBullets++;
            CreateBullet(bulletPrefab, this.transform.position, bulletSpeed, Vector3.up);
        }
    }

    void FollowMouse()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 targetPosition = WaveManager.instance.mainCamera.ScreenToWorldPoint
            (new(mouseScreenPosition.x, mouseScreenPosition.y, WaveManager.instance.mainCamera.nearClipPlane));

        targetPosition.x = Mathf.Clamp(targetPosition.x, WaveManager.minX, WaveManager.maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, WaveManager.minY, WaveManager.maxY);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity entity))
        {
            this.TakeDamage();
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("HurtOnTouch"))
        {
            this.TakeDamage();
        }
        else if (collision.CompareTag("Resupply") && currentBullet < maxBullet)
        {
            WaveManager.instance.ReturnResupply(collision.GetComponent<Resupply>());
            AddBullet(2);
        }
        else if (collision.CompareTag("HealthPack") && health < maxHealth)
        {
            Destroy(collision.gameObject);
            health++;
        }
    }

    public void AddBullet(int addition)
    {
        currentBullet = Mathf.Min(currentBullet + addition, maxBullet);
    }

    #endregion

#region Ending

    protected override void DamageEffect()
    {
        tookDamage++;
        StartCoroutine(Immunity(true));
    }

    public IEnumerator Immunity(bool animation)
    {
        immune = true;
        float elapsedTime = 0f;
        bool flicker = true;

        Vector3 darkness = new(0.1f, 0.1f, 0.1f);
        Vector3 gray = new(0.25f, 0.25f, 0.25f);
        if (animation)
            WaveManager.instance.mainCamera.backgroundColor = new(darkness.x, darkness.y, darkness.z);

        while (elapsedTime < immuneTime)
        {
            elapsedTime += Time.deltaTime;
            if (animation)
            {
                flicker = !flicker;
                SetAlpha(this.spriteRenderer, flicker ? (elapsedTime / immuneTime) : 1f);
                Vector3 target = Vector3.Lerp(darkness, gray, elapsedTime / immuneTime);
                WaveManager.instance.mainCamera.backgroundColor = new(target.x, target.y, target.z);
            }
            yield return null;
        }

        WaveManager.instance.mainCamera.backgroundColor = new(gray.x, gray.y, gray.z);
        SetAlpha(this.spriteRenderer, 1);
        immune = false;        
    }

    protected override void DeathEffect()
    {
        immune = true;
        tookDamage++;
        SetAlpha(this.spriteRenderer, 0.5f);

        Level currentLevel = Translator.inst.CurrentLevel();
        if (currentLevel.endless)
        {
            int score = (int)(PrefManager.GetDifficulty() * 100) + (WaveManager.instance.currentWave-1)*10;
            if (PrefManager.GetJuggle() == 1)
                score += 15;
            if (PrefManager.GetInfinity() == 1)
                score -= 15;

            WaveManager.instance.EndGame(AutoTranslate.DoEnum(ToTranslate.Lost), PlayerStats(), score);
            if (score > PrefManager.GetScore(currentLevel.levelName.ToString()))
                PrefManager.SetScore(currentLevel.levelName.ToString(), score);
        }
        else
        {
            WaveManager.instance.EndGame(AutoTranslate.DoEnum(ToTranslate.Lost), PlayerStats(), -1);
        }
    }

    public (int, int) PlayerStats()
    {
        gameTimer.Stop();
        return (firedBullets - landedBullets, tookDamage);
    }

    #endregion

}