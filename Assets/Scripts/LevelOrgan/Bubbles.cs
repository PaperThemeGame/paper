using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubbles : MonoBehaviour
{
    public float effectRadius;

    private void OnDestroy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, effectRadius);
        foreach (var hitCollider in hitColliders)
        {
            Mimosa mimosa = hitCollider.GetComponent<Mimosa>();
            if (mimosa != null)
            {
                mimosa.GetComponent<Animator>().SetBool("isTriggered", true);
                mimosa.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
