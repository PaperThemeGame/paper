using UnityEngine;

public class Spike : MonoBehaviour
{
    private bool isEnter;
    [SerializeField]
    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnter)
        {
            playerController.Respawn();
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
