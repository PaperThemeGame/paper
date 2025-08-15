using System.Collections;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public StateMachine stateMachine;
    public State normalState;
    public State attackState;

    [Header("攻击相关")]
    public float attackCoolTime;

    [Header("方块投放设置")]
    public GameObject fallingBlockPrefab;
    public float dropSpeed = 5f;
    public float dropInterval = 2f;
    public float dropRange = 10f;

    [Header("预警线设置")]
    public GameObject warningLinePrefab;
    public float warningDuration = 1f;
    public int flashCount = 3;
    public float flashInterval = 0.2f;

    private Transform player;
    private Camera mainCamera;
    private float lastDropTime;
    private bool isDropping = false;

    private void Start()
    {
        InitState();
        InitializeComponents();
    }

    private void LateUpdate()
    {
        if (stateMachine == null) InitState();
        if (stateMachine != null) stateMachine.CheckChangeState();
        if (mainCamera == null) mainCamera = Camera.main;
        
        if (!isDropping && Time.time - lastDropTime >= dropInterval && mainCamera != null)
        {
            StartCoroutine(DropBlockSequence());
        }
    }

    public void InitState()
    {
        stateMachine = new StateMachine();
        normalState = new NormalState(stateMachine, this);
        attackState = new AttackState(stateMachine, this);
        stateMachine.Init(normalState);
    }

    private void InitializeComponents()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        mainCamera = Camera.main;
        lastDropTime = Time.time;
    }

    private IEnumerator DropBlockSequence()
    {
        isDropping = true;
        float playerX = GetPlayerXPosition();
        Vector3 dropPosition = GetDropPosition(playerX);
        yield return StartCoroutine(ShowWarningLine(dropPosition));
        SpawnFallingBlock(dropPosition);
        lastDropTime = Time.time;
        isDropping = false;
    }

    private float GetPlayerXPosition()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else return 0f;
        }
        return player.position.x;
    }

    private Vector3 GetDropPosition(float targetX)
    {
        float topY = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - 2f;
        float screenLeft = mainCamera.ScreenToWorldPoint(Vector3.zero).x;
        float screenRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        targetX = Mathf.Clamp(targetX, screenLeft, screenRight);
        return new Vector3(targetX, topY, 0f);
    }

    private IEnumerator ShowWarningLine(Vector3 dropPosition)
    {
        if (warningLinePrefab == null) yield break;

        Vector3 rayStart = new Vector3(dropPosition.x, dropPosition.y, 0f);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground", "Platform"));
        
        float groundY = hit.collider != null ? hit.point.y : mainCamera.ScreenToWorldPoint(Vector3.zero).y;
        float ceilingY = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        float lineHeight = ceilingY - groundY;
        
        Vector3 warningPosition = new Vector3(dropPosition.x, groundY + lineHeight / 2f, 0f);
        GameObject warningLine = Instantiate(warningLinePrefab, warningPosition, Quaternion.identity);
        
        SpriteRenderer lineRenderer = warningLine.GetComponent<SpriteRenderer>();
        if (lineRenderer != null)
        {
            warningLine.transform.localScale = new Vector3(0.1f, lineHeight, 1f);
            lineRenderer.sortingOrder = 10;
        }

        for (int i = 0; i < flashCount; i++)
        {
            warningLine.SetActive(true);
            yield return new WaitForSeconds(flashInterval);
            warningLine.SetActive(false);
            yield return new WaitForSeconds(flashInterval);
        }

        Destroy(warningLine);
    }

    private void SpawnFallingBlock(Vector3 position)
    {
        if (fallingBlockPrefab == null) return;

        GameObject block = Instantiate(fallingBlockPrefab, position, Quaternion.identity);
        block.SetActive(true);
        
        SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;
        
        Rigidbody2D rb = block.GetComponent<Rigidbody2D>();
        if (rb == null) rb = block.AddComponent<Rigidbody2D>();
        
        Collider2D collider = block.GetComponent<Collider2D>();
        if (collider == null) collider = block.AddComponent<BoxCollider2D>();

        rb.gravityScale = 0f;
        rb.velocity = Vector2.down * dropSpeed;
        block.AddComponent<FallingBlock>();
    }
}

public class FallingBlock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameOver(other.gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    private void GameOver(GameObject player)
    {
        MonoBehaviour[] playerScripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in playerScripts)
        {
            if (script.GetType().Name.Contains("Player") || script.GetType().Name.Contains("Movement") || script.GetType().Name.Contains("Control"))
            {
                script.enabled = false;
            }
        }
        
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null) playerRb.velocity = Vector2.zero;
        
        Time.timeScale = 0f;
        Destroy(gameObject);
    }

    private void Update()
    {
        if (transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y - 5f)
        {
            Destroy(gameObject);
        }
    }
}
