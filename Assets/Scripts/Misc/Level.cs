using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevel", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public List<Wave<Collection>> listOfWaves = new();
    public ToTranslate levelName;
    public bool endless;
}

[System.Serializable]
public class Wave<Collection>
{
    public List<Collection> enemies = new();
    public ToTranslate tutorialKey;
}

[System.Serializable]
public class Collection
{
    public BaseEnemy toCreate;
    public Vector2 position;
}
