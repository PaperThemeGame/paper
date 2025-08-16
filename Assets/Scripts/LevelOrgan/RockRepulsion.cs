using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockRepulsion : MonoBehaviour
{
    [Header("冲刺碰撞设置")]
    public float dashRepulsionForce = 15f;      // 冲刺碰撞反弹力度
    public float rockFallDelay = 0.2f;         // 岩石下落延迟时间
    
    [Header("抛物线反弹设置")]
    [Range(1f, 10f)] public float parabolaSpeed = 5f; // 抛物线反弹速度（1=慢，5=中等，10=快）
    [Range(0.5f, 5f)] public float parabolaHeight = 2f; // 抛物线高度（0.5=低弧线，2=中等，5=高弧线）
    
    [Header("岩石设置")]
    public float fallForce = 10f;              // 岩石下落力度
    public LayerMask groundLayer = 1 << 6;    // 地面图层（根据实际调试改为第6层）
    
    // 以下参数已弃用（保留兼容性但不再显示在Inspector中）
    [HideInInspector] public float returnSpeed = 5f;            // 已弃用：回弹速度
    [HideInInspector] [Range(0.1f, 10f)] public float bounceSpeedMultiplier = 1f;    // 已弃用：反弹速度倍率
    [HideInInspector] public bool useParabolaPath = true;       // 已弃用：抛物线路径开关
    
    private Transform playerTransform;         // 玩家Transform
    private Rigidbody2D playerRb2D;          // 玩家刚体
    private Rigidbody2D rockRb2D;            // 岩石刚体
    private PlayerMovement playerMovement;     // 玩家移动控制器（原PlayerController）
    private bool hasFallen = false;          // 岩石是否已下落
    private bool hasLanded = false;          // 岩石是否已落地
    private bool hasRecordedStartPos = false; // 是否已记录起跳位置
    private Vector3 playerOriginalPos;       // 玩家起跳位置
    
    void Start()
    {
        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerRb2D = player.GetComponent<Rigidbody2D>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }
        
        // 获取岩石刚体
        rockRb2D = GetComponent<Rigidbody2D>();
        if (rockRb2D != null)
        {
            rockRb2D.isKinematic = true; // 初始静止
            rockRb2D.mass = 1000f; // 设置极大质量防止被推动
        }
    }
    
    // 检测碰撞
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查是否是玩家碰撞
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!hasFallen && playerMovement != null && playerMovement.isDash)
            {
                // 1. 立即清除玩家速度
                if (playerRb2D != null)
                {
                    playerRb2D.velocity = Vector2.zero;
                    playerRb2D.angularVelocity = 0f;
                }
                
                // 2. 重置玩家冲刺
                playerMovement.ResetDashCoolTime();
                
                // 3. 抛物线后半段反弹回到起跳位置
                StartCoroutine(ParabolaReturnToOriginalPos());
                
                // 4. 触发岩石下落
                StartCoroutine(TriggerRockFall());
            }
        }
        
        // 检测岩石接触地面
        if (collision.gameObject.layer == groundLayer && !hasLanded)
        {
            hasLanded = true;
            
            // 固定岩石
            if (rockRb2D != null)
            {
                rockRb2D.isKinematic = true;
                rockRb2D.velocity = Vector2.zero;
                rockRb2D.angularVelocity = 0f;
            }
        }
    }
    
    // 触发岩石下落协程
    private IEnumerator TriggerRockFall()
    {
        yield return new WaitForSeconds(rockFallDelay);
        
        if (rockRb2D != null)
        {
            rockRb2D.isKinematic = false;
            rockRb2D.AddForce(Vector2.down * fallForce, ForceMode2D.Impulse);
            hasFallen = true;
        }
    }
    
    // 更新玩家起跳位置（在玩家开始冲刺时调用）
    public void UpdatePlayerOriginalPos()
    {
        if (playerTransform != null)
        {
            playerOriginalPos = playerTransform.position;
        }
    }
    
    // 抛物线后半段反弹（上升-下降回到起跳位置）
    private IEnumerator ParabolaReturnToOriginalPos()
    {
        if (playerTransform == null) yield break;

        // 禁用玩家控制
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 禁用物理
        if (playerRb2D != null)
        {
            playerRb2D.velocity = Vector2.zero;
            playerRb2D.angularVelocity = 0f;
        }

        Vector3 startPos = playerTransform.position;
        Vector3 targetPos = playerOriginalPos; // 直接回到起跳位置
        
        // 计算水平距离
        float horizontalDistance = Vector3.Distance(new Vector3(startPos.x, 0, 0), new Vector3(targetPos.x, 0, 0));
        float duration = horizontalDistance / parabolaSpeed; // 根据距离和速度计算时间
        
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // 使用平滑的抛物线公式，确保运动流畅
            // 使用二次函数：y = -4h(t-0.5)² + h，其中h是高度
            float horizontalPos = Mathf.Lerp(startPos.x, targetPos.x, t);
            float verticalOffset = -4 * parabolaHeight * (t - 0.5f) * (t - 0.5f) + parabolaHeight;
            
            playerTransform.position = new Vector3(horizontalPos, startPos.y + verticalOffset, startPos.z);
            yield return null;
        }
        
        // 确保最终位置精确，避免位置误差
        playerTransform.position = targetPos;

        // 重新启用玩家控制
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // 恢复物理状态 - 给予一个向下的初始速度，避免停顿
        if (playerRb2D != null)
        {
            playerRb2D.gravityScale = 1f; // 恢复重力
            // 给一个小幅度的向下速度，避免停顿感
            playerRb2D.velocity = new Vector2(0f, -6f);
            playerRb2D.angularVelocity = 0f;
        }
    }
    
    // 方法已移除 - 现在使用直接反弹，不需要计算贝塞尔曲线
    private void Update()
    {
        // 更精确的起跳位置记录
        if (playerMovement != null)
        {
            // 当玩家开始冲刺时记录位置（更准确）
            if (playerMovement.isDash && !hasRecordedStartPos)
            {
                // 记录玩家当前位置作为起跳点
                UpdatePlayerOriginalPos();
                hasRecordedStartPos = true;
            }
            
            // 当玩家停止冲刺时重置记录状态
            if (!playerMovement.isDash)
            {
                hasRecordedStartPos = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}