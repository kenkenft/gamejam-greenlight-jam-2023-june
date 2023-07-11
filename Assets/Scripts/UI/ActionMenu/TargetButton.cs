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
    public static SendIntArray TargetButtonClicked; 
    public void OnClick()
    {
        if(!IsDefaultTarget)
        {
            // Debug.Log("Button clicked: " + WhichSubSystem);
            int[] targetData = new int[2];
            if(!IsSelected)
            {
                targetData = new int[] {(int)WhichSubSystem, 1};
                gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetSelected);
            }
            else
            {
                targetData = new int[] {(int)WhichSubSystem, 0};
                gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetIsNotSelected);                
            }
            IsSelected = !IsSelected;
            TargetButtonClicked?.Invoke(targetData);
        }
    }
}
