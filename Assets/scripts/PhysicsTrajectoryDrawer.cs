using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BananaTrajectoryPreview : MonoBehaviour
{
    public Transform throwOrigin;             // Start rzutu (np. rêka ma³py)
    public float throwForce = 10f;            // Si³a rzutu
    public float gravityScale = 1f;           // Skala grawitacji (jak w RigidBody2D)
    public int trajectoryPoints = 30;         // Iloœæ punktów w linii
    public float timeStep = 0.1f;             // Czas miêdzy punktami

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - (Vector2)throwOrigin.position).normalized;

        if (direction.magnitude > 0.2f)
        {
            ShowTrajectory(direction);
        }
        else
        {
            HideTrajectory();
        }
    }

    void ShowTrajectory(Vector2 direction)
    {
        lineRenderer.positionCount = trajectoryPoints;

        Vector2 startPos = throwOrigin.position;
        Vector2 velocity = direction * throwForce;
        Vector2 gravity = Physics2D.gravity * gravityScale;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector2 displacement = velocity * t + 0.5f * gravity * t * t;
            Vector2 point = startPos + displacement;
            lineRenderer.SetPosition(i, new Vector3(point.x, point.y, 0f));
        }
    }

    void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}
