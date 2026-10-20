using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    private float turnSpeed = 150f;
    public float doubleJumpStrength = 4f;
    public float SpeedMultiplier { get; set; } = 1f;
    public float minJumpStrength = 5f;
    public float maxJumpStrength = 10f;

    public float maxChargeTime = 3f;

    private float requestedJumpStrength;
    private float chargeTime = 0f;
    private bool isCharging = false;
    private bool runRequested = false;
    private bool jumpRequested = false;
    private bool doubleJumpRequested = false;
    private bool hasDoubleJumped = false;

    [SerializeField] private float groundDistance = 1.2f;
    [SerializeField] private LayerMask groundMask;

    private bool isGrounded;
    public bool IsGrounded => isGrounded;

    private Rigidbody rb;// the physics body we move
    private Vector3 moveDirection;      // worked out from input in Update()
    private float turnInput;


    public float fallThreshold = -10f;
    private bool restarting = false;
    


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            RestartScene();
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);

        // Back on the ground: the double jump is available again
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        // Movement input
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right turn input
        float moveZ = Input.GetAxisRaw("Vertical");  // W/S or Up/Down forward/backward

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        // Rotate the player transform in Update for responsive turning visuals
        // Calculate forward movement relative to current facing direction
        moveDirection = cameraForward * moveZ + cameraRight * moveX;

        if (moveDirection.magnitude > 0.0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                turnSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                isCharging = true;
                chargeTime = 0f;
            }
            else
            {
                TryDoubleJump();
            }
        }

        // Jump when Space is released or charge reaches 3 seconds
         if(Input.GetButton("Jump")){
            // Increase charge time while the button is held
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);
        }

        if (Input.GetButtonUp("Jump") && isCharging) // If true
        {
            isCharging = false;

            requestedJumpStrength = Mathf.Lerp(minJumpStrength, maxJumpStrength, chargeTime / maxChargeTime);

            jumpRequested = true;
            chargeTime = 0f;
        }

        //run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            runRequested = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            runRequested = false;
        }
    }

    private void FixedUpdate(){
        //player actually moving 
        // Movement logic
        MovePlayer();

        //Jump
        if (jumpRequested)
        {
            Jump(requestedJumpStrength);
            jumpRequested = false;
        }
 
        //the second jump, in mid-air
        if (doubleJumpRequested)
        {
            Jump(doubleJumpStrength);
            doubleJumpRequested = false;
        }
    }

    private void MovePlayer()
    {
        // Calculate velocity based on current forward vector
        Vector3 targetVelocity = moveDirection * movementSpeed * SpeedMultiplier;

        // Apply movement while preserving vertical velocity (gravity/jumping)
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void Jump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        jumpRequested = false;
        // rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
    }

    private bool TryDoubleJump()
    {
        // Not allowed on the ground, or if it was already used
        if (isGrounded || hasDoubleJumped) return false;
 
        doubleJumpRequested = true;
        hasDoubleJumped = true;
        return true;
    }

    void RestartScene()
    {
        if (!restarting)
            restarting = true;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
