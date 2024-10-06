using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody2D rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
 
        moveInput = Input.GetAxis("Horizontal");

        Debug.Log("Move Input: " + moveInput);

        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }
}
