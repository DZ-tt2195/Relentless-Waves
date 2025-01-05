using UnityEngine;

public class MovingCannon : BaseEnemy
{
    protected override void Awake()
    {
        base.Awake();
        moveDirection = Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right;
    }

    protected override void Update()
    {
        base.Update();
        if (this.transform.position.x < WaveManager.minX)
            moveDirection = Vector3.right;
        else if (transform.position.x > WaveManager.maxX)
            moveDirection = Vector3.left;
    }
}
