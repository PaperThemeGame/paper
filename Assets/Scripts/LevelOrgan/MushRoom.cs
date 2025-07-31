using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushRoom : MonoBehaviour
{
    public float ejectionForce;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.TryGetComponent(out Rigidbody2D rb2D))
        {
            AddForce(rb2D);
        }
    }

    public void AddForce(Rigidbody2D rb2D)
    {
        rb2D.AddForce(ejectionForce * Vector2.up, ForceMode2D.Impulse);
    }
}
