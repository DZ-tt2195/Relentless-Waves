using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
    public int health;
    protected SpriteRenderer spriteRenderer;

    protected bool immune = false;
    float immuneTime = 0f;
    [SerializeField] protected float bulletSpeed;

    protected Bullet prefab { get; private set; }
    Queue<Bullet> bulletQueue = new();

    protected virtual void Awake()
    {
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        prefab = this.transform.Find("Bullet").GetComponent<Bullet>();
        prefab.gameObject.SetActive(false);
    }

    public virtual void Setup(float immuneTime, string tag)
    {
        this.tag = tag;
        this.immuneTime = immuneTime;
    }

    IEnumerator Immunity()
    {
        immune = true;
        float elapsedTime = 0f;
        bool flicker = true;

        Vector3 darkness = new(0.1f, 0.1f, 0.1f);
        Vector3 gray = new(0.25f, 0.25f, 0.25f);
        Player.instance.mainCamera.backgroundColor = new(darkness.x, darkness.y, darkness.z);

        while (elapsedTime < immuneTime)
        {
            flicker = !flicker;
            elapsedTime += Time.deltaTime;
            SetAlpha(flicker ? (elapsedTime / immuneTime) : 1f);
            Vector3 target = Vector3.Lerp(darkness, gray, elapsedTime / immuneTime);
            Player.instance.mainCamera.backgroundColor = new(target.x, target.y, target.z);
            yield return null;
        }

        Player.instance.mainCamera.backgroundColor = new(gray.x, gray.y, gray.z);
        SetAlpha(1);
        immune = false;
    }

    protected void SetAlpha(float alpha)
    {
        Color newColor = spriteRenderer.color;
        newColor.a = alpha;
        spriteRenderer.color = newColor;
    }

    public void TakeDamage()
    {
        if (immune)
            return;

        health--;
        if (health == 0)
            DeathEffect();
        else
            StartCoroutine(Immunity());
    }

    protected virtual void DeathEffect()
    {
    }

    protected void CreateBullet(Bullet prefab, float bulletSpeed, Vector3 start, Vector3 direction)
    {
        Bullet newBullet = (bulletQueue.Count > 0) ? bulletQueue.Dequeue() : Instantiate(prefab);
        newBullet.tag = this.tag;
        newBullet.transform.position = start;
        newBullet.AssignInfo(bulletSpeed, direction, this);
        newBullet.gameObject.SetActive(true);
    }

    public void ReturnBullet(Bullet bullet)
    {
        bulletQueue.Enqueue(bullet);
        bullet.gameObject.SetActive(false);
    }
}