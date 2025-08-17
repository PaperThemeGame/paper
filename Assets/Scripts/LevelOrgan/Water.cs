//水要设置box collider 2d,勾选is trigger


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    [Range(-20f, 20f)]
    public float pushForce = 5f; // 推力大小和方向

    private PlayerMovement playerMovement; //玩家

    void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerMovement.Respawn(); //触碰尖刺时重生
        }
        else if (collision.gameObject.CompareTag("CanDestory")) //泡泡进入水中被推动
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.AddForce(Vector2.right * pushForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("CanDestory"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.AddForce(Vector2.right * pushForce, ForceMode2D.Force);
            }
            else
            {
                Debug.LogWarning($"{other.name} 没有 Rigidbody2D 组件，无法施加水的推力。");
            }
        }
    }
}
