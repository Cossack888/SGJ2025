using UnityEngine;

public class EnvironmentDamageScript : MonoBehaviour
{
    private bool damaging;

    [SerializeField] private float damageVal;
    [SerializeField] private float timeFactor;

    private GameObject playerObj;
    private PlayerStatTracker playerStats;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            damaging = true;
            playerObj = collision.gameObject;
            playerStats = playerObj.GetComponent<PlayerStatTracker>();
        }
    }

    void Update()
    {
        if (damaging)
        {
            playerStats.PlayerHealth -= damageVal;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            damaging = false;
            playerObj = null;
            playerStats = null;
        }
    }

}
