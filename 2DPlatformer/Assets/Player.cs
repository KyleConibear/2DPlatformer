using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody2D = null;
    [SerializeField] private float force = 5;
    [SerializeField] private float xMove = 0;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        xMove = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = new Vector2(xMove * force * Time.deltaTime, 0);
    }
}