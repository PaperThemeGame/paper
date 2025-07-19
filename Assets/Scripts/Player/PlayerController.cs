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

    [Header("状态")]
    public bool isDashing;

    [Header("地面检测箱参数")]
    public Vector2 size;
    public Vector2 offset;

    private Rigidbody2D rb2D;
    private float dashTimer;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        dashTimer = dashCoolTime;
    }

    private void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float rawX = Input.GetAxisRaw("Horizontal");
        float rawY= Input.GetAxisRaw("Vertical");
        Vector2 dir=new Vector2(inputX, inputY);
        Vector2 dirRaw=new Vector2(rawX, rawY);
        dashTimer += Time.deltaTime;
        Move(dir);
        if(Input.GetKeyDown(KeyCode.K))
        {
            Jump();
        }
        if(!isDashing&&Input.GetKeyDown(KeyCode.L)&&dashTimer>dashCoolTime)
        {
            Dash(dirRaw);
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
        if(CheckGround())
        {
            rb2D.velocity += Vector2.up * jumpForce;
        }
    }

    public void Dash(Vector2 dir)
    {
        rb2D.velocity = Vector2.zero;
        dashTimer = 0;
        if(dir==Vector2.zero)
        {
            rb2D.velocity += dashSpeed *(Vector2)transform.right.normalized;
        }
        else
        {
            rb2D.velocity += dashSpeed * dir.normalized;
        }
        StartCoroutine(IDash());
    }

    public IEnumerator IDash()
    {
        isDashing = true;
        rb2D.gravityScale = 0;
        rb2D.drag = 5;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        rb2D.drag = 0;
        rb2D.gravityScale = 6;
    }


    public bool CheckGround()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + offset, size, 0, LayerMask.GetMask("Ground"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + offset, size);
    }
}
