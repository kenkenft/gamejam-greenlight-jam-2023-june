using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : MonoBehaviour
{
    public GameProperties.SubSystem WhichSubSystem;

    [HideInInspector]
    public delegate void SendInt(int data);
    public static SendInt TargetButtonClicked; 
    public void OnClick()
    {
        Debug.Log("Button clicked: " + WhichSubSystem);
    }
}
