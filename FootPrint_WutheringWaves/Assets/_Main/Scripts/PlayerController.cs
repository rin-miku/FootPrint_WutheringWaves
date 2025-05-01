using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float rotationSmoothTime = 0.1f;

    public Rigidbody rb;
    public Animator animator;

    private Vector3 inputDirection;
    private float currentVelocity;

    void Update()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

        if (inputDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (inputDirection != Vector3.zero)
        {
            Vector3 move = inputDirection * walkSpeed;
            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }
    }

    private void UpdateAnimator()
    {
        float speedPercent = 0f;

        speedPercent = inputDirection != Vector3.zero ? 0.5f : 0f;

        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    }
}
