using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().spawnPos = transform;
        }
    }
    
}
