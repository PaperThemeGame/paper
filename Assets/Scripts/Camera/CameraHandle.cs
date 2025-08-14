using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraHandle : MonoBehaviour
{
    public int currentRoomID;
    public int targetRoomID;
    public List<RoomCamera> roomCameras;
    public GameObject roomLight;

    private void Update()
    {
        //灯光改变
        if (currentRoomID >= 23 && currentRoomID <= 29)
        {
            roomLight.GetComponent<Light2D>().intensity = 0.005f;
        }
        else
        {
            roomLight.GetComponent<Light2D>().intensity = 1.0f;
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
