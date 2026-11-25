using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movment")]
    public float moveSpeed = 5f;
    public float horizontalMovement = 10;
    [Header("Jump")]
    //Jumping
    public float jumpPower = 10f;
    public int MaxJumps = 2;
    int JumpsRemaining;
    [Header("GroundCheck")]
    public Transform GroundCheckPos;
    public Vector2 GroundCheckSize = new Vector2(0.5f ,0.05f);
    public LayerMask groundlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;
        GroundCheck();
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(GroundCheckPos.position, GroundCheckSize);
    }
    private void GroundCheck()
    {
        if(Physics2D.OverlapBox(GroundCheckPos.position, GroundCheckSize, 0, groundlayer))
        {
            JumpsRemaining = MaxJumps;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (JumpsRemaining > 0)
        {
            if (context.performed)
            {
                //Håll ner Hopp knappen fullt = Högst hopp som möjligt
                rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpPower);
                JumpsRemaining--;
            }
            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x , rb.linearVelocity.y * 0.5f);
                JumpsRemaining--;
            }
        }

    }


}
