using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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

    [Header("冲刺相关")]
    public float dashSpeed;
    public float dashTime;
    public float dashCoolTime;
    public float dashDrag;//冲刺时的线性阻力

    [Header("地面检测箱参数")]
    public Vector2 groundBoxSize;
    public Vector2 groundBoxOffset;

    [Header("状态")]
    public bool isJumping;
    public bool isDash;
    public bool isStillOnGround;

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
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        dashTimer += Time.deltaTime;

        if(CheckGround())
        {
            if(!isStillOnGround)
            {
                coyoteTimer = coyoteTime;
            }
        }
        else
        {
            coyoteTimer-=Time.deltaTime;
            isStillOnGround = false;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(CheckGround() || coyoteTimer>0)
            {
                Jump();
                coyoteTimer=0;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)&&dashTimer>=dashCoolTime)
        {
            Dash(moveInput);
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
    }

    public bool CheckGround()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBoxSize, 0, LayerMask.GetMask("Ground"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + groundBoxOffset, groundBoxSize);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(wallBoxOffset.x, wallBoxOffset.y), wallBoxSize);
        //Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(-wallBoxOffset.x, wallBoxOffset.y), wallBoxSize);
    }

}
