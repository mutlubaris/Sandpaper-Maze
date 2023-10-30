using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;

public class WaypointManagerWindow : OdinEditorWindow
{
    [MenuItem("HyperCasualBase/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    [SerializeField]
    [HideInInspector]
    private Transform WaypointRoot;

    protected override void OnGUI()
    {
        base.OnGUI();
        SerializedObject obj = new SerializedObject(this);

        GameObject activeObj = Selection.activeGameObject;
        if (activeObj != null)
        {
            if (string.Equals(activeObj.name, "Waypoints"))
                WaypointRoot = activeObj.transform;
        }

        if (GUILayout.Button("SelectRoot"))
        {
            if (Selection.activeGameObject == null)
            {
                CreateWaypointRoot();
            }
            else WaypointRoot = Selection.activeGameObject.transform;

            Selection.activeGameObject = WaypointRoot.gameObject;
        }


        if (WaypointRoot != null)
        {
            GUILayout.BeginVertical();
            DrawButtons();
            GUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    public void DrawButtons()
    {
        if (GUILayout.Button("Add Waypoint"))
            AddWaypoint();

        if (GUILayout.Button("Add Branch"))
            AddBranch();

        if (GUILayout.Button("Insert Waypoint"))
            InsertWaypoint();

        if (GUILayout.Button("Remove Waypoint"))
            RemoveWaypoint();

    }

    private void AddBranch()
    {
        Waypoint waypoint = InsertWaypoint();
        waypoint.PreviousWaypoint.Branch = waypoint;
        waypoint.PreviousWaypoint.NextWaypoint = waypoint.NextWaypoint;
    }

    public void CreateWaypointRoot()
    {
        GameObject obj = new GameObject();
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.name = "Waypoints";
        WaypointRoot = obj.transform;
    }

    public Waypoint AddWaypoint()
    {
        if (WaypointRoot == null)
            CreateWaypointRoot();

        GameObject obj = new GameObject("Waypoint", typeof(Waypoint));
        obj.transform.SetParent(WaypointRoot, false);

        Waypoint waypoint = obj.GetComponent<Waypoint>();

        if(WaypointRoot.childCount > 1)
        {
            waypoint.PreviousWaypoint = WaypointRoot.GetChild(WaypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.PreviousWaypoint.NextWaypoint = waypoint;
            waypoint.transform.position = waypoint.PreviousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.PreviousWaypoint.transform.forward;
        }

        Selection.activeGameObject = obj;
        return waypoint;
    }

    public Waypoint InsertWaypoint()
    {
        GameObject obj = new GameObject("Waypoint", typeof(Waypoint));
        obj.transform.SetParent(WaypointRoot);

        Waypoint newWaypoint = obj.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        obj.transform.position = selectedWaypoint.transform.position;
        obj.transform.forward = selectedWaypoint.transform.forward;
        newWaypoint.PreviousWaypoint = selectedWaypoint;

        if(selectedWaypoint.NextWaypoint != null)
        {
            selectedWaypoint.NextWaypoint.PreviousWaypoint = newWaypoint;
            newWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
        }
        selectedWaypoint.NextWaypoint = newWaypoint;
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex()+1);
        Selection.activeGameObject = obj;
        return newWaypoint;
    }

    public void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        if (selectedWaypoint.NextWaypoint != null)
        {
            selectedWaypoint.NextWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
        }
        if(selectedWaypoint.PreviousWaypoint != null)
        {
            selectedWaypoint.PreviousWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
            Selection.activeGameObject = selectedWaypoint.PreviousWaypoint.gameObject;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }
}
