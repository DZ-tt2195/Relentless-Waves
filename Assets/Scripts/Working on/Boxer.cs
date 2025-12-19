using UnityEngine;

public class Boxer : BaseEnemy
{
    Vector2 mostRecent;
    bool attacking = false;

    protected override void Awake()
    {
        base.Awake();
        DoneAttacking();
    }

    void DoneAttacking()
    {
        attacking = false;
        Invoke(nameof(Charge), attackRate);        
    }

    void Charge()
    {
        attacking = true;
    }

    protected override void Update()
    {
        if (attacking)
        {
            this.transform.Translate(moveSpeed * Time.deltaTime * mostRecent, Space.World);
            if (this.transform.position.x < WaveManager.minX || transform.position.x > WaveManager.maxX || transform.position.y < WaveManager.minY || transform.position.y > WaveManager.maxY)
                DoneAttacking();
        }
        else
        {
            base.Update();
            mostRecent = AimAtPlayer();
        }
    }
}
