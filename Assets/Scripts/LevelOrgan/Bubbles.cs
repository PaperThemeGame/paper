//需要预设泡泡,可以直接复制 氧气泡 (16)
//尖刺的tag设置为Spike,需要box collider 2d,泡泡碰撞消失就是识别碰撞物体的tag是否为Spike
//respawnbubble 为泡泡重生的public函数
//泡泡和预设泡泡最好按照test中  氧气泡 (16)  设置,不能少Box cillider 2d
//泡泡被冲刺碰撞后销毁到重生的时间和被尖刺刺破到重生的时间都可以在bubbles.cs调节


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 泡泡类，控制泡泡的行为，包括碰撞检测、销毁和重生机制
public class Bubbles : MonoBehaviour
{
    // 泡泡效果半径，用于检测影响范围内的对象
    public float effectRadius;
    
    // 泡泡预制体，用于重生时创建新的泡泡实例

    public GameObject bubblePrefab; // 泡泡预设
    
    // 泡泡重生间隔时间（秒）
    public float respawnInterval = 1f; // 刷新间隔时间

    // 被玩家冲刺碰撞销毁后的泡泡重生间隔时间（秒）
    public float dashRespawnInterval = -1f; // 冲刺碰撞重生间隔时间

    // 记录泡泡的初始位置，用于重生时定位
    private Vector3 initialPosition;
    
    // 标记泡泡是否已被销毁
    private bool isDestroyed = false;

    // 初始化，记录泡泡的初始位置
    private void Start()
    {
        initialPosition = transform.position;
    }

    // 当泡泡与其他2D碰撞体发生碰撞时调用
    // 特别处理与"Spike"标签对象的碰撞
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查碰撞的物体是否是Spike
        if (collision.gameObject.CompareTag("Spike"))
        {
            // 销毁前调用静态刷新方法
            RespawnBubbleStatic(bubblePrefab, initialPosition, respawnInterval, effectRadius);
            isDestroyed = true;
            Destroy(gameObject);
        }
    }


    // 公共方法，用于在泡泡被非Spike物体销毁时手动触发重生机制
    public void RespawnBubble()
    {
        // 检查是否设置了专门的冲刺重生间隔时间
        float interval = dashRespawnInterval >= 0 ? dashRespawnInterval : respawnInterval;
        RespawnBubbleStatic(bubblePrefab, initialPosition, interval, effectRadius);
    }

    // 静态方法，用于启动泡泡重生过程
    // 使用临时管理器来处理协程，因为当前对象即将被销毁
    private static void RespawnBubbleStatic(GameObject bubblePrefab, Vector3 position, float interval, float radius)
    {
        // 用一个临时管理器对象来启动协程
        GameObject manager = new GameObject("BubbleRespawnManager");
        BubbleRespawnManager mgrScript = manager.AddComponent<BubbleRespawnManager>();
        mgrScript.StartRespawn(bubblePrefab, position, interval, radius);
    }

    // 当泡泡被销毁时调用，用于触发范围内的Mimosa对象动画
    private void OnDestroy()
    {
        // 检测范围内的所有碰撞体
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, effectRadius);
        foreach (var hitCollider in hitColliders)
        {
            // 查找范围内的Mimosa对象
            Mimosa mimosa = hitCollider.GetComponent<Mimosa>();
            if (mimosa != null)
            {
                // 触发Mimosa的动画和状态变化
                mimosa.GetComponent<Animator>().SetBool("isTriggered", true);
                mimosa.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    // 在Scene视图中绘制 Gizmos，用于可视化泡泡的效果范围
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}

// 泡泡重生管理器类，专门用于处理泡泡重生的协程
// 因为原泡泡对象会被销毁，所以需要一个独立的对象来执行协程
public class BubbleRespawnManager : MonoBehaviour
{

    public void StartRespawn(GameObject bubblePrefab, Vector3 position, float interval, float radius)
    {
        StartCoroutine(Respawn(bubblePrefab, position, interval, radius));
    }
    
       private IEnumerator Respawn(GameObject bubblePrefab, Vector3 position, float interval, float radius)
    {
        // 等待指定的重生间隔时间
        yield return new WaitForSeconds(interval);
        
        // 创建新的泡泡实例
        GameObject newBubble = Instantiate(bubblePrefab, position, Quaternion.identity);
        Bubbles bubbleScript = newBubble.GetComponent<Bubbles>();
        if (bubbleScript != null)
        {
            // 保持新泡泡的配置与原泡泡一致
            bubbleScript.bubblePrefab = bubblePrefab;
            bubbleScript.respawnInterval = interval;
            bubbleScript.effectRadius = radius;
        }
        
        // 完成重生后销毁管理器对象
        Destroy(gameObject); // 销毁管理器
    }
}