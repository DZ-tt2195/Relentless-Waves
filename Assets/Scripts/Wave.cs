using MyBox;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "NewWave", menuName = "ScriptableObjects/Wave")]
public class Wave : ScriptableObject
{
    public List<Vector2> enemySpawns = new();
}