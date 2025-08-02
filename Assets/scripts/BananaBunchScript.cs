using UnityEngine;

public class BananaBunchScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private CapsuleCollider2D capCol2d;

    [SerializeField] private int ammoNum;
    [SerializeField] private float healthNum;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        capCol2d = GetComponent<CapsuleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Banana")
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            capCol2d.isTrigger = false;
        }

        if (collision.gameObject.tag == "Player")
        {
            PlayerStatTracker pst = collision.gameObject.GetComponent<PlayerStatTracker>();

            pst.PlayerAmmo += ammoNum;
            pst.PlayerHealth += healthNum;

            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerStatTracker pst = collision.gameObject.GetComponent<PlayerStatTracker>();

            pst.PlayerAmmo += ammoNum;
            pst.PlayerHealth += healthNum;

            gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            capCol2d.isTrigger = false;
        }
    }

    
}
