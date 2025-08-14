using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement movement;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(movement.isDash)
        {
            if (collision.CompareTag("CanDestory"))
            {
                if(collision.gameObject.TryGetComponent(out Bubbles bubbles))
                {
                    movement.ResetDashCoolTime();
                }
                movement.ResetDashCoolTime();
                Destroy(collision.gameObject);
            }
        }
    }
}
