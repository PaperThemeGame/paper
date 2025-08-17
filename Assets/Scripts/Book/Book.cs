using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public Animator anim;
    public GameObject[] page;
    private int current_page = 0;
    private bool isFlipping;    // 防止连续按键
    private void Awake()
    {
        for (int i = 0; i < page.Length; i++)
        {
            page[i].SetActive(false);
        }
        page[current_page].SetActive(true);
    }
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Control();
    }

    private void Control()
    {
        if (isFlipping) return; // 防止连续按键

        if (Input.GetKeyDown(KeyCode.E))
        {
            isFlipping = true;
            anim.Play("nextPage");
            StartCoroutine(SwitchPageAfter(NextPage));

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isFlipping = true;
            anim.Play("lastPage");
            StartCoroutine(SwitchPageAfter(LastPage));
            
        }
    }
    
    private IEnumerator SwitchPageAfter(System.Action changeIndex)
    {
        page[current_page].SetActive(false); // 先隐藏当前页
        changeIndex();                       // 计算新索引
        yield return null;                   // 等一帧，避免闪烁
        yield return new WaitForSeconds(0.75f); // 等待动画播放完毕
        page[current_page].SetActive(true);  // 显示新页
        isFlipping = false;                  // 允许再次翻页
    }

    private void NextPage()
    {
        current_page++;
        if (current_page >= page.Length)
            current_page = 0;
    }

    private void LastPage()
    {
        current_page--;
        if (current_page < 0)
            current_page = page.Length - 1;
    }
}
