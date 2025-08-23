using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEggGenerator : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            EasterEggManager.Instance.CreateEasterEgg();
        }
    }
}
