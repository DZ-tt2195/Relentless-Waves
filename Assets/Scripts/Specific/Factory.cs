using UnityEngine;
using System.Collections.Generic;

public class Factory : BaseEnemy
{
    int numBullets;
    List<(Vector3, Vector3)> positionsToShoot = new();

    protected override void Awake()
    {
        base.Awake();
        Reset();
    }
    
    void Reset()
    {
        numBullets = 10;
        positionsToShoot.Clear();
        for (int i = 0; i<2; i++)
        {
            bool moveRight = Random.Range(0, 2) == 0;
            Vector3 movement = moveRight ? Vector3.right : Vector3.left;
            Vector3 toShoot = new(moveRight ? WaveManager.minX+0.1f : WaveManager.maxX - 0.1f, Random.Range(WaveManager.minY+0.25f, 2f));
            positionsToShoot.Add((movement, toShoot));
        }
    }

    protected override void ShootBullet()
    {
        foreach ((Vector3 movement, Vector3 position) in positionsToShoot)
            CreateBullet(bulletPrefab, position, bulletSpeed, movement);
        
        numBullets--;
        if (numBullets == 0)
            Reset();
    }
}
