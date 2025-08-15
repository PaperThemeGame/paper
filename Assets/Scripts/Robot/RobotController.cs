using System.Collections;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Hover")]
    public float hoverDistance = 0.5f;
    public float hoverSpeed = 1f;

    [Header("Emotion")]
    public float emotionChangeInterval = 10f;

    [Header("Follow")]
    public float followSmooth = 5f;      // 越大越快
    public Vector3 baseOffset = new Vector3(2.5f, 3f, 0); // X 为左右距离，Y 为上方高度

    private int emotion;
    private float timer;
    private Vector3 velocity;            // SmoothDamp 用

    private Animator anim;
    private Animator playerAnim;
    private PlayerMovement playerMove;
    private Transform player;

    private void Awake()
    {
        anim      = GetComponent<Animator>();
        player    = GameObject.FindWithTag("Player").transform;
        playerAnim = player.GetComponent<Animator>();
        playerMove = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        SmoothFollow();
        MatchScale();
        Emotion();
    }

    // 平滑跟随 + 悬浮
    private void SmoothFollow()
    {
        // 1. 水平方向：根据玩家朝向决定正负
        float dir = playerMove.transform.localScale.x > 0 ? -1f : 1f;
        Vector3 targetOffset = new Vector3(dir * baseOffset.x, baseOffset.y + Mathf.Sin(Time.time * hoverSpeed) * hoverDistance, baseOffset.z);

        // 2. 目标世界坐标
        Vector3 targetPos = player.position + targetOffset;

        // 3. 平滑插值
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / followSmooth);
    }

    // 让机器人朝向与玩家一致
    private void MatchScale()
    {
        transform.localScale = new Vector3(playerMove.transform.localScale.x, transform.localScale.y, transform.localScale.z);
        anim.SetBool("isMove", playerAnim.GetBool("isRunning"));
    }

    private void Emotion()
    {
        timer += Time.deltaTime;
        if (timer >= emotionChangeInterval)
        {
            timer = 0;
            emotion = Random.Range(1, 21);
            anim.SetInteger("emotion", emotion);
            StartCoroutine(EmotionReset());
        }
    }

    private IEnumerator EmotionReset()
    {
        yield return new WaitForSeconds(0.5f);
        emotion = 0;
        anim.SetInteger("emotion", emotion);
    }
}