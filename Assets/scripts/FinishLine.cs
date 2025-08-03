using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                MenuManager.Instance.SetActivePanel("win");

            }
        }

    }
}
