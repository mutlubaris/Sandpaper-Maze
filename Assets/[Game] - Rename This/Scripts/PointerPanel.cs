using AdvanceUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerPanel : AdvancePanel
{
    [SerializeField] private GameObject cursor;

    private void LateUpdate()
    {
        cursor.transform.position = Input.mousePosition;
    }
}
