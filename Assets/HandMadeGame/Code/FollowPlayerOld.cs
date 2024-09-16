using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerOld : MonoBehaviour
{
    public BirdControllerOld bird;
    public Transform target;
    public float smoothTime = 0.3f;
    public float lookSmoothTime = 0.1f;
    private Vector3 refVec = new Vector3(0, 0.5f, -1);
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.TransformPoint(refVec);
        if (targetPosition.y <= 0) targetPosition = new Vector3(targetPosition.x, 0.2f, targetPosition.z);

        // Smoothly move the camera towards that target position
        if (!bird.GetHovering()){
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    void LateUpdate()
    {
        if (!bird.GetHovering()) {
            Vector3 lookDirection = target.position - this.transform.position;
            lookDirection.Normalize();
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(lookDirection), lookSmoothTime * Time.deltaTime);
        }
    }
}
