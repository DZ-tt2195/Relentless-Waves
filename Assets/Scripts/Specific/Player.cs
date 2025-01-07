using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class Player : Entity
{
    public static Player instance;
    public static bool paused = false;

    [Foldout("Player info", true)]
    int currentBullet;
    [SerializeField] int maxBullet;

    [Foldout("UI", true)]
    [SerializeField] Slider bulletSlider;
    [SerializeField] TMP_Text bulletCounter;

    [SerializeField] GameObject pauseScreen;
    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text healthCounter;

    int maxHealth;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        instance = this;
        this.Setup(2f, "Player");

        currentBullet = maxBullet;
        maxHealth = health;
    }

    void Update()
    {
        if (health > 0 && !paused)
        {
            FollowMouse();
            ShootBullet();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
            pauseScreen.SetActive(paused);
            Time.timeScale = (paused) ? 0f : 1f;
        }

        healthSlider.value = health / (float)maxHealth;
        healthCounter.text = $"Health: {health} / {maxHealth}";
        bulletSlider.value = currentBullet / (float)maxBullet;
        bulletCounter.text = $"Bullets: {currentBullet} / {maxBullet}";
    }

    void ShootBullet()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && currentBullet >= 1)
        {
            currentBullet--;
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

    protected override void DeathEffect()
    {
        immune = true;
        SetAlpha(0.5f);
        WaveManager.instance.EndGame("You Lost.");
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
        else if (collision.TryGetComponent(out Resupply resupply) && currentBullet < maxBullet)
        {
            WaveManager.instance.ReturnResupply(resupply);
            currentBullet = Mathf.Min(currentBullet + 2, maxBullet);
        }
    }
}