using UnityEngine;

public class DartScript : MonoBehaviour
{


    public Vector2 playerLoc;
    [SerializeField] private int damageValue;

    [SerializeField] private float bulletSpeed;


    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerLoc, bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStatTracker pst = collision.GetComponent<PlayerStatTracker>();

        if (collision.gameObject.tag == "Player")
        {
            pst.PlayerHealth -= damageValue;
            gameObject.SetActive(false);
            Debug.Log("Hit the player");
        }
        else
        {
            gameObject.SetActive(false);
            Debug.Log("Hit something else lol.");
        }
    }
}
