using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationButton : MonoBehaviour
{
    public Button confirmationButton;
    public SOFightMoves SelectedMove;
    public int[] TargetedBodyParts = {0, 0, 0, 0, 0};

    [HideInInspector]
    public delegate void SendBool(bool state);
    public static SendBool AllTargetsSet;



    void OnEnable()
    {
        ActionMenuUI.AllTargetsSet += SetInteractable;
        ActionMenuUI.AllTargetsConfirmed += SetTargetedBodyParts;
        ActionButton.FightMoveSelected += SetSelectedMove;
        
    }

    void OnDisable()
    {
        ActionMenuUI.AllTargetsSet -= SetInteractable;
        ActionMenuUI.AllTargetsConfirmed -= SetTargetedBodyParts;
        ActionButton.FightMoveSelected -= SetSelectedMove;
    }

    void SetInteractable(bool state)
    {
        confirmationButton.interactable = state;
    } 

    void SetSelectedMove(SOFightMoves selectedMove)
    {
        SelectedMove = selectedMove;
    }

    void SetTargetedBodyParts(int[] targetedBodyParts)
    {
        TargetedBodyParts = targetedBodyParts;
    }
}
