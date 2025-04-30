using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 10f;

    public Rigidbody rb;
    public Animator animator;

    private float moveSpeed;
    private bool isGrounded;
    private Vector3 inputDirection;

    void Update()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }

        if (isGrounded)
        {
            moveSpeed = walkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = runSpeed;
            }
        }

        //Debug.Log(animator.pivotWeight);

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (inputDirection != Vector3.zero)
        {
            Vector3 move = inputDirection * moveSpeed;
            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }
    }

    private void UpdateAnimator()
    {
        float speedPercent = 0f;

        if (inputDirection != Vector3.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speedPercent = 1f;
            }
            else
            {
                speedPercent = 0.5f;
            }
        }
        else
        {
            speedPercent = 0f;
        }

        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        { 
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            isGrounded = false;
        }
    }
}
