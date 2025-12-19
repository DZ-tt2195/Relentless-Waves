using System.Collections.Generic;
using UnityEngine;

public class SpinningZones : MonoBehaviour
{
    [SerializeField] List<GameObject> listOfWalls = new();
    [SerializeField] float rotateSpeed;

    void Awake()
    {
        rotateSpeed *= PrefManager.GetDifficulty() * Random.Range(0, 2) == 0 ? -1 : 1;
        listOfWalls[Random.Range(0, listOfWalls.Count)].SetActive(false);
    }

    void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, rotateSpeed)*Time.deltaTime);
    }
}
