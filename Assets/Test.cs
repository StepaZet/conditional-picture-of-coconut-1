using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Test : MonoBehaviour 
{
    public Rigidbody2D rb;
    public float speed = 2f;
    public Vector2 moveVector;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Walk();
    }
    void Walk()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        moveVector.y = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(moveVector.x * speed, moveVector.y * speed);
    }
    //-----------------------------------------------------------------
}