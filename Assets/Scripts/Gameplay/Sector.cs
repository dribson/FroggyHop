using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    [SerializeField] GameObject Platform;
    [SerializeField] List<Collectable> Collectables;
    [SerializeField] float platChance, CollectableChance;
    [SerializeField] [Range(0, 5)] int MaxCollectables;
    [SerializeField] [Range(1, 10)] int MaxCollectableRolls;
    Vector3 randPos;

    private void Start()
    {
        GeneratePlatforms();
        GenerateCollectables();
    }

    void GeneratePlatforms()
    {

        float randX = Random.Range(-3.25f, 1.25f);
        Instantiate(Platform, new Vector3(randX, -2) + transform.position, Quaternion.identity, transform);
        randX = Random.Range(-2.25f, 2.25f);
        Instantiate(Platform, new Vector3(randX, 2) + transform.position, Quaternion.identity, transform);
        if (Random.Range(1, 100) < platChance)
        {
            randX = Random.Range(-3.25f, 1.25f);
            Instantiate(Platform, new Vector3(randX, -1) + transform.position, Quaternion.identity, transform);
        }
        if (Random.Range(1, 100) < platChance)
        {
            randX = Random.Range(-1.25f, 3.25f);
            Instantiate(Platform, new Vector3(randX, 0) + transform.position, Quaternion.identity, transform);
        }
        if (Random.Range(1, 100) < platChance)
        {
            randX = Random.Range(-2.25f, 2.25f);
            Instantiate(Platform, new Vector3(randX, 1) + transform.position, Quaternion.identity, transform);
        }
        if (Random.Range(1, 100) < platChance)
        {
            randX = Random.Range(-1.25f, 3.25f);
            Instantiate(Platform, new Vector3(randX, 3) + transform.position, Quaternion.identity, transform);
        }
    }

    void GenerateCollectables()
    {
        int rand, spawned = 0;
        for(int i = 0; i < MaxCollectableRolls; i++)
        {
            rand = Random.Range(0, 100);
            if(rand <= CollectableChance)
            {
                spawned++;
                Instantiate(Collectables[Random.Range(0, Collectables.Count)], new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f)) + transform.position, Quaternion.identity, transform);
                if (spawned >= MaxCollectables)
                    break;
            }
        }
    }

}
