using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemyScript> enemiesList = new List<EnemyScript>();
    [SerializeField] List<float> numbers;

    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float enemySpeed;
    [SerializeField] private float enemySpawnRate;

    [SerializeField] private float timeBeforeFirstSpawn = 0f;

    [SerializeField] private bool isSpawning = false;
    [SerializeField] private bool hasFinishedSpawning = false;

    private Vector2 spawnPoint;
    private int numSpawned = 0;

    void Start()
    {
        // register the x and y of the spawner
        spawnPoint = new Vector2(transform.position.x, transform.position.y);

        // generate the list of spawnpoints based on the min and max distance
        numbers = GenerateIntegerList(minDistance, maxDistance);
    }

    // generate integer list to determine range of movement for the enemies.
    List<float> GenerateIntegerList(float minFloat, float maxFloat)
    {
        List<float> result = new List<float>();

        int minInt =Mathf.RoundToInt(minFloat);
        int maxInt =Mathf.RoundToInt(maxFloat);

        for (int i = minInt; i <= maxFloat; i++)
        {
            result.Add(i);
        }

        return result;
    }

    void Update()
    {
        // FOR TEST PURPOSES
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ActivateSpawner();
            
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("Collision happened with " + collision.gameObject.name);
        // Only trigger the trigger, if you're not already spawning and not finished
        if (collision.gameObject.tag == "MainCamera" && !isSpawning && !hasFinishedSpawning)
        {
            ActivateSpawner();
            // start spawning after X amount of time
            // default set to IMMEDIATELY
        }
    }

    void ActivateSpawner()
    {
        // Check the number of enemies that need to be spawned
        if (enemiesList.Count <= enemiesToSpawn)
        {
            enemiesToSpawn = enemiesList.Count;
        }

        InvokeRepeating("SpawnEnemies", timeBeforeFirstSpawn, enemySpawnRate);
    }

    // The reason for using a Coroutine is so that once it completes, it won't retrigger
    // and if it's paused, it can start again where it left off.
    // it can also be triggered multiple times.
    void SpawnEnemies()
    {
        for (int i = 0; i <= enemiesToSpawn; i++)
        {
            if (!enemiesList[i].gameObject.activeSelf && numbers.Count > 0)
            {
                float finalMoveCoord;

                if (numbers.Count > 0)
                {
                    // determine which spawn point to use
                    int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
                    finalMoveCoord = numbers[randomIndex];

                    numbers.RemoveAt(randomIndex); // remove the number from possible spawn locations
                }
                else
                {
                    break;
                }

                enemiesList[i].ActivateEnemy(finalMoveCoord, spawnPoint, enemySpeed);

                numSpawned++;

                break; //stop the loop once it has found an object to 'spawn'
            }
        }
        
        // stop spawning if the number of enemies spawned is reached, or if you run out of coords to walk to
        if (enemiesToSpawn == numSpawned || numbers.Count == 0)
        {
            CancelInvoke("SpawnEnemies");
        }
    }


    // Exists but is inert during gameplay.

    // once triggered by the player's Camera trigger box, start its routine

    // Spawn # of enemies based on VARIABLE 1
    // spawning should be at a VARIABLE 4 frequency in seconds
    // Spawning is simply activating an existing child Enemy object
    // List of Enemy objects is called VARIABLE 5

    // Reveal VARIABLE 2 to enemies so they know how far they can walk

    // Reveal VARIABLE 3 to enemies so they know which directions they can walk
}
