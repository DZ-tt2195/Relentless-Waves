using UnityEngine;

public class Orb : Bullet
{
    protected override void TryAndReturn(bool landed)
    {
        this.gameObject.SetActive(false);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player target))
        {
            this.owner.TakeDamage();
            TryAndReturn(true);
        }
        else if (collision.CompareTag("Wall") && !collision.transform.parent.CompareTag(this.tag))
        {
            TryAndReturn(false);
        }
    }
}
