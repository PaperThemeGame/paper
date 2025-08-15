using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float pushForce = 10f; //推力
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
}
