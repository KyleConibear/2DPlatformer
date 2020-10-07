using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rigidbody2D = null;

    [Range(100, 250)]
    [SerializeField] private float force = 150;

    [Range(250, 1000)]
    [SerializeField] private float jumpHeight = 500;

    private void Jump() // Passing variable into method
    {
        Debug.Log("Jump");
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, (jumpHeight * Time.deltaTime));
    }

    private void Move(float xInput)
    {   
        rigidbody2D.velocity = new Vector2((xInput * force * Time.deltaTime), rigidbody2D.velocity.y);
    }

    // Opening Tag
    #region MonoBehaviour Methods

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // GetKeyDown happens once. -> KeyCode.Space is the spacebar
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        this.Move(Input.GetAxis("Horizontal"));
    }

    // Closing Tag
    #endregion
}