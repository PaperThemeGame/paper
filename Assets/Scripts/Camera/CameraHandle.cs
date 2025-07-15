using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandle : MonoBehaviour
{
    public int currentRoomID;
    public int targetRoomID;
    public List<RoomCamera> roomCameras;

    public void GoToTargetRoom()
    {
        if(currentRoomID==targetRoomID)
        {
            return;
        }
        RoomCamera targetRoomCamera=null;
        foreach(RoomCamera roomCamera in roomCameras)
        {
            roomCamera.SetCameraPriority(10);
            if(roomCamera.id==targetRoomID)
            {
                targetRoomCamera = roomCamera;
            }
        }
        targetRoomCamera.SetCameraPriority(20);
        currentRoomID = targetRoomID;
    }
}
