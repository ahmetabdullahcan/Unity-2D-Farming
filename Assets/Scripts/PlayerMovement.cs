using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody2D rb;
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Vector2 movement;

    private void HandleMove()
    {
        movement = playerInput.actions["Move"].ReadValue<Vector2>();
        movement = movement.normalized;
        rb.linearVelocity = movement * speed;
    }

    private void FlipSprite(float horizontal)
    {
        if (horizontal > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontal < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleAnimation()
    {
        if (movement != Vector2.zero)
        {
            FlipSprite(movement.x);
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
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
}
