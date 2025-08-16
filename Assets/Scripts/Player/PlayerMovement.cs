using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform spawnPos;
    
    //上升和下降时重力比例大小切换
    [Header("重力相关")]
    public float normalGravityScale;
    public float downGravityScale;

    [Header("移动相关")]
    public float moveMaxSpeed;//最大移动速度
    [Range(0f, 20f)]
    public float moveAcceleration;//移动加速度
    [Range(0f,20f)]
    public float moveDecceleration;//移动减速度
    [Range(0,1f)]
    public float airAccel;//在空中时加速度

    [Header("跳跃相关")]
    public float jumpForce;
    public float coyoteTime;//土狼时间
    public float wallJumpForce;//蹬墙跳力度

    [Header("冲刺相关")]
    public float dashSpeed;
    public float dashTime;
    public float dashCoolTime;
    public float dashDrag;//冲刺时的线性阻力

    [Header("地面检测箱参数")]
    public Vector2 groundBoxSize;
    public Vector2 groundBoxOffset;

    [Header("墙壁检测箱参数")]
    public Vector2 wallBoxSize;
    public Vector2 wallBoxOffset;

    [Header("状态")]
    public bool isJumping;
    public bool isDash;
    public bool isStillOnGround;

    private Animator animator;
    private Vector2 moveInput;
    private Rigidbody2D rb2D;
    private float gravityScale;

    private float coyoteTimer;
    private float dashTimer;

    private void Start()
    {
        rb2D=GetComponent<Rigidbody2D>();
        gravityScale=rb2D.gravityScale;
        dashTimer = dashCoolTime;
        animator=GetComponent<Animator>();
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            animator.SetBool("isRunning", true);
        }
        else if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        dashTimer += Time.deltaTime;

        if (CheckGround())
        {
            if (!isStillOnGround)
            {
                coyoteTimer = coyoteTime;
            }
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
            isStillOnGround = false;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (CheckGround() || coyoteTimer > 0)
            {
                Jump();
                coyoteTimer = 0;
            }

            if (!CheckGround() && CheckLeftWall())
            {
                WallJump(new Vector2(1, 1));
            }

            if (!CheckGround() && CheckRightWall())
            {
                WallJump(new Vector2(-1, 1));
            }
        }

        if (Input.GetKeyDown(KeyCode.L) && dashTimer >= dashCoolTime)
        {
            Dash(moveInput);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        if (!isDash)
        {
            ChangeGravityScale();
        }

    }

    private void FixedUpdate()
    {
        if (isDash) return;
        Run();
    }

    private void Run()
    {
        float targetSpeed=moveInput.x*moveMaxSpeed;
        float acceleration;
        if(CheckGround())
        {
            if(Mathf.Abs(moveInput.x)>0.01f)
            {
                acceleration = moveAcceleration;
            }
            else
            {
                acceleration = moveDecceleration;
            }
        }
        else
        {
            if (Mathf.Abs(moveInput.x) > 0.01f)
            {
                acceleration = moveAcceleration*airAccel;
            }
            else
            {
                acceleration = moveDecceleration*airAccel;
            }
        }
        float speedDif = targetSpeed - rb2D.velocity.x;
        rb2D.velocity += new Vector2(speedDif * acceleration*Time.fixedDeltaTime, 0);
    }

    public void Jump()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
        rb2D.velocity += Vector2.up * jumpForce;
        isStillOnGround = true;
    }

    public void WallJump(Vector2 dir)
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
        rb2D.velocity += wallJumpForce * dir;
    }

    public void Dash(Vector2 dir)
    {
        dashTimer = 0;//清空冲刺计时器时间
        Vector2 targetDir=Vector2.right*transform.localScale.x;
        if(dir!=Vector2.zero)
        {
            targetDir = dir;
        }
        rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
        rb2D.velocity += dashSpeed*targetDir.normalized;
        StartCoroutine(IDash());
    }

    public IEnumerator IDash()
    {
        isDash = true;
        rb2D.gravityScale = 0;
        rb2D.drag=dashDrag;
        yield return new WaitForSeconds(dashTime);
        rb2D.gravityScale = gravityScale;
        rb2D.drag = 0;
        isDash = false;
        if(CheckGround())
        {
            ResetDashCoolTime();
        }
    }



    #region 重生
    public void Respawn()
    {
        transform.position = spawnPos.position;
    }
    #endregion
    public void ResetDashCoolTime()
    {
        dashTimer = dashCoolTime;
    }

    public void ChangeGravityScale()
    {
        if(rb2D.velocity.y > 0)
        {
            rb2D.gravityScale = normalGravityScale;
        }
        else if(rb2D.velocity.y < 0)
        {
            rb2D.gravityScale = downGravityScale;
        }
    }

    public bool CheckGround()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBoxSize, 0, LayerMask.GetMask("Ground"));
    }

    public bool CheckLeftWall()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(-wallBoxOffset.x, wallBoxOffset.y), wallBoxSize,0,LayerMask.GetMask("Ground"));
    }

    public bool CheckRightWall()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(wallBoxOffset.x, wallBoxOffset.y), wallBoxSize, 0, LayerMask.GetMask("Ground"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + groundBoxOffset, groundBoxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(wallBoxOffset.x, wallBoxOffset.y), wallBoxSize);
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(-wallBoxOffset.x, wallBoxOffset.y), wallBoxSize);
    }

}
