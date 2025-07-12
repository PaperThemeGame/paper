using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("属性")]
    public float jumpForce = 10f;
    public float speed = 5f;
    [Header("状态")]
    public bool canMove = true;
    public bool canJump;
    public bool canDash;
    public bool isDashing;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Move()
    {
        if (canMove)
        {
            
        }
    }
}
