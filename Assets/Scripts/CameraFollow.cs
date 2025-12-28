using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // target (player)
    public Vector3 offset;

    void LateUpdate()
    {
        if (target != null)
        {
            //  follow the target 
            transform.position = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                transform.position.z
            );
        }
    }
}