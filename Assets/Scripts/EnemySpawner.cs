using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Transform enemyCollector, ground;
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private float spawnTimer;

    private bool canSpawn = true;


    void Start()
    {
        SpawnEnemy(1);
    }

    void Update()
    {
        SpawnEnemyOnTime();
    }

    private void SpawnEnemyOnTime()
    {
        if (!canSpawn) return;

        SpawnEnemy(1);
        StartCoroutine(CanSpawn());
    }

    IEnumerator CanSpawn()
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnTimer);
        canSpawn = true;
    }

    public void SpawnEnemy(int amount)
    {
        Vector3 groundSize = new Vector3();
        try
        {
            groundSize = ground.GetComponent<Renderer>().bounds.size;
        }
        catch(MissingReferenceException ex)
        {
            Debug.Log(ex);
            return;
        }
        

        for (int i = 0; i < amount; i++)
        {
            Vector3 position = new Vector3(Random.Range(groundSize.x / 2, -groundSize.x / 2), 1, Random.Range(groundSize.z / 2, -groundSize.z / 2));
            GameObject e = Instantiate(enemy, position, Quaternion.identity, enemyCollector);
            e.SetActive(true);
        }        
    }
}
