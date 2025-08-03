using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DartScript : MonoBehaviour
{


    public Vector2 playerLoc;
    private Vector2 moveDirection;
    [SerializeField] private int damageValue;

    [SerializeField] private float bulletSpeed;

    void OnEnable()
    {
        playerLoc.y = playerLoc.y + 1;
        moveDirection = (playerLoc - (Vector2)transform.position).normalized;
        RotateObject();

        StartCoroutine("DisableDart");
    }

    void Start()
    {
        playerLoc.y = playerLoc.y + 1;
        moveDirection = (playerLoc - (Vector2)transform.position).normalized;

        RotateObject();

        StartCoroutine("DisableDart");
    }

    private void RotateObject()
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 180f);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(playerLoc);
        transform.Translate(moveDirection * bulletSpeed * Time.deltaTime, Space.World);
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
        else if (collision.gameObject.name == "GrabPoint" ||
            collision.gameObject.tag == "Spawner" || 
            collision.gameObject.tag == "Enemy")
        {

        }
        else
        {
            gameObject.SetActive(false);
            Debug.Log("Hit something else lol." + collision.gameObject.name);
        }
    }

    private IEnumerator DisableDart()
    {
        yield return new WaitForSeconds(6f);

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
