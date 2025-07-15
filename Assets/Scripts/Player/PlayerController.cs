using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("属性")]
    public float jumpForce;
    public float speed;

    [Header("状态")]
    public bool canMove = true;
    public bool canJump;
    public bool canDash;
    public bool isDashing;

    [Header("地面检测箱参数")]
    public Vector2 size;
    public Vector2 offset;

    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
        if(Input.GetKeyDown(KeyCode.K))
        {
            Jump();
        }
    }

    public void Move()
    {
        float inputX=Input.GetAxisRaw("Horizontal");
        rb2D.velocity = new Vector2(inputX * speed, rb2D.velocity.y);
    }

    public void Jump()
    {
        if(CheckGround())
        {
            rb2D.velocity += Vector2.up * jumpForce;
        }
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
