using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDirectionButton : MonoBehaviour
{
    public GameProperties.TargetDirection WhichDirection;
    public bool IsSelected = false, PreventSelfDisable = false;

    [HideInInspector]
    public delegate void SendInt(int data);
    public static SendInt DirectionButtonClicked; 
    public delegate void SendBool(bool state);
    public static SendBool CheckIsSelectedRequested;

    void OnEnable()
    {
        TargetDirectionButton.CheckIsSelectedRequested += ToggleSelectedState;
    }

    void OnDisable()
    {
        TargetDirectionButton.CheckIsSelectedRequested -= ToggleSelectedState;
    }

    public void OnClick()
    {
        IsSelected = !IsSelected;
        PreventSelfDisable = true;  // Prevents setting IsSelected to false when DirectionButtonClicked (on itself) is invoked.
        CheckIsSelectedRequested?.Invoke(IsSelected);
        if(IsSelected)
            DirectionButtonClicked?.Invoke((int)WhichDirection);
        else
            DirectionButtonClicked?.Invoke((int)GameProperties.TargetDirection.None);
    }

    void ToggleSelectedState(bool otherButtonIsSelected)
    {
        if(otherButtonIsSelected && !PreventSelfDisable)
            IsSelected = false;

        if(IsSelected)
                gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetSelected);
            else
                gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetIsNotSelected);

        PreventSelfDisable = false;
    }

}
