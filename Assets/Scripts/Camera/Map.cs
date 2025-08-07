using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("无限地图")]
    public GameObject mainCamera;// 主摄像机对象
    public float mapwidth;//地图宽度
    public int mapNums;// 地图重复的次数
    private float totalwidth;// 总地图宽度
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");//查找标签为"Maincamera"的对象并赋值
        mapwidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;// 通过spriteRenderer获得图像宽度
        totalwidth = mapwidth* mapNums;// 计算总地图宽度
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tempPosition =transform.position;// 获取当前位置
        if (mainCamera.transform.position.x > transform.position.x + totalwidth / 2)
        {
            tempPosition.x += totalwidth;// 将地图向右平移一个完整的地图宽度
            transform.position = tempPosition;//更新位置
        }
        else if (mainCamera.transform.position.x < transform.position.x - totalwidth / 2)
        {
            tempPosition.x -= totalwidth;// 将地图向左平移一个完整的地图宽度
            transform.position=tempPosition;//更新位置
        }
    }
}
