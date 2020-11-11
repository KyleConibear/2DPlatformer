using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour // Enemy is now base class
{
    [SerializeField] protected GameObject actor = null;
    [SerializeField] protected float movementSpeed = 1.0f;
    [SerializeField] protected Transform[] movePositions = new Transform[0];
    [SerializeField] protected Vector2 movementPauseRange = new Vector3(0.1f, 1);
    private int moveIndex = 0;
    private bool isMovingForward = true;
    private bool canMove = true;

    protected virtual void Move()
    {
        if (canMove)
        {
            // Moving to target
            // Translate = which is changing the transform position value
            this.actor.transform.position = Vector3.MoveTowards(this.actor.transform.position, this.movePositions[moveIndex].position, this.movementSpeed * Time.deltaTime);
        }
    }

    private void LookAtTarget()
    {
        // Moving to the left
        if (this.actor.transform.position.x < this.movePositions[moveIndex].position.x)
        {
            this.actor.transform.localScale = new Vector3(-1, 1, 1);            
        }
        // Moving to the right
        else
        {
            this.actor.transform.localScale = Vector3.one; // New Vector(1,1,1);
        }
    }

    // Coroutine -> Async
    private IEnumerator ToggleCanMove()
    {
        canMove = false;
        yield return new WaitForSeconds(Random.Range(this.movementPauseRange.x, this.movementPauseRange.y)); // Random delay between 0.1 - 0.99 seconds
        canMove = true;
    }

    private void Awake()
    {
        this.actor.transform.position = this.movePositions[0].position;
    }

    // Update is called once per frame
    private void Update()
    {
        this.Move();              
    }
    
    private void LateUpdate()
    {
        // Look at target
        this.LookAtTarget();

        // Check distance to target
        if (Vector3.Distance(this.actor.transform.position, this.movePositions[moveIndex].position) < Mathf.Epsilon)
        {
            ArrivedAtDestination();

            StartCoroutine(ToggleCanMove());

            // Go to next move position
            if (isMovingForward) // True statement
            {
                this.moveIndex++;

                if (this.moveIndex == this.movePositions.Length)
                {
                    isMovingForward = !isMovingForward;
                }
            }

            if (!isMovingForward) // False statement
            {
                this.moveIndex--;
                if (this.moveIndex == -1)
                {
                    isMovingForward = !isMovingForward;
                    this.moveIndex++;
                }
            }
            // this.moveIndex = this.moveIndex % this.movePositions.Length; // If index exceeds count set it to the beginning 0
        }
    }

    protected abstract void ArrivedAtDestination(); // Signature // Within base class has no body or functionality but is required by inherited classes
}
