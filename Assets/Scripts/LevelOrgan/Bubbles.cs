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
                Destroy(mimosa.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
