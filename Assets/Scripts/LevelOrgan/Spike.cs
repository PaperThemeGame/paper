using UnityEngine;

public class Spike : MonoBehaviour
{
    private bool isEnter;
    [SerializeField]
    private PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnter)
        {
            playerMovement.Respawn();
            isEnter = false;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isEnter = true;
        }
    }
}
