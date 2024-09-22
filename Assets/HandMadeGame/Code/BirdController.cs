using UnityEngine;

public class BirdController : MonoBehaviour
{
    // rotation limits for flying
    public float speed = 0.5f;

    // movement for flying
    public float moveSpeed = 1.4f;
    public float internalMoveSpeed = 0f;
    public float moveSmoothing = 0.8f;
    private Vector3 zeroVec = Vector3.zero;

    // access to rigidbody and model
    public Rigidbody rb;
    public Transform model;
    public Transform animModel;

    // utility for flying rotation
    private float newX;
    private float newY;

    // utility for stopping tilt & hovering
    public float tiltSmooth = 0.01f;
    private float sinCount = 0f;
    private Vector3 startingPos = Vector3.zero;
    public bool hovering = true;

    // utility for mouse shtuff
    private float lastMousePositionX;
    private float lastMousePositionY;

    void Start()
    {
        //TODO: Properly integrate with UiController
        Cursor.lockState = CursorLockMode.Locked;
        internalMoveSpeed = moveSpeed;
        lastMousePositionX = Input.GetAxisRaw("Mouse Y");
        lastMousePositionY = Input.GetAxisRaw("Mouse X");
    }

    void Update()
    {
        // update flying velocity
        float horizontalMove = Input.GetAxisRaw("Vertical");
        if (horizontalMove < 0) horizontalMove = 0;

        // mousePositionY corresponds to left/right and X to up/down
        // will go back and update variable names at some point

        // find change in mouse position
        float newPositionX = Input.GetAxisRaw("Mouse Y");
        float newPositionY = Input.GetAxisRaw("Mouse X");
        float deltaX = lastMousePositionX - newPositionX;
        float deltaY = lastMousePositionY - newPositionY;
        lastMousePositionX = newPositionX;
        lastMousePositionY = newPositionY;

        float oldX = newX;
        float oldY = newY;
        newX += 1000 * Time.deltaTime * speed * deltaX;
        newY -= 1000 * Time.deltaTime * speed * deltaY;

        float diffX = oldX - newX;
        float diffY = oldY - newY;

        // things I tried to fix the jitter
        // tried using newX directly
        // tried only multiplying rotation if diffX or diffY is over a certain value
        // tried doing rotation in FixedUpdate
        // tried lerping old and new positions, didn't fix it
        // tried using moveRotation on the rigidbody, stopped movement entirely
        // tried moving rigidbody to a child object of the bird
        // tried changing interpolate setting to both interpolate and extrapolate on rigidbody

        // next thing I was going to try was using the rigidbody's angular velocity but it's
        // almost midnight and I'm tired haha

        // update player rotation
        transform.rotation *= Quaternion.AngleAxis(-diffY, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(-diffX, this.gameObject.transform.right);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        // handle movement logic
        Vector3 targetVelocity = transform.forward * horizontalMove * internalMoveSpeed;
        Vector3 curVelocity = rb.velocity;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroVec, moveSmoothing);

        // check if player is hovering
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
#if false
        if (horizontalMove == 0 && rb.velocity.magnitude != 0) {
            Quaternion target = Quaternion.Euler(-15, 0, 0);
            model.rotation = Quaternion.Slerp(model.rotation, target, tiltSmooth);
        } else {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            model.rotation = Quaternion.Slerp(model.rotation, target, tiltSmooth);
        }
#endif
    }
}
