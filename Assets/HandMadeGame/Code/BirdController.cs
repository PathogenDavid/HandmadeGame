using UnityEngine;

public class BirdController : MonoBehaviour
{
    // rotation limits for flying
    public float speed = 0.5f;

    // movement for flying
    public float moveSpeed = 1.4f;
    public float moveSmoothing = 0.8f;
    private Vector3 acceleration = Vector3.zero;

    // access to rigidbody and model
    public Rigidbody rb;
    public Transform visuals;

    // utility for flying rotation
    private float newX;
    private float newY;

    // utility for stopping tilt & hovering
    public float tiltSmooth = 0.01f;
    public bool hovering = true;

    // Hover animation parameters
    private float hoverAnimation = 0f;
    public float hoverAnimationSpeed = 2.5f;
    public float hoverAnimationMagnitude = (1f / 15f) * 0.5f;
    public float hoverAnimationCutoff = 0.1f;

    // utility for mouse shtuff
    private float lastMousePositionX;
    private float lastMousePositionY;

    void Start()
    {
        //TODO: Properly integrate with UiController
        Cursor.lockState = CursorLockMode.Locked;
        lastMousePositionX = Input.GetAxisRaw("Mouse Y");
        lastMousePositionY = Input.GetAxisRaw("Mouse X");
    }

    private void FixedUpdate()
    {
        // handle movement logic
        float horizontalMove = Mathf.Clamp01(Input.GetAxisRaw("Vertical"));
        Vector3 targetVelocity = transform.forward * horizontalMove * moveSpeed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref acceleration, moveSmoothing);

        // check if player is hovering
        if (rb.velocity.magnitude < .01)
        {
            hovering = true;
            //rb.velocity = Vector3.zero;
        }
        else
        {
            hovering = false;
        }
    }

    private void Update()
    {
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

        // apply hover animation when player is hovering in place
        {
            hoverAnimation = (hoverAnimation + hoverAnimationSpeed * Time.deltaTime) % (Mathf.PI * 2f);
            float speedMagnitude = Mathf.Clamp01((hoverAnimationCutoff - rb.velocity.magnitude) / hoverAnimationCutoff);
            float hover = Mathf.Sin(hoverAnimation) * hoverAnimationMagnitude * speedMagnitude;
            visuals.transform.localPosition = new Vector3(0f, hover, 0f);
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
