using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationButton : MonoBehaviour
{
    public Button confirmationButton;

    [HideInInspector]
    public delegate void SendBool(bool state);
    public static SendBool AllTargetsSet;

    void OnEnable()
    {
        ActionMenuUI.AllTargetsSet += SetInteractable;
    }

    void OnDisable()
    {
        ActionMenuUI.AllTargetsSet -= SetInteractable;
    }

    void SetInteractable(bool state)
    {
        confirmationButton.interactable = state;
    } 
}
