using System.Collections;
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

            StartCoroutine("DamageOverTime");
        }
    }

    IEnumerator DamageOverTime()
    {
        damaging = true;

        while (damaging)
        {
            DealDamage();
            yield return new WaitForSeconds(timeFactor);
        }
    }

    void DealDamage()
    {
        playerStats.PlayerHealth -= damageVal;
            Debug.Log("Current Health is " + playerStats.PlayerHealth);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StopCoroutine("DamageOverTime");

            damaging = false;
            playerObj = null;
            playerStats = null;

            
        }
    }

}
