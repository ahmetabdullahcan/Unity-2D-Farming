using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody2D rb;
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [Header("Ui")]
    [SerializeField] private GameObject interactableUI;

    private Vector2 movement;
    bool isMenuOpen = false;

    void Start()
    {
        if (interactableUI != null) 
            interactableUI.SetActive(false);
    }

    void MenuControls()
    {
        if (playerInput.actions["Menu"].WasPerformedThisFrame())
        {
            isMenuOpen = !isMenuOpen;
            if (interactableUI != null) 
                interactableUI.SetActive(isMenuOpen);
        }
    }

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
        MenuControls();
    }
}