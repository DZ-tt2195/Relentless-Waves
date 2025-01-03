using UnityEngine;

public class KnightBullet : Bullet
{
    protected override void Movement()
    {
        this.transform.Translate(bulletSpeed * Time.deltaTime * Vector3.down);
    }
}
