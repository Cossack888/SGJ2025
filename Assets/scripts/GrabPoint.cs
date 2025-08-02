using UnityEngine;

public class GrabPoint : MonoBehaviour
{
    public BigMonkeyController monkeyController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            monkeyController.NotifyEnteredGrabZone(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            monkeyController.NotifyExitedGrabZone(other);
        }
    }
}