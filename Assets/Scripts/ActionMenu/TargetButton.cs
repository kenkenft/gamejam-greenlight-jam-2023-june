using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetButton : MonoBehaviour
{
    public GameProperties.SubSystem WhichSubSystem;
    public bool IsDefaultTarget = false, IsSelected = false;
    public Button TargettingButton;

    [HideInInspector]
    public delegate void SendIntArray(int[] data);
    public static SendIntArray TargetButtonClicked; 
    public void OnClick()
    {
        if(!IsDefaultTarget)
        {
            // Debug.Log("Button clicked: " + WhichSubSystem);
            if(!IsSelected)
            {
                IsSelected = true;
                int[] targetData = {(int)WhichSubSystem, 1};
                TargetButtonClicked?.Invoke(targetData);
            }
            else
            {
                IsSelected = false;
                int[] targetData = {(int)WhichSubSystem, 0};
                TargetButtonClicked?.Invoke(targetData);
            }
        }
    }
}
