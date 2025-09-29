using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float crouchSpeed = 2f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 2f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.1f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float crouchHeight = 2f;
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float crouchTransitionSpeed = 6f;
    private float standingHeight;
    private Vector3 standingCameraPos;
    private Vector3 crouchingCameraPos;
    private Vector3 cameraVelocity = Vector3.zero;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;
    private float originalHeight;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        Cursor.lockState = CursorLockMode.Locked;

        standingHeight = controller.height;
        standingCameraPos = cameraTransform.localPosition;
        crouchingCameraPos = new Vector3(standingCameraPos.x, standingCameraPos.y - 0.6f, standingCameraPos.z);
    }

    void Update()
    {
        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Ground Check
        isGrounded = controller.isGrounded;

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

  
        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Crouch (Toggle or Hold)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Crouch();
        }
        else
        {
            UnCrouch();
        }

        // Toggle crouch
        bool isCrouchInput = Input.GetKey(KeyCode.LeftControl);

        // Smoothly change character height
        float targetHeight = isCrouchInput ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        // Adjust center so you stay grounded
        controller.center = new Vector3(0, controller.height / 2f, 0);

        // Smooth camera position
        Vector3 targetCamPos = isCrouchInput ? crouchingCameraPos : standingCameraPos;
        cameraTransform.localPosition = Vector3.SmoothDamp(cameraTransform.localPosition, targetCamPos, ref cameraVelocity, 0.08f);

        // Update coyote time counter
        if (controller.isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Update jump buffer counter
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Jump logic
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0f; // Reset buffer
        }

        // Apply gravity
        if (velocity.y < 0)
        {
            // Falling — apply extra gravity
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else if (velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Let go of jump — shorter jump
            velocity.y += gravity * lowJumpMultiplier * Time.deltaTime;
        }
        else
        {
            // Normal gravity
            velocity.y += gravity * Time.deltaTime;
        }

        // Apply movement
        controller.Move(velocity * Time.deltaTime);

    }

    void Crouch()
    {
        controller.height = crouchHeight;
        isCrouching = true;
    }

    void UnCrouch()
    {
        controller.height = originalHeight;
        isCrouching = false;
    }
}
