using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MyBox;

public class Entity : MonoBehaviour
{
    [Foldout("Entity info", true)]
    public int health;
    protected int maxHealth { get; private set; }
    protected SpriteRenderer spriteRenderer;

    public bool immune {get; protected set; }
    [SerializeField] protected float bulletSpeed;

    protected Bullet bulletPrefab { get; private set; }
    protected Queue<Bullet> bulletQueue = new();
    protected int landedBullets { get; private set; }

    protected virtual void Awake()
    {
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        try
        {
            bulletPrefab = this.transform.Find("Bullet").GetComponent<Bullet>();
            bulletPrefab.gameObject.SetActive(false);
        } catch { }
        maxHealth = health;
    }

    public void TakeDamage()
    {
        if (immune)
            return;

        health--;
        if (health == 0)
            DeathEffect();
        else
            DamageEffect();
    }

    protected virtual void DeathEffect()
    {
    }

    protected virtual void DamageEffect()
    {
    }

    protected Bullet CreateBullet(Bullet prefab, Vector3 start, float bulletSpeed, Vector3 direction)
    {
        Bullet newBullet = (bulletQueue.Count > 0) ? bulletQueue.Dequeue() : Instantiate(prefab);
        newBullet.transform.position = start;
        newBullet.AssignInfo(bulletSpeed, direction, this);
        return newBullet;
    }

    public void ReturnBullet(Bullet bullet, bool landed)
    {
        bulletQueue.Enqueue(bullet);
        bullet.gameObject.SetActive(false);
        if (landed)
            landedBullets++;
    }
}