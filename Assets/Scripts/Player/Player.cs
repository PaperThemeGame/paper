using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CanDestory"))
        {
            if (controller.isDashing)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
