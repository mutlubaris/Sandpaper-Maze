using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraAreaController : MonoBehaviour
{
    public CinemachineVirtualCamera CinemachineVirtualCamera;

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if(character != null)
        {
            if(character.CharacterData.CharacterControlType == CharacterControlType.Player)
            {
                CinemachineVirtualCamera.Follow = CharacterManager.Instance.Player.transform.Find("Graphic"); 
                CinemachineVirtualCamera.LookAt = CharacterManager.Instance.Player.transform.Find("Graphic"); 
                CinemachineVirtualCamera.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null)
        {
            if (character.CharacterData.CharacterControlType == CharacterControlType.Player)
            {
                CinemachineVirtualCamera.gameObject.SetActive(false);
            }
        }
    }
}
