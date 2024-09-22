using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public BirdController bird;
    public Transform target;
    public float smoothTime = 0.3f;
    public float lookSmoothTime = 4f;
    public Vector3 refVec = new Vector3(0, 0.5f, -1);
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.TransformPoint(refVec);

        //TODO: This won't work in the real level and should be replaced by camera physics if we have time
        //if (targetPosition.y <= 0) targetPosition = new Vector3(targetPosition.x, 0.2f, targetPosition.z);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        //if (!bird.hovering)
        {
            Vector3 lookDirection = target.position - this.transform.position;
            lookDirection.Normalize();
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(lookDirection), lookSmoothTime * Time.deltaTime);
        }
    }
}
