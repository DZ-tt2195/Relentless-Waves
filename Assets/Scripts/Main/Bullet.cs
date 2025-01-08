using UnityEngine;
using System;
using MyBox;

public class Bullet : MonoBehaviour
{
    protected Vector3 direction;
    protected Entity owner;
    protected float bulletSpeed;

    void TryAndReturn(bool landed)
    {
        if (owner == null)
            Destroy(this.gameObject);
        else
            owner.ReturnBullet(this, landed);
    }

    public void AssignInfo(float speed, Vector3 direction, Entity owner)
    {
        this.bulletSpeed = speed;
        this.direction = direction;
        this.owner = owner;
    }

    void Update()
    {
        Movement();
        if (this.transform.position.x < WaveManager.minX || this.transform.position.x > WaveManager.maxX ||
            this.transform.position.y < WaveManager.minY || this.transform.position.y > WaveManager.maxY)
            TryAndReturn(false);
    }

    protected virtual void Movement()
    {
        this.transform.Translate(bulletSpeed * Time.deltaTime * direction);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity target) && target.health > 0 && !this.CompareTag(target.tag))
        {
            target.TakeDamage();
            TryAndReturn(true);
        }
        else if (collision.CompareTag("Wall") && !collision.transform.parent.CompareTag(this.tag))
        {
            TryAndReturn(false);
        }
    }
}