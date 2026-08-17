using UnityEngine;
using System.Collections.Generic;

public class GameFiles : MonoBehaviour
{
    [SerializeField] List<Level> listOfLevels;
    [SerializeField] List<BaseEnemy> enemiesToSpawn;
    [SerializeField] List<Rule> listOfRules;
    public static GameFiles inst;

    void Awake()
    {
        inst = this;
    }
    public BaseEnemy RandomEnemy()
    {
        return enemiesToSpawn[Random.Range(0, enemiesToSpawn.Count)];
    }
    public List<Level> AllLevels() => listOfLevels;
    public Level CurrentLevel()
    {
        return listOfLevels[PrefManager.GetCurrentLevel()];
    }
    public List<Rule> AllRules() => listOfRules;    
    public HashSet<Rule> SavedRules()
    {
        HashSet<Rule> toReturn = new();
        for (int i = 0; i<NewCustomizer.numTwists; i++)
        {
            int savedNumber = PrefManager.GetRule(i);
            if (savedNumber != -1)
                toReturn.Add(listOfRules[savedNumber]);
        }
        while (toReturn.Count < NewCustomizer.numTwists)
        {
            int randomNumber = Random.Range(0, listOfRules.Count);
            toReturn.Add(listOfRules[randomNumber]);                
        }
        return toReturn;
    }
}