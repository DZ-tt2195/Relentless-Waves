using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevel", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public List<Wave<Collection>> listOfWaves = new();
}

[System.Serializable]
public class Wave<Collection>
{
    public List<Collection> enemies = new();
    public string tutorialKey;
}

[System.Serializable]
public class Collection
{
    public BaseEnemy toCreate;
    public Vector2 position;
}
