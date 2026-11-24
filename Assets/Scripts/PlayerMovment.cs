using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    public Rigidbody2D rb;
    //Movment
    public float moveSpeed = 5f;
    float horizontalMovement;
    //Jumping
    public float jumpPower = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Håll ner Hopp knappen fullt = Högst hopp som möjligt
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpPower);
        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x , rb.linearVelocity.y * 0.5f);
        }

    }
}
