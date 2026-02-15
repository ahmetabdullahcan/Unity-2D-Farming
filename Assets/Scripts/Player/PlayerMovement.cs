using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody2D rb;
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 movement;

    private void HandleMove()
    {
        movement = playerInput.actions["Move"].ReadValue<Vector2>();
        movement = movement.normalized;
    }

    private void FlipSprite(float horizontal)
    {
        if (horizontal > 0.1f)
            spriteRenderer.flipX = false;
        else if (horizontal < -0.1f)
            spriteRenderer.flipX = true;
    }

    private void HandleAnimation()
    {
        Vector2 actualVelocity = rb.linearVelocity;
        
        if (actualVelocity.magnitude > 0.1f)
        {
            FlipSprite(actualVelocity.x);
            animator.SetFloat("Horizontal", actualVelocity.x);
            animator.SetFloat("Vertical", actualVelocity.y);
            animator.SetFloat("Speed", actualVelocity.sqrMagnitude);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    void Update()
    {
        HandleMove();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * speed;
    }
}