using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;               // اسحب Main Camera هون
    public Animator animator;

    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float rotationSpeed = 15f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    Vector3 velocity;

    public bool hasKey = false;
    public GameObject ImageKey;

    [Header("إعدادات أصوات الخطوات")]
    public AudioSource audioSource;
    public AudioClip footstepSound;
    public float stepInterval = 0.45f;
    float stepTimer;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (ImageKey != null)
            ImageKey.SetActive(hasKey);

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // اتجاه الحركة نسبةً للكاميرا
        Vector3 camF = cam.forward; camF.y = 0f; camF.Normalize();
        Vector3 camR = cam.right; camR.y = 0f; camR.Normalize();
        Vector3 move = Vector3.ClampMagnitude(camR * x + camF * z, 1f);

        bool isMoving = move.sqrMagnitude > 0.01f;
        bool isSprinting = isMoving && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // اللاعب بيلف باتجاه حركته
        if (isMoving)
        {
            Quaternion target = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

        // خطوات
        if (isMoving && controller.isGrounded)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                if (footstepSound != null && audioSource != null)
                    audioSource.PlayOneShot(footstepSound);
                stepTimer = isSprinting ? stepInterval * 0.6f : stepInterval;
            }
        }
        else stepTimer = 0f;

        // Animator
        animator.SetBool("isWalk", isMoving);

        // قفز
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        controller.Move((move * currentSpeed + velocity) * Time.deltaTime);
    }
}