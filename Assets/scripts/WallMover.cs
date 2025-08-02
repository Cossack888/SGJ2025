using UnityEngine;

public class FireWallMover : MonoBehaviour
{
    public float speed = 2f;
    public bool isActive = true;

    void Update()
    {
        if (isActive)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }
}