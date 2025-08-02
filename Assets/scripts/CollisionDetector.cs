using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    PlayerStatTracker playerStatTracker;
    private void Start()
    {
        playerStatTracker = GetComponent<PlayerStatTracker>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("FireWall"))
            {
                playerStatTracker.PlayerHealth -= 100;
            }
        }
    }
}
