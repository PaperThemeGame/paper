using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BubbleFloat : MonoBehaviour
{
    [Header("漂浮")]
    public float amplitude  = 0.6f;
    public float hoverSpeed = 1.2f;

    [Header("受踩下降")]
    public float downGravityScale = 3f;   // 玩家踩在头上时的重力倍率

    [Header("回弹")]
    public float dropExtra = 1.2f;        // 玩家离开后额外下降的距离
    public float reboundUpSpeed = 3f;     // 回弹上升速度（单位/秒）

    private Rigidbody2D rb;
    private float baseY;                  // 原始漂浮中心
    private float lowestY;                // 本次回弹的最低点
    private bool playerOnTop = false;

    private enum State { Floating, Pressed, ReboundDown, ReboundUp }
    private State state = State.Floating;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType   = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        baseY = transform.position.y;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Floating:
                DoFloat();
                break;

            case State.Pressed:
                rb.gravityScale = downGravityScale;
                break;

            case State.ReboundDown:
                // 已到达最低点？（简单用 Y 判断）
                if (rb.position.y <= lowestY)
                {
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 0f;
                    state = State.ReboundUp;
                }
                break;

            case State.ReboundUp:
                Vector2 pos = rb.position;
                pos.y = Mathf.MoveTowards(pos.y, baseY, reboundUpSpeed * Time.fixedDeltaTime);
                rb.position = pos;

                if (Mathf.Abs(pos.y - baseY) < 0.05f)
                {
                    rb.position = new Vector2(pos.x, baseY);
                    state = State.Floating;
                }
                break;
        }
    }

    private void DoFloat()
    {
        float targetY = baseY + Mathf.Sin(Time.time * hoverSpeed) * amplitude;
        float error = targetY - rb.position.y;
        float force = error * 30f - rb.velocity.y * 8f;
        rb.AddForce(Vector2.up * force);
    }

    /*================ 碰撞检测 =================*/
    private void OnCollisionStay2D(Collision2D c)
    {
        if (!playerOnTop && c.gameObject.CompareTag("Player"))
        {
            foreach (var p in c.contacts)
            {
                if (p.point.y > transform.position.y)
                {
                    playerOnTop = true;
                    state = State.Pressed;
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            playerOnTop = false;

            if (state == State.Pressed)
            {
                // 计算最低点：当前位置再往下 dropExtra
                lowestY = rb.position.y - dropExtra;
                rb.gravityScale = downGravityScale; // 继续下降
                state = State.ReboundDown;
            }
        }
    }
}