using UnityEngine;
using System;
using MyBox;

public class Bullet : MonoBehaviour
{
    protected Vector3 direction;
    protected Entity owner;
    protected float speed;

    float minX;
    float maxX;
    float minY;
    float maxY;

    private void Awake()
    {
        float cameraHeight = 2f * Player.instance.mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * Player.instance.mainCamera.aspect;

        minX = Player.instance.mainCamera.transform.position.x - cameraWidth / 2f;
        maxX = Player.instance.mainCamera.transform.position.x + cameraWidth / 2f;
        minY = Player.instance.mainCamera.transform.position.y - cameraHeight / 2f;
        maxY = Player.instance.mainCamera.transform.position.y + cameraHeight / 2f;
    }

    void TryAndReturn()
    {
        if (owner == null)
            Destroy(this.gameObject);
        else
            owner.ReturnBullet(this);
    }

    public void AssignInfo(float speed, Vector3 direction, Entity owner)
    {
        this.speed = speed;
        this.direction = direction;
        this.owner = owner;
    }

    void Update()
    {
        Movement();
        if (this.transform.position.x < minX || this.transform.position.x > maxX ||
            this.transform.position.y < minY || this.transform.position.y > maxY)
            TryAndReturn();
    }

    protected virtual void Movement()
    {
        this.transform.Translate(direction * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity target) && target.health > 0 && !this.CompareTag(target.tag))
        {
            target.TakeDamage();
            TryAndReturn();
        }
    }
}