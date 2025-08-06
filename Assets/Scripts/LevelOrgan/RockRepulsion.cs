using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockRepulsion : MonoBehaviour
{
    [Header("冲刺碰撞设置")]
    public float dashRepulsionForce = 15f;      // 冲刺碰撞反弹力度
    public float rockFallDelay = 0.2f;         // 岩石下落延迟时间
    [Header("回弹调节")]
    public float returnSpeed = 5f;            // 回弹速度（值越小越慢）
    public float bounceDistanceMultiplier = 1f; // 反弹距离倍率（1=回到起跳点，0.5=一半距离，2=两倍距离）
    [Range(0.1f, 10f)]
    public float bounceSpeedMultiplier = 1f;    // 反弹速度倍率（调节按钮）
    [Header("抛物线设置")]
    public float parabolaHeight = 2f;         // 抛物线高度（值越大弧线越高）
    public bool useParabolaPath = true;       // 是否使用抛物线路径
    
    [Header("岩石设置")]
    public float fallForce = 10f;              // 岩石下落力度
    public LayerMask groundLayer = 1 << 8;    // 地面图层（默认第8层）
    
    private Transform playerTransform;         // 玩家Transform
    private Rigidbody2D playerRb2D;          // 玩家刚体
    private Rigidbody2D rockRb2D;            // 岩石刚体
    private PlayerController playerController; // 玩家控制器
    private bool hasFallen = false;          // 岩石是否已下落
    private bool hasLanded = false;          // 岩石是否已落地
    private Vector3 playerOriginalPos;       // 玩家起跳位置
    
    void Start()
    {
        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerRb2D = player.GetComponent<Rigidbody2D>();
            playerController = player.GetComponent<PlayerController>();
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
        // 检查是否是玩家碰撞（不停止下落）
        if (collision.gameObject.CompareTag("Player") && !hasFallen)
        {
            // 检查玩家是否处于冲刺状态
            if (playerController != null && playerController.IsDashing())
            {
                // 1. 重置玩家冲刺
                playerController.ResetDash();
                
                // 2. 平滑移动玩家回起跳位置
                StartCoroutine(SmoothReturnToOriginalPos());
                
                // 3. 触发岩石下落（无论是否碰撞玩家都会继续下落）
                StartCoroutine(TriggerRockFall());
            }
        }
        
        // 检测岩石接触地面（只有碰到地面才会停止）
        if (!hasLanded && collision.gameObject.layer == groundLayer)
        {
            hasLanded = true;
            
            // 固定岩石
            if (rockRb2D != null)
            {
                rockRb2D.isKinematic = true;
                rockRb2D.velocity = Vector2.zero;
                rockRb2D.angularVelocity = 0f;
            }
            
            Debug.Log("岩石已落地并固定");
        }
        
        // 检测岩石碰到其他物体（继续下落）
        if (hasFallen && !hasLanded && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("岩石碰到玩家但继续下落");
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
            Debug.Log("岩石开始下落");
        }
    }
    
    // 更新玩家起跳位置（在玩家开始冲刺时调用）
    public void UpdatePlayerOriginalPos()
    {
        if (playerTransform != null)
        {
            playerOriginalPos = playerTransform.position;
            Debug.Log("已更新玩家起跳位置: " + playerOriginalPos);
        }
    }
    
    // 平滑移动玩家回起跳位置（支持抛物线路径）
    private IEnumerator SmoothReturnToOriginalPos()
    {
        if (playerTransform == null) yield break;
        
        // 禁用玩家控制，避免回弹过程中移动
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        Vector3 startPos = playerTransform.position;
        
        // 根据距离倍率计算实际反弹距离
        Vector3 targetPos = startPos + (playerOriginalPos - startPos) * bounceDistanceMultiplier;
        
        float distance = Vector3.Distance(startPos, targetPos);
        float adjustedSpeed = returnSpeed * bounceSpeedMultiplier;
        float duration = distance / adjustedSpeed; // 根据距离和速度倍率计算持续时间
        
        Vector3 controlPoint;
        
        if (useParabolaPath)
        {
            // 计算抛物线控制点（最高点）
            Vector3 midPoint = Vector3.Lerp(startPos, targetPos, 0.5f);
            // 计算垂直方向（向上）
            Vector3 upDirection = Vector3.up;
            // 设置抛物线高度
            controlPoint = midPoint + upDirection * parabolaHeight;
        }
        else
        {
            // 使用直线运动
            controlPoint = Vector3.Lerp(startPos, targetPos, 0.5f);
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // 使用平滑的插值
            t = Mathf.SmoothStep(0, 1, t);
            
            // 使用二次贝塞尔曲线计算抛物线路径
            Vector3 position = CalculateQuadraticBezierPoint(t, startPos, controlPoint, targetPos);
            playerTransform.position = position;
            yield return null;
        }
        
        // 确保到达目标位置
        playerTransform.position = targetPos;
        
        // 重新启用玩家控制
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // 清除速度
        if (playerRb2D != null)
        {
            playerRb2D.velocity = Vector2.zero;
        }
        
        Debug.Log($"玩家已平滑回弹到目标位置: {targetPos}");
    }
    
    // 计算二次贝塞尔曲线点
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // B(t) = (1-t)²P0 + 2(1-t)tP1 + t²P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        
        Vector3 point = uu * p0; // (1-t)²P0
        point += 2 * u * t * p1; // 2(1-t)tP1
        point += tt * p2; // t²P2
        
        return point;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}