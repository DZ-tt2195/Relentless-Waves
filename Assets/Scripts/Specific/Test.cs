using UnityEngine;

public class Test : BaseEnemy
{
    protected override void ShootBullet()
    {
        for (int i = 0; i<360; i+=45)
        {
            CreateBullet(bulletPrefab, Player.instance.transform.position, bulletSpeed, new(i, 0));
        }
    }
}
