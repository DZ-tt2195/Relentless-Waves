using UnityEngine;
using System;
using MyBox;

namespace Week1
{
    public class Bullet : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        [ReadOnly] public Vector3 direction;

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

        private void Update()
        {
            this.transform.Translate(direction * Time.deltaTime);
            if (this.transform.position.x < minX || this.transform.position.x > maxX ||
                this.transform.position.y < minY || this.transform.position.y > maxY)
                WaveManager.instance.ReturnBullet(this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Entity target))
                target.TakeDamage(this);
        }
    }
}