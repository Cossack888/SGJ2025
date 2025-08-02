using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform background;
        public float parallaxFactor;
    }

    public ParallaxLayer[] layers;
    public Camera targetCamera;

    private Vector3 previousCameraPosition;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        previousCameraPosition = targetCamera.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = targetCamera.transform.position - previousCameraPosition;

        foreach (var layer in layers)
        {
            Vector3 backgroundTargetPos = layer.background.position + cameraDelta * layer.parallaxFactor;
            layer.background.position = new Vector3(backgroundTargetPos.x, layer.background.position.y, layer.background.position.z);
        }

        previousCameraPosition = targetCamera.transform.position;
    }
}
