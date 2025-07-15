using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferArea : MonoBehaviour
{
    public CameraHandle cameraHandle;
    public int toRoomID;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cameraHandle.targetRoomID = toRoomID;
            cameraHandle.GoToTargetRoom();
        }
    }
}
