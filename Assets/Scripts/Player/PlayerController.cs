using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("属性")]
    public float jumpForce;
    public float speed;
    public float speedLerpTime;
    public float dashSpeed;
    public float dashTime;
    public float dashCoolTime;
    public float dashDrag;
    public float coyoteTime;
    public Transform spawnPos;

    [Header("状态")]
    public bool isDashing;
    public bool isClimb;
    private bool isRevive;


    [Header("动画")]
    private Animator anim;

    [Header("地面检测箱参数")]
    public Vector2 groundBoxSize;
    public Vector2 groundBoxOffset;

    [Header("墙壁检测箱参数")]
    public Vector2 wallBoxSize;
    public Vector2 wallBoxOffset;

    private Rigidbody2D rb2D;
    private float dashTimer;
    private Coroutine dashC;//用于存储冲刺协程 
    private float coyoteTimerCounter;//用于记录人物离开地面滞空时间
    private float normalGravityScale;
    [HideInInspector]
    public bool isStillOnGround;//用于记录玩家按下跳跃键之后是否仍然在地面检测范围内,用于防止土狼时间导致二段跳

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        dashTimer = dashCoolTime;
        normalGravityScale=rb2D.gravityScale;
    }

    private void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float rawX = Input.GetAxisRaw("Horizontal");
        float rawY = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(inputX, inputY);
        Vector2 dirRaw = new Vector2(rawX, rawY);
        if (!isDashing)
        {
            dashTimer += Time.deltaTime;
        }
        if (CheckGround())
        {
            if (!isStillOnGround)
            {
                coyoteTimerCounter = coyoteTime;
            }
        }
        else
        {
            coyoteTimerCounter -= Time.deltaTime;
            isStillOnGround = false;
        }

        if (inputX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            anim.SetBool("isRunning", true);
        }
        else if (inputX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        Move(dir);

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (coyoteTimerCounter > 0)
            {
                Jump();
                coyoteTimerCounter = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.L) && dashTimer > dashCoolTime)
        {
            Dash(dirRaw);
        }

        if (Input.GetKeyDown(KeyCode.J) && CheckWall())
        {
            rb2D.gravityScale = 0;
            rb2D.velocity = Vector2.zero;
        }
        else if (Input.GetKeyUp(KeyCode.J))
        {
            rb2D.gravityScale = normalGravityScale;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        if (isRevive)
        {
            float reviveTimer = 0;
            if (reviveTimer < 0.5)
            {
                reviveTimer += 0.1f;
            }
            else
            {
                isRevive = false;
                reviveTimer = 0;
            }
            
        }
    }

    public void Move(Vector2 dir)
    {
        if(!isDashing)
        {
            rb2D.velocity = new Vector2(dir.x * speed, rb2D.velocity.y);
        }
        else
        {
            rb2D.velocity = Vector2.Lerp(rb2D.velocity, new Vector2(dir.x, rb2D.velocity.y), speedLerpTime*Time.deltaTime);
        }
    }

    public void Jump()
    {
        isStillOnGround = true;
        rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
        rb2D.velocity += Vector2.up * jumpForce;
    }

    public void Dash(Vector2 dir)
    {
        if(isDashing&&dashC!=null)
        {
            StopCoroutine(dashC);
            isDashing = false;
            rb2D.drag = 0;
            rb2D.gravityScale = normalGravityScale;
        }
        rb2D.velocity = Vector2.zero;
        dashTimer = 0;
        
        // 更新起跳位置
        RockRepulsion rockRepulsion = FindObjectOfType<RockRepulsion>();
        if (rockRepulsion != null)
        {
            rockRepulsion.UpdatePlayerOriginalPos();
        }
        
        if(dir==Vector2.zero)
        {
            rb2D.velocity += dashSpeed *new Vector2(transform.localScale.x,0);
        }
        else
        {
            rb2D.velocity += dashSpeed * dir.normalized;
        }
        dashC = StartCoroutine(IDash());
    }

    public void ResetDashTimer()
    {
        dashTimer = dashCoolTime + 1;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    public void ResetDash()
    {
        if (dashC != null)
        {
            StopCoroutine(dashC);
            isDashing = false;
            rb2D.drag = 0;
            rb2D.gravityScale = normalGravityScale;
        }
        dashTimer = 0;
    }

    public IEnumerator IDash()
    {
        isDashing = true;
        rb2D.gravityScale = 0;
        rb2D.drag = dashDrag;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        rb2D.drag = 0;
        rb2D.gravityScale = normalGravityScale;
    }




    #region 重生
    public void Respawn()
    {
        transform.position = spawnPos.position;
        isRevive = true;
    }
    #endregion
    public bool CheckGround()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBoxSize, 0, LayerMask.GetMask("Ground"));
    }

    public bool CheckWall()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(wallBoxOffset.x, wallBoxOffset.y), wallBoxSize, 0, LayerMask.GetMask("Ground"))
            || Physics2D.OverlapBox((Vector2)transform.position + new Vector2(-wallBoxOffset.x, wallBoxOffset.y),wallBoxSize,LayerMask.GetMask("Ground"));
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
