using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraHandle : MonoBehaviour
{
    [Tooltip("变化速度")]public float lightTransitionSpeed = 2.0f;
    public int currentRoomID;
    public int targetRoomID;
    public List<RoomCamera> roomCameras;
    public GameObject roomLight;

    private void Update()
    {
        //灯光改变
        if (currentRoomID >= 23 && currentRoomID <= 29)
        {
            roomLight.GetComponent<Light2D>().intensity = Mathf.MoveTowards(roomLight.GetComponent<Light2D>().intensity, 0.005f, lightTransitionSpeed * Time.deltaTime);
        }
        else if (currentRoomID >= 15 && currentRoomID <= 22)
        {
            roomLight.GetComponent<Light2D>().intensity = Mathf.MoveTowards(roomLight.GetComponent<Light2D>().intensity, 0.3f, lightTransitionSpeed * Time.deltaTime);
        }
        else
        {
            roomLight.GetComponent<Light2D>().intensity = Mathf.MoveTowards(roomLight.GetComponent<Light2D>().intensity, 1f, lightTransitionSpeed * Time.deltaTime);
        }
    }
    public void GoToTargetRoom()
    {
        if (currentRoomID == targetRoomID)
        {
            return;
        }
        RoomCamera targetRoomCamera = null;
        foreach (RoomCamera roomCamera in roomCameras)
        {
            roomCamera.SetCameraPriority(10);
            if (roomCamera.id == targetRoomID)
            {
                targetRoomCamera = roomCamera;
            }
        }
        targetRoomCamera.SetCameraPriority(20);
        currentRoomID = targetRoomID;
    }
}
