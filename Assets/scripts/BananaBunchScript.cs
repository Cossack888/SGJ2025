using UnityEngine;

public class BananaBunchScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private CapsuleCollider2D capCol2d;

    [SerializeField] private int ammoNum;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        capCol2d = GetComponent<CapsuleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Banana")
        {
            rb2d.simulated = true;
            capCol2d.isTrigger = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            rb2d.simulated = true;
            capCol2d.isTrigger = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // add ammo to the amo counter of the monkeys
            Debug.Log("Adding " + ammoNum + " amount of ammo to the smoll monkey.");

            gameObject.SetActive(false);
        }
    }
}
