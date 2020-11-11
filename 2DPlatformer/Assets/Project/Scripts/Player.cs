using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using static KyleConibear.Logger;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SceneChanger))]
public class Player : MonoBehaviour
{
    public bool isLogging = false;

    private Animator animator = null;
    private new Rigidbody2D rigidbody2D = null;
    private BoxCollider2D boxCollider2D = null;
    private SceneChanger sceneChanger = null;

    [Range(5, 25)]
    [SerializeField] private float baseForce = 6;

    [Range(1.25f, 2)]
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Range(5, 25)]
    [SerializeField] private float jumpHeight = 15;

    [SerializeField] private float airMoveMultiplier = 0.5f;

    /// <summary>
    /// Keeps track of whether the player is currently holding the spacebar
    /// </summary>
    private bool isJumping = false;

    [Tooltip("The amount gravity is multiplied by when falling.")]
    [Range(1, 3)]
    [SerializeField] private float fallMultiplier = 2.0f; // This is used for a full held high jump

    [Tooltip("If the player releases the jump button before the jumps peak height gravity will be multiplied to quicken descent.")]
    [Range(1, 2)]
    [SerializeField] private float lowJumpGravityMultiplier = 1.5f; // This is used for a low tap jump

    [Range(0.1f, 1)]
    [SerializeField] private float groundCheckDepth = 0.5f;

    [SerializeField] private LayerMask groundLayerMask;

    private bool isGrounded = false;

    private State state = State.Idle;
    public enum State
    {
        Idle = 0,
        Moving = 1,
        Jumping = 2,
        Falling = 3,
        Dead = 4
    }

    private void Move(float xInput, float sprintMultiplier = 1.0f)
    {
        if (state == State.Dead) // Player has died
            return; // Code below the return will not run

        if (LevelManager.instance.IsXPositionWithinLevel(this.transform.position.x + xInput))
        {
            float xValue = xInput * (baseForce * sprintMultiplier);
            if (!this.isGrounded)
            {
                if ((rigidbody2D.velocity.x > 0 && xInput < 0) || (rigidbody2D.velocity.x < 0 && xInput > 0)) // Reduce opposite direction of travel while airborne
                {
                    xValue *= this.airMoveMultiplier;
                }
            }
            rigidbody2D.velocity = new Vector2(xValue, rigidbody2D.velocity.y);
        }
    }
    private void Jump(bool forced = false) // Passing variable into method
    {
        if (state != State.Dead && this.isGrounded || forced) // or statement ||
        {
            // Debug.Log("Jump");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpHeight);
        }
    }

    /// <summary>
    /// Makes the player fall faster
    /// </summary>
    private void FasterFall()
    {
        // We are jumping up
        if (this.rigidbody2D.velocity.y > 0 && this.isJumping == false) // The spacebar has been released
        {
            this.rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * this.lowJumpGravityMultiplier * Time.deltaTime;
        }
        // Check if character is falling
        else if (this.rigidbody2D.velocity.y < 0)
        {
            Log(this.isLogging, Type.Message, "Character is falling.");
            // Physics2D.gravity.y = -9.81
            this.rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * this.fallMultiplier * Time.deltaTime;
        }
    }

    private void SetState(State state)
    {
        this.state = state;
        this.animator.SetInteger("state", (int)state);

        if (this.state == State.Dead)
        {
            this.rigidbody2D.velocity = new Vector2(0, -1);
            this.rigidbody2D.isKinematic = true; // Fall through floor
            StartCoroutine(DeathDelay());
        }
    }
    private bool IsGrounded()
    {
        bool tmpGrounded = false;

        RaycastHit2D hit = Physics2D.BoxCast(this.boxCollider2D.bounds.center, this.boxCollider2D.bounds.size, 0f, Vector2.down, this.groundCheckDepth, groundLayerMask);
        Color rayColor;

        tmpGrounded = hit.collider != null;

        if (tmpGrounded)
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

        Log(this.isLogging, Type.Message, $"Character {this.name} is grounded = {tmpGrounded}");

        return tmpGrounded;
    }
    private void StateObserver()
    {
        if (state == State.Dead) // Player has died
            return; // Code below the return will not run

        if (this.isGrounded)
        {
            // Set state to update animator
            // Moving to the right
            if (rigidbody2D.velocity.x > 0.1f) // Mathf.Epsilon is a really tiny float
            {
                this.transform.localScale = new Vector3(1, 1, 1);
                SetState(State.Moving);
            }
            // Moving left
            else if (rigidbody2D.velocity.x < -0.1f)
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

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(3);
        this.sceneChanger.SetScene(0); // Return to main menu
    }

    #region MonoBehaviour Methods

    private void Awake()
    {
        this.boxCollider2D = GetComponent<BoxCollider2D>();
        this.animator = GetComponent<Animator>();
        this.rigidbody2D = GetComponent<Rigidbody2D>();
        this.sceneChanger = GetComponent<SceneChanger>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // GetKeyDown happens once. -> KeyCode.Space is the spacebar
        {
            isJumping = true;
            Jump();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }
    private void LateUpdate()
    {
        StateObserver();
        FasterFall();
    }
    private void FixedUpdate()
    {
        this.isGrounded = IsGrounded();

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f) // Abs convets negative number to positive
        {
            if (Input.GetKey(KeyCode.LeftShift))
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
    }

    // This will be called when our player enters a 2DCollider
    private void OnTriggerEnter2D(Collider2D collision) // collision is the other GameObject
    {
        PickUp pickUp = collision.GetComponent<PickUp>();
        // This is a gem
        if (pickUp)
        {
            switch (pickUp.type)
            {
                case PickUp.Type.Gem:
                    LevelManager.instance.IncrementGemCount();
                    break;

                case PickUp.Type.Cherry:
                    LevelManager.instance.IncrementCherryCount();
                    break;
            }
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetComponentInParent<Enemy>())
        {
            // If falling, kill enemy
            if (this.state == State.Falling)
            {
                this.Jump(true);
                Destroy(collision.transform.parent.gameObject);
            }
            // else die
            else
            {
                this.SetState(State.Dead);
            }
        }
    }

    #endregion
}