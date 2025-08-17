using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement movement;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (movement.isDash)
        {
            if (collision.CompareTag("CanDestory"))
            {
                // 尝试获取碰撞对象上的Bubbles组件
                if (collision.gameObject.TryGetComponent(out Bubbles bubbles))
                {
                    // 调用泡泡的重生方法
                    bubbles.RespawnBubble();
                    movement.ResetDashCoolTime();
                }
                movement.ResetDashCoolTime();
                // 销毁原泡泡对象
                Destroy(collision.gameObject);
            }
        }
    }
}
