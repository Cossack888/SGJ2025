using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private float moveSpeed;
    private float moveLoc;

    [SerializeField] private GameObject dart;
    [SerializeField] private GameObject net;
    [SerializeField] private float netForce;
    [SerializeField] private bool netLoaded = true;
    [SerializeField] private GameObject baton;
    [SerializeField] private float contactDmg;
    [SerializeField] private float contactDmgMult;
    [SerializeField] GameObject VFX;
    [SerializeField] private float shootingSpeed;

    [SerializeField] private PlayerStatTracker[] players;
    private Transform player;
    private bool canDamage = true;
    public float damageCooldown = 1f;
    [SerializeField] private List<DartScript> allDarts = new List<DartScript>();

    public enum EnemyState { Alive, Dazed, Dead };
    private enum EnemyAttackState { Shooting, Throwing };

    // time in game from the moment the spawner is triggered
    private float startTime;
    [SerializeField] private bool stopped = false;
    bool shooting = false;

    EnemyState enemyState;
    EnemyAttackState enemyAttackState;

    Vector2 objOrigin;

    public void ActivateEnemy(float moveCoord, Vector2 parentOrigin, float speed)
    {
        // Remove this obj from the spawner parent
        // Note, should still be in the list of spawnable objects.
        transform.SetParent(null);
        VFX = GetComponentInChildren<ParticleSystem>().gameObject;
        VFX.SetActive(false);
        transform.position = parentOrigin;
        moveLoc = moveCoord;
        moveSpeed = speed;

        // once it finds one game object that isn't active, activate it.
        gameObject.SetActive(true);

        objOrigin = new Vector2(transform.position.x, transform.position.y);
        startTime = Time.time;

        players = FindObjectsByType<PlayerStatTracker>(FindObjectsSortMode.None);



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
            else if (distance < 3 && netLoaded)
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
    }

    public void ChangeStateTo(EnemyState state)
    {

        if (state == EnemyState.Dazed)
        {
            enemyState = EnemyState.Dazed;
            VFX.SetActive(true);
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
            Vector2 dartSpawn = new Vector2(transform.position.x - 0.6f, transform.position.y + 0.55f);

            GameObject spawnedDart = Instantiate(dart, dartSpawn, transform.rotation);
            DartScript ds = spawnedDart.GetComponent<DartScript>();
            ds.playerLoc = player.position;
            allDarts.Add(ds);
        }
        else
        {
            if (allDarts != null)
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
    }

    Transform TargetPlayer(PlayerStatTracker[] p)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (PlayerStatTracker t in p)
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
        if (moveSpeed <= 0f)
        {
            Debug.LogWarning("moveSpeed is zero or negative!");

            stopped = true;
            return;
        }

        float duration = (Time.time - startTime) / moveSpeed;

        float newX = Mathf.SmoothStep(objOrigin.x, objOrigin.x + moveLoc, duration);

        if (float.IsNaN(newX))
        {
            Debug.LogError("Calculated position is NaN!");
            return;
        }

        transform.position = new Vector2(newX, objOrigin.y);

        if (Mathf.Approximately(transform.position.x, objOrigin.x + moveLoc))
        {
            stopped = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canDamage) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with " + collision.gameObject.name);

            canDamage = false;
            Invoke(nameof(ResetDamage), damageCooldown);

            ChangeStateTo(EnemyState.Dead);

            if (enemyState == EnemyState.Dazed)
            {
                contactDmgMult = contactDmgMult / 2;
            }

            collision.gameObject.GetComponent<PlayerStatTracker>().PlayerHealth -= contactDmg * contactDmgMult;
        }
    }

    private void ResetDamage()
    {
        canDamage = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (enemyState == EnemyState.Dazed)
        {
            if (collision.gameObject.CompareTag("Fist"))
            {
                Debug.Log("EnemyDead");
                ChangeStateTo(EnemyState.Dead);
            }
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

