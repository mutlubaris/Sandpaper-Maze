using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTarget : InterfaceBase, ICameraTarget
{
    
    private void OnEnable()
    {
        SubToCamera();
    }

    private void OnDisable()
    {
        UnSubToCamera();
    }

    public void SubToCamera()
    {
        List<CinemachineVirtualCamera> virtualCameras = new List<CinemachineVirtualCamera>(FindObjectsOfType<CinemachineVirtualCamera>());

        foreach (var item in virtualCameras)
        {
            item.Follow = transform.Find("Graphic");
            item.LookAt = transform.Find("Graphic");
        }
       
    }

    public void UnSubToCamera()
    {
        List<CinemachineVirtualCamera> virtualCameras = new List<CinemachineVirtualCamera>(FindObjectsOfType<CinemachineVirtualCamera>());

        foreach (var item in virtualCameras)
        {
            item.Follow = null;
            item.LookAt = null;
        }
    }
}
