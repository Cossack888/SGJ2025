using System.Collections;
using UnityEngine;

public class NetScript : MonoBehaviour
{
    [SerializeField] private float holdTimeVar;

    void OnTriggerEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerStatTracker pst = collision.gameObject.GetComponent<PlayerStatTracker>();

            pst.PlayerSpeed = 0;
            // trigger minigame

            StartCoroutine(ReleaseAfterTime(holdTimeVar, pst));
        }
    }

    IEnumerator ReleaseAfterTime(float holdTime, PlayerStatTracker tracker)
    {
        yield return new WaitForSeconds(holdTime);

        tracker.PlayerSpeed = tracker.ResetSpeed;

        gameObject.SetActive(false);
    }
}
