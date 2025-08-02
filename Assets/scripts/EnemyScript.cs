using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyScript : MonoBehaviour
{
    private float moveSpeed;
    private float moveLoc;

    [SerializeField] private GameObject dart;
    [SerializeField] private GameObject net;
    [SerializeField] private float netForce;
    [SerializeField] private bool netLoaded = false;
    [SerializeField] private GameObject baton;

    [SerializeField] private float shootingSpeed;

    [SerializeField] private PlayerInput[] players;
    private Transform player;

    [SerializeField] private List<DartScript> allDarts = new List<DartScript>();

    private enum EnemyState { Alive, Dazed, Dead };
    private enum EnemyAttackState { Shooting, Throwing };

    // time in game from the moment the spawner is triggered
    private float startTime;
    private bool stopped = false;
    bool shooting = false;

    EnemyState enemyState;
    EnemyAttackState enemyAttackState;

    Vector2 objOrigin;

    public void ActivateEnemy(float moveCoord, Vector2 parentOrigin, float speed)
    {
        // Remove this obj from the spawner parent
        // Note, should still be in the list of spawnable objects.
        transform.SetParent(null);

        transform.position = parentOrigin;
        moveLoc = moveCoord;
        moveSpeed = speed;

        // once it finds one game object that isn't active, activate it.
        gameObject.SetActive(true);

        objOrigin = new Vector2(transform.position.x, transform.position.y);
        startTime = Time.time;

        players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);



        enemyState = EnemyState.Alive;
        enemyAttackState = EnemyAttackState.Shooting;
    }

    void Update()
    {
        if (!stopped)
        {
            MoveEnemy();
        }
        else
        {
            player = TargetPlayer(players);
            float distance = Vector2.Distance(player.transform.position, transform.position);

            if (distance < 1)
            {
                if (shooting)
                {
                    CancelInvoke("Shooting");
                }
                BattonAttack();
            }
            else if (distance < 3 && !netLoaded)
            {
                if (shooting)
                {
                    CancelInvoke("Shooting");
                }
                ThrowNet();
            }
            else if (distance >= 3 && !shooting)
            {
                InvokeRepeating("Shooting", 0.5f, shootingSpeed);
                shooting = true;
            }
        }

        if (Input.GetKeyDown("space"))
        {
            ThrowNet();
        }
    }

    void ChangeStateTo(EnemyState state)
    {

        if (state == EnemyState.Dazed)
        {
            enemyState = EnemyState.Dazed;
        }
        else if (state == EnemyState.Dead)
        {
            enemyState = EnemyState.Dead;

            EnemyDies();

            gameObject.SetActive(false);
        }
    }

    private void BattonAttack()
    {
        Debug.Log("Pull out baton and do more damage");
        // Animate pulling out the batom and hitting the enemy. Die.
    }


    private void ThrowNet()
    {
        Debug.Log("Throw the net");

        float angleInDegrees = 135f;

        GameObject spawnedNet = Instantiate(net, transform);
        Rigidbody2D sNet = spawnedNet.GetComponent<Rigidbody2D>();
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        Vector2 forceDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        sNet.AddForce(forceDirection * netForce, ForceMode2D.Impulse); // needs to be in the direction of the player

        netLoaded = false;
    }

    private void Shooting()
    {
        if (allDarts.Count < 10)
        {
            GameObject spawnedDart = Instantiate(dart, transform);
            DartScript ds = spawnedDart.GetComponent<DartScript>();
            ds.playerLoc = player.position;
            allDarts.Add(ds);
        }
        else
        {
            for (int d = 0; d <= allDarts.Count; d++)
            {
                if (!allDarts[d].gameObject.activeSelf)
                {
                    allDarts[d].transform.position = transform.position;
                    allDarts[d].playerLoc = player.position;
                    allDarts[d].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    Transform TargetPlayer(PlayerInput[] p)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (PlayerInput t in p)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    // on activation, should start moving in the direction it was assigned
    private void MoveEnemy()
    {

        float duration = (Time.time - startTime) / moveSpeed;

        // move direction will be +1 for moving right
        // move direction will be 0 for standing still
        // move direction will be -1 for moving left
        transform.position = new Vector2(Mathf.SmoothStep(
            objOrigin.x,
            objOrigin.x + moveLoc, duration),
            objOrigin.y);

        if (transform.position.x == objOrigin.x + moveLoc)
        {
            stopped = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with " + collision.gameObject.name);

        if (collision.gameObject.tag == "Player")
        {
            ChangeStateTo(EnemyState.Dead);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // once hit by a banana logic
        if (collision.gameObject.tag == "Banana")
        {
            ChangeStateTo(EnemyState.Dazed);
        }
    }

    void EnemyDies()
    {
        // logic for the enemy dying AND flying off the screen. 
        // depending on its state, the enemy will deal damage to the player
    }
    
}

    // has a STATE value
        /// 1. Alive and SHooting
        /// 2. Alive and Net Throwing
        /// 3. Alive and Stabbing
        /// 4. Blinded
        /// 5. Dead

        // On finishing its move, start shooting at the player (wherever they are)

        // On player entering a certain range, try to throw a net at them

        // On player entering close range, pull out stun baton and hit the player

        // Death animation on impact with the player
    
