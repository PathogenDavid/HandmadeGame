using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdControllerOld : MonoBehaviour
{
    // rotation limits for flying
    public float minX = 60f;
    public float maxX = 120f;
    public float speed = .5f;

    // speed limits for birds
    public float lowSpeedLimit = 0.6f;
    public float highSpeedLimit = 1.4f;
    public float speedAdjustment = 0.01f;

    // movement for flying
    public float moveSpeed = 10f;
    public float internalMoveSpeed;
    public float moveSmoothing = 0.05f;
    private Vector3 zeroVec = Vector3.zero;

    // access to rigidbody and model
    public Rigidbody rb;
    public Transform model;
    public Transform animModel;

    // utility for flying rotation
    private float change;
    private float smooth;
    private float mouse;
    private float newX;
    private float newY;

    // utility for flying speed
    private float horizontalMove = 0.0f;

    // utility for stopping tilt & hovering
    public float tiltSmooth = 0.3f;
    private float sinCount = 0f;
    private Vector3 startingPos = Vector3.zero;
    public bool hovering = true;

    private bool Suppressed;

    // Start is called before the first frame update
    void Start()
    {
        internalMoveSpeed = moveSpeed;

        UiController.UiInteractionStart += () =>
        {
            Suppressed = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        };

        UiController.UiInteractionEnd += () =>
        {
            Suppressed = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Suppressed)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // update flying velocity
        horizontalMove = Input.GetAxisRaw("Vertical");
        if (horizontalMove < 0) horizontalMove = 0;

        // check if mouse is far enough from center to look up/down
        if (Input.mousePosition.y < Screen.height / 4) {
            newX += 2 * speed * (Mathf.Abs(Input.mousePosition.x - Screen.height / 2) / Screen.height);
            internalMoveSpeed = Mathf.Clamp(internalMoveSpeed + (horizontalMove * speedAdjustment), lowSpeedLimit, highSpeedLimit);
        }
        else if (Input.mousePosition.y > 3 * Screen.height / 4) {
            newX -= 2 * speed * (Mathf.Abs(Input.mousePosition.x - Screen.height / 2) / Screen.height);
            internalMoveSpeed = Mathf.Clamp(internalMoveSpeed - (horizontalMove * speedAdjustment), lowSpeedLimit, highSpeedLimit);
        }

        // check if mouse is far enough from center to look left/right
        if (Input.mousePosition.x < 8 * Screen.width / 20) {
            newY -= speed * 2 * (Mathf.Abs(Input.mousePosition.x - Screen.width / 2) / Screen.width);
            horizontalMove = 1; // force player to fly forward to turn
        }
        else if (Input.mousePosition.x > 12 * Screen.width / 20) {
            newY += speed * 2 * (Mathf.Abs(Input.mousePosition.x - Screen.width / 2) / Screen.width);
            horizontalMove = 1; // force player to fly forward to turn
        }

        // hide cursor again when clicking into game again
        // WILL NEED TO CHANGE THIS! to show cursor when decorating nest
        //if (Input.GetMouseButtonDown(0)) Cursor.visible = false;
        
        // avoid big scary numbers
        if (newX < -360) newX = 360 + newX;
        if (newY < 0) newY = 360 + newY;
        if (newY > 360) newY = newY - 360;

        // clamp x rotation
        newX = Mathf.Clamp(newX, minX, maxX);

        // update player rotation
        transform.rotation = Quaternion.Euler(newX, newY, transform.rotation.z);
        animModel.rotation = Quaternion.Euler(newX, newY, animModel.rotation.z);

        // handle movement logic
        Vector3 targetVelocity = transform.forward * horizontalMove * internalMoveSpeed;
        Vector3 curVelocity = rb.velocity;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroVec, moveSmoothing);
        // make numbers nicer
        if (rb.velocity.magnitude < .01){
            if (startingPos == Vector3.zero) {
                startingPos = this.transform.position;
            }
            hovering = true;
            rb.velocity = Vector3.zero;
            sinCount = sinCount + 0.01f;
            this.transform.position = new Vector3(startingPos.x, startingPos.y + (Mathf.Sin(sinCount) / 15), startingPos.z);
        } else {
            sinCount = 0;
            startingPos = Vector3.zero;
            hovering = false;
        }
        // check if decelerating to tilt model down
        if (horizontalMove == 0 && rb.velocity.magnitude != 0) {
            Quaternion target = Quaternion.Euler(-15, 0, 0);
            model.rotation = Quaternion.Slerp(model.rotation, target, tiltSmooth);
        } else {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            model.rotation = Quaternion.Slerp(model.rotation, target, tiltSmooth);
        }
    }

    public bool GetHovering() {
        return hovering;
    }
}
