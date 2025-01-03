using UnityEngine;

public class KnightBullet : Bullet
{
    protected override void Movement()
    {
        this.transform.Translate(speed * Time.deltaTime * Vector3.down);
    }
}
