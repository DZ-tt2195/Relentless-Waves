using UnityEngine;
using System;
using MyBox;

public class Bullet : MonoBehaviour
{
    protected Vector3 direction;
    public Entity owner { get; private set; }
    protected float bulletSpeed;

    protected virtual void TryAndReturn(bool landed)
    {
        if (owner == null)
            Destroy(this.gameObject);
        else
            owner.ReturnBullet(this, landed);
    }

    public virtual void AssignInfo(float speed, Vector3 direction, Entity owner)
    {
        this.tag = owner.tag;
        this.bulletSpeed = speed;
        this.direction = direction;
        this.owner = owner;
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        Movement();
        if (this.transform.position.x < WaveManager.minX - 0.5f || this.transform.position.x > WaveManager.maxX + 0.5f ||
            this.transform.position.y < WaveManager.minY - 0.5f || this.transform.position.y > WaveManager.maxY + 0.5f)
            TryAndReturn(false);
    }

    protected virtual void Movement()
    {
        this.transform.Translate(bulletSpeed * Time.deltaTime * direction, Space.World);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity target) && target.health > 0 && !owner.CompareTag(target.tag))
        {
            target.TakeDamage();
            TryAndReturn(true);
        }
        else if (collision.CompareTag("Wall"))
        {
            if (collision.TryGetComponent(out Bullet bullet) && !bullet.owner.CompareTag(this.owner.tag))
                TryAndReturn(false);
            else if (!collision.transform.parent.CompareTag(owner.tag))
                TryAndReturn(false);
        }
    }
}