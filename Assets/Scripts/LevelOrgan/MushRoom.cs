using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MushRoom : MonoBehaviour
{
    public float ejectionForce;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Rigidbody2D rb2D))
        {
            anim.SetBool("isEnter", true);
            AddForce(rb2D);
            StartCoroutine(Reset());
        }
    }
    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isEnter", false);
    }
    public void AddForce(Rigidbody2D rb2D)
    {
        rb2D.AddForce(ejectionForce * Vector2.up, ForceMode2D.Impulse);
    }
}
