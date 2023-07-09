using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDirectionButton : MonoBehaviour
{
    public GameProperties.TargetDirection WhichDirection;
    public bool IsSelected = false;

    [HideInInspector]
    public delegate void SendInt(int data);
    public static SendInt DirectionButtonClicked; 

    public void OnClick()
    {
        IsSelected = !IsSelected;
        DirectionButtonClicked?.Invoke((int)WhichDirection);
        Debug.Log("TargetDirection clicked: " + WhichDirection + "," + (int)WhichDirection);
    }
}
