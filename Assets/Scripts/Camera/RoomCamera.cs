using Cinemachine;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    public int id;
    


    private CinemachineVirtualCamera virtualCamera;
    private GameObject player;
    

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        //特殊相机
        if (id == 18)
        {
            virtualCamera.transform.position = new Vector3(player.transform.position.x, virtualCamera.transform.position.y, -10.0f);
        }
        
    }
    public void SetCameraPriority(int priority)
    {
        virtualCamera.Priority = priority;
    }
}
    