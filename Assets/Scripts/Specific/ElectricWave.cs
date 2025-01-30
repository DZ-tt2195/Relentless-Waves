using UnityEngine;

public class ElectricWave : Bullet
{
    [SerializeField] float targetXpand;
    [SerializeField] float travelTime;
    [SerializeField] float vanishTime;
    SpriteRenderer spriteRenderer;

    float myTimer = 0;
    bool moving = true;

    float invertTime = 0.25f;
    bool inverted = true;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        travelTime *= 2 - PlayerPrefs.GetFloat("Difficulty");
        vanishTime *= 2 - PlayerPrefs.GetFloat("Difficulty");
    }

    public override void AssignInfo(float speed, Vector3 direction, Entity owner)
    {
        base.AssignInfo(speed, direction, owner);
        this.transform.localScale = Vector3.zero;
        this.transform.eulerAngles = new(0, 0, Random.Range(-360f, 360f));

        moving = true;
        myTimer = 0;
        invertTime = 0.25f;
        SetAlpha(spriteRenderer, 1);
    }

    protected override void Movement()
    {
        myTimer += Time.deltaTime;

        if (moving)
        {
            base.Movement();
            this.transform.localScale = Vector2.Lerp(Vector3.zero, new(targetXpand, targetXpand), myTimer / travelTime);

            if (myTimer >= travelTime)
            {
                myTimer = 0f;
                moving = false;
            }
        }
        else
        {
            SetAlpha(spriteRenderer, 1 - (myTimer / vanishTime));
            if (myTimer >= vanishTime)
                TryAndReturn(false);
        }

        invertTime -= Time.deltaTime;
        if (invertTime < 0f)
        {
            inverted = !inverted;
            invertTime = 0.25f;
        }

        float newYScale = Mathf.Abs(this.transform.localScale.y) * (inverted ? -1 : 1);
        this.transform.localScale = new Vector3(this.transform.localScale.x, newYScale);
    }

    void SetAlpha(SpriteRenderer target, float alpha)
    {
        Color newColor = target.color;
        newColor.a = alpha;
        target.color = newColor;
    }
}