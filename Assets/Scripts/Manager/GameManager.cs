using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        //测试事件中心
        EventCenter.Instance.EventTrigger("Test1");
    }
}
