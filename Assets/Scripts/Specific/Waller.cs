using UnityEngine;

public class Waller : BaseEnemy
{
    GameObject wall;
    [SerializeField] float wallTimer;

    protected override void Awake()
    {
        base.Awake();
        wall = transform.Find("Wall").gameObject;
        wallTimer *= PlayerPrefs.GetFloat("Difficulty");
        InvokeRepeating(nameof(FlickerWall), wallTimer*0.5f, wallTimer);
    }

    void FlickerWall()
    {
        wall.SetActive(!wall.activeSelf);
    }
}
