using System.Collections.Generic;
using UnityEngine;

public class Snowflake : BaseEnemy
{
    protected override void ShootBullet()
    {
        Vector2 target = AimAtPlayer();
        target.Normalize();

        for (float i = -7f; i <= 7f; i += 1.75f)
        {
            float transformX = this.transform.position.x + i;
            CreateBullet(bulletPrefab, new Vector3(transformX, WaveManager.maxY), bulletSpeed, target);
        }
    }
}
