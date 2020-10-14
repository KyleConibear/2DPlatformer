using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using static KyleConibear.Logger;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public bool isLogging = false;

    private Animator animator = null;
    private Rigidbody2D rigidbody2D = null;
    private BoxCollider2D boxCollider2D = null;

    [Range(5, 25)]
    [SerializeField] private float baseForce = 6;

    [Range(1.25f, 2)]
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Range(5, 25)]
    [SerializeField] private float jumpHeight = 15;

    [Range(0.1f, 1)]
    [SerializeField] private float groundCheckDepth = 0.5f;

    [SerializeField] private LayerMask groundLayerMask;

    private State state = State.Idle;
    public enum State
    {
        Idle = 0,
        Moving = 1,
        Jumping = 2,
        Falling = 3
    }

    private void Move(float xInput, float sprintMultiplier = 1.0f)
    {
        // Move character
        rigidbody2D.velocity = new Vector2((xInput * (baseForce * sprintMultiplier)), rigidbody2D.velocity.y);
    }
    private void Jump() // Passing variable into method
    {
        if (this.IsGrounded())
        {
            Debug.Log("Jump");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpHeight);
        }
    }
    private void SetState(State state)
    {
        this.state = state;
        this.animator.SetInteger("state", (int)state);
    }
    private bool IsGrounded()
    {
        bool isGrounded = false;

        RaycastHit2D hit = Physics2D.BoxCast(this.boxCollider2D.bounds.center, this.boxCollider2D.bounds.size, 0f, Vector2.down, this.groundCheckDepth, groundLayerMask);
        Color rayColor;

        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            rayColor = Color.green;

            Log(this.isLogging, Type.Message, $"Character {this.name} ground collider hit {hit.collider}.");
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(this.boxCollider2D.bounds.center + new Vector3(this.boxCollider2D.bounds.extents.x, 0), Vector2.down * (this.boxCollider2D.bounds.extents.y + this.groundCheckDepth), rayColor);
        Debug.DrawRay(this.boxCollider2D.bounds.center - new Vector3(this.boxCollider2D.bounds.extents.x, 0), Vector2.down * (this.boxCollider2D.bounds.extents.y + this.groundCheckDepth), rayColor);
        Debug.DrawRay(this.boxCollider2D.bounds.center - new Vector3(this.boxCollider2D.bounds.extents.x, this.boxCollider2D.bounds.extents.y + this.groundCheckDepth), Vector2.right * (this.boxCollider2D.bounds.extents.x * 2f), rayColor);

        Log(this.isLogging, Type.Message, $"Character {this.name} is grounded = {isGrounded}");

        return isGrounded;
    }
    private void StateObserver()
    {
        if (this.IsGrounded())
        {
            // Set state to update animator
            // Moving to the right
            if (rigidbody2D.velocity.x > 0) // Mathf.Epsilon is a really tiny float
            {
                this.transform.localScale = new Vector3(1, 1, 1);
                SetState(State.Moving);
            }
            // Moving left
            else if (rigidbody2D.velocity.x < 0)
            {
                this.transform.localScale = new Vector3(-1, 1, 1);
                SetState(State.Moving);
            }
            // Idle
            else
            {
                SetState(State.Idle);
            }
        }
        else
        {
            if (this.rigidbody2D.velocity.y > Mathf.Epsilon)
            {
                SetState(State.Jumping);
            }
            else
            {
                SetState(State.Falling);
            }
        }
    }

    #region MonoBehaviour Methods

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // GetKeyDown happens once. -> KeyCode.Space is the spacebar
        {
            Jump();
        }
    }
    private void LateUpdate()
    {
        StateObserver();
    }
    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            this.Move(Input.GetAxis("Horizontal"), sprintMultiplier);
            animator.SetFloat("sprintMultiplier", sprintMultiplier);
        }
        else
        {
            this.Move(Input.GetAxis("Horizontal")); // sprintMultiplier == 1.0f
            animator.SetFloat("sprintMultiplier", 1.0f);
        }        
    }

    #endregion
}