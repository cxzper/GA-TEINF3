using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    public Rigidbody2D rb;
    bool isFacingRight = true;

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
    bool isGrounded;
    
    [Header("WallCheck")]
    public Transform WallCheckPos;
    public Vector2 WallCheckSize = new Vector2(0.5f ,0.05f);
    public LayerMask walllayer;
    
    [Header("Gravity")]
    public float baseGravity = 2;
    public float MaxFallSpeed = 18f;
    public float fallSpeedMulitplyer = 2f;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding = false;

    //Wall Jumping
    bool isWallJumping;
    float walljumpDirection;
    float wallJumpTime = 0.5f;
    float walljumptimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        if (!isWallJumping)
        {
            Vector2 newVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = newVelocity;
            Flip();
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

        //Wall Jump
        if (context.performed && walljumptimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(walljumpDirection * wallJumpPower.x, wallJumpPower.y);
            walljumptimer = 0f;

            //Force Flip
            if (Mathf.Sign(transform.localScale.x) != walljumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime - 0.2f); //Wall Jump = 0.5f -- Jump again = 0.6f
        }

    }
    public void ProcessGravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMulitplyer;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -MaxFallSpeed, float.MaxValue));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }
    private void ProcessWallSlide()
    {
        if(!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void ProcessWallJump()
    {
        if (isWallSliding && !isWallJumping)
        {
            walljumpDirection = -Mathf.Sign(transform.localScale.x);
            walljumptimer = wallJumpTime;
        }
        else if (walljumptimer > 0f)
        {
            walljumptimer -= Time.deltaTime;
        }
    }
    private void CancelWallJump()
    {
        isWallJumping = false;
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(GroundCheckPos.position, GroundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(WallCheckPos.position, WallCheckSize);
    }
    private void GroundCheck()
    {
        if(Physics2D.OverlapBox(GroundCheckPos.position, GroundCheckSize, 0, groundlayer))
        {
            JumpsRemaining = MaxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(WallCheckPos.position, WallCheckSize, 0, walllayer);
    }
    
    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }



}
