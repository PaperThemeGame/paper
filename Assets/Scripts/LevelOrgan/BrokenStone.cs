using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrokenStone : MonoBehaviour
{
    public float timeToBroken;
    public float timeToRecover;

    private Collider2D _collider;
    private float brokenTimer;
    private float recoverTimer;
    private bool isPlayerStay;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if(isPlayerStay)
        {
            brokenTimer += Time.deltaTime;
        }
        if(brokenTimer>timeToBroken)
        {
            _collider.enabled = false;
            brokenTimer = 0;
            recoverTimer = 0;
        }
        if(!_collider.enabled)
        {
            recoverTimer += Time.deltaTime;
        }
        if(recoverTimer > timeToRecover)
        {
            _collider.enabled=true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            isPlayerStay = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isPlayerStay=false;
        }
    }
}
