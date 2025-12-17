using UnityEngine;
using System;
using System.Collections.Generic;
using MyBox;

[Serializable]
public class Moon
{
    public GameObject obj;
    public float rot;
    [ReadOnly] public float radius;
}

public class Planet : BaseEnemy
{
    [SerializeField] List<Moon> toSpin = new();
    [SerializeField] float spinSpeed;

    protected override void Awake()
    {
        base.Awake();
        spinSpeed *= PrefManager.GetDifficulty() * (UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
        foreach (Moon next in toSpin)
            next.radius = UnityEngine.Random.Range(1.5f, 3.5f);
    }

    protected override void Update() 
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, moveSpeed*Time.deltaTime);    

        foreach (Moon next in toSpin)
        {
            next.rot += spinSpeed * Mathf.Rad2Deg * Time.deltaTime; 
            float rad = next.rot * Mathf.Deg2Rad; 
            float x = this.transform.position.x + Mathf.Cos(rad) * next.radius; 
            float y = this.transform.position.y + Mathf.Sin(rad) * next.radius; 
            next.obj.transform.position = new Vector3(x, y, 0);    
        }
    }
}
