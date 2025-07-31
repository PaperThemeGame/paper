using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(controller.isDashing)
        {
            if (collision.CompareTag("CanDestory"))
            {
                if(collision.gameObject.TryGetComponent(out Bubbles bubbles))
                {
                    controller.ResetDashTimer();
                }
                controller.ResetDashTimer();
                Destroy(collision.gameObject);
            }
        }
    }
}
