using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 10f, -10f);
    public float followSpeed = 5f;

    [Header("References")]
    public MapGeneration map;
    [Header("Camera Bounds (World Space)")]
    public float xscale;
    public float yscale;
    public float maxX =30f ;
    public float maxY = 30f;
    
    private void LateUpdate()
    {
        maxX = (xscale * map.Width);
        maxY = (yscale * map.Height);
        Debug.Log(maxX);
        Debug.Log(maxY);
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float clampedX = Mathf.Clamp(desiredPosition.x,(11.46f), maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y, 6.39f, maxY);

        Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        transform.position = Vector3.Lerp(transform.position,clampedPosition,followSpeed * Time.deltaTime
        );
    }
}