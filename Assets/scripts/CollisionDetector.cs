using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    PlayerStatTracker playerStatTracker;
    BigMonkeyController bigMonkeyController;
    SmallMonkeyController smallMonkeyController;
    MinigameController minigameController;
    private void Start()
    {
        playerStatTracker = GetComponent<PlayerStatTracker>();
        bigMonkeyController = GetComponentInChildren<BigMonkeyController>();
        smallMonkeyController = GetComponentInChildren<SmallMonkeyController>();
        minigameController = FindFirstObjectByType<MinigameController>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("FireWall"))
            {
                playerStatTracker.PlayerHealth -= 100;
            }
            if (collision.gameObject.CompareTag("BananaBunch"))
            {
                BananaBunchScript bunchScript = collision.GetComponent<BananaBunchScript>();
                if (bunchScript != null)
                {
                    playerStatTracker.PlayerHealth += bunchScript.healthNum;
                }
                LevelManager.Instance.GainPoints(1);
            }
            if (collision.gameObject.CompareTag("Net"))
            {
                bigMonkeyController.SetFree(false);
                smallMonkeyController.SetFree(false);
                minigameController.ActivateGame();
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            bigMonkeyController.SetFree(false);
            smallMonkeyController.SetFree(false);
            minigameController.ActivateGame();
        }
    }
}
