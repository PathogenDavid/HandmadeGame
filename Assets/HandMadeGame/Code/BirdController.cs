using UnityEngine;

public class BirdController : MonoBehaviour
{
    // mouse look
    public float lookSpeed = 60f;
    public bool invertHorizontalLook;
    public bool invertVerticalLook;

    // movement
    public float moveSpeed = 5f;
    public float fastMoveSpeed = 15f;
    public float moveSmoothing = 0.8f;
    private Vector3 acceleration = Vector3.zero;

    // access to rigidbody and model
    public Rigidbody rb;
    public Transform visuals;

    // utility for stopping tilt & hovering
    public float tiltSmooth = 0.01f;
    public bool hovering = true;

    // Hover animation parameters
    private float hoverAnimation = 0f;
    public float hoverAnimationSpeed = 2.5f;
    public float hoverAnimationMagnitude = (1f / 15f) * 0.5f;
    public float hoverAnimationCutoff = 0.1f;

    void Start()
    {
        //TODO: Properly integrate with UiController
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        // handle movement logic
        float horizontalMove = Mathf.Clamp01(Input.GetAxisRaw("Vertical"));
        Vector3 targetVelocity = visuals.forward * horizontalMove * (Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed);
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
        // update player rotation
        Vector2 look = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")) * lookSpeed * Mathf.Clamp01(Time.deltaTime);
        if (invertHorizontalLook)
            look.x *= -1f;
        if (invertVerticalLook)
            look.y *= -1f;
        visuals.rotation = Quaternion.AngleAxis(look.x, Vector3.up) * Quaternion.AngleAxis(look.y, visuals.right) * visuals.rotation;

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
