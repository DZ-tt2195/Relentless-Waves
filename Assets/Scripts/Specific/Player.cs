using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Collections;
using System.Diagnostics;
using System;

public class Player : Entity
{
    public static Player instance;
    public static bool paused = false;

    [Foldout("Player info", true)]
    int currentBullet;
    [SerializeField] int maxBullet;
    [SerializeField] float immuneTime;
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

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        instance = this;
        paused = false;
        Time.timeScale = 1f;

        this.tag = "Player";
        currentBullet = maxBullet;
        immuneTime *= 2 - PlayerPrefs.GetFloat("Difficulty");

        gameTimer = new Stopwatch();
        gameTimer.Start();
    }

    void Update()
    {
        if (health > 0 && !paused)
        {
            FollowMouse();
            ShootBullet();
        }

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

        healthSlider.value = health / (float)maxHealth;
        healthCounter.text = $"Health: {health} / {maxHealth}";
        bulletSlider.value = currentBullet / (float)maxBullet;
        bulletCounter.text = $"Bullets: {currentBullet} / {maxBullet}";

        timerText.text = $"{PlayerPrefs.GetFloat("Difficulty") * 100:F1}% Difficulty\n{StopwatchTime(gameTimer)}";
        string StopwatchTime(Stopwatch stopwatch)
        {
            TimeSpan time = stopwatch.Elapsed;
            string part = time.Seconds < 10 ? $"0{time.Seconds}" : $"{time.Seconds}";
            return $"{time.Minutes}:{part}.{time.Milliseconds}";
        }
    }

    void ShootBullet()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && currentBullet >= 1)
        {
            currentBullet--;
            firedBullets++;
            CreateBullet(prefab, this.transform.position, bulletSpeed, Vector3.up);
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

    protected override void DamageEffect()
    {
        tookDamage++;
        StartCoroutine(Immunity());
        IEnumerator Immunity()
        {
            immune = true;
            float elapsedTime = 0f;
            bool flicker = true;

            Vector3 darkness = new(0.1f, 0.1f, 0.1f);
            Vector3 gray = new(0.25f, 0.25f, 0.25f);
            WaveManager.instance.mainCamera.backgroundColor = new(darkness.x, darkness.y, darkness.z);

            while (elapsedTime < immuneTime)
            {
                flicker = !flicker;
                elapsedTime += Time.deltaTime;
                SetAlpha(this.spriteRenderer, flicker ? (elapsedTime / immuneTime) : 1f);
                Vector3 target = Vector3.Lerp(darkness, gray, elapsedTime / immuneTime);
                WaveManager.instance.mainCamera.backgroundColor = new(target.x, target.y, target.z);
                yield return null;
            }

            WaveManager.instance.mainCamera.backgroundColor = new(gray.x, gray.y, gray.z);
            SetAlpha(this.spriteRenderer, 1);
            immune = false;
        }
    }

    protected override void DeathEffect()
    {
        immune = true;
        SetAlpha(this.spriteRenderer, 0.5f);
        WaveManager.instance.EndGame($"You Lost.", PlayerStats());
    }

    public (int, int) PlayerStats()
    {
        gameTimer.Stop();
        return (firedBullets - landedBullets, tookDamage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity entity))
        {
            this.TakeDamage();
        }
        else if (collision.CompareTag("Wall"))
        {
            this.TakeDamage();
        }
        else if (collision.CompareTag("Resupply") && currentBullet < maxBullet)
        {
            WaveManager.instance.ReturnResupply(collision.GetComponent<Resupply>());
            currentBullet = Mathf.Min(currentBullet + 2, maxBullet);
        }
        else if (collision.CompareTag("HealthPack") && health < maxHealth)
        {
            Destroy(collision.gameObject);
            health++;
        }
    }
}