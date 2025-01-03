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
    public Camera mainCamera;
    [SerializeField] Slider bulletSlider;
    [SerializeField] TMP_Text bulletCounter;

    [SerializeField] GameObject pauseScreen;
    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text healthCounter;

    float minX;
    float maxX;
    float minY;
    float maxY;
    int maxHealth;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        instance = this;
        this.Setup(2f, "Player");

        currentBullet = maxBullet;
        maxHealth = health;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        minX = mainCamera.transform.position.x - cameraWidth / 2f;
        maxX = mainCamera.transform.position.x + cameraWidth / 2f;
        minY = mainCamera.transform.position.y - cameraHeight / 2f;
        maxY = 3.5f;
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
            CreateBullet(prefab, bulletSpeed, this.transform.position, Vector3.up);
        }
    }

    void FollowMouse()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
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
        else if (collision.TryGetComponent(out Resupply resupply) && currentBullet < maxBullet)
        {
            WaveManager.instance.ReturnResupply(resupply);
            currentBullet = Mathf.Min(currentBullet + 2, maxBullet);
        }
    }
}