using UnityEngine;

public class ExpandRay : Bullet
{
    [SerializeField] float targetXpand;
    [SerializeField] float travelTime;
    float myTimer = 0;

    protected override void Awake()
    {
        base.Awake();
        travelTime *= 2 - PlayerPrefs.GetFloat("Difficulty");
    }

    public override void AssignInfo(float speed, Vector3 direction, Entity owner)
    {
        base.AssignInfo(speed, direction, owner);
        this.transform.localScale = Vector3.zero;
        myTimer = 0f;
    }

    protected override void Movement()
    {
        myTimer += Time.deltaTime;
        this.transform.localScale = Vector2.Lerp(Vector3.zero, new(targetXpand, targetXpand), myTimer / travelTime);
        if (myTimer >= travelTime)
            TryAndReturn(false);
    }
}