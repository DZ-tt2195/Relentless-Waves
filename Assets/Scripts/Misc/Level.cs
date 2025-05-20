using MyBox;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "NewLevel", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public List<Wave<Collection>> listOfWaves = new();
}

[System.Serializable]
public class Wave<T>
{
    public List<T> enemies = new();
}

[System.Serializable]
public class Collection
{
    public BaseEnemy toCreate;
    public Vector2 position;
}
