using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationButton : MonoBehaviour
{
    public Button confirmationButton;
    public SOFightMoves SelectedMove;
    // public int RelativeDirection = 1;
    public int[] TargetedBodyParts = {0, 0, 0, 0, 0}, 
                RelativeDirectionData = {0, 1};

    [HideInInspector]
    public static SendBool AllTargetsSet, ButtonPressed;
    public static FightMoveSent FightMoveConfirmed;
    public static SendIntArray AllTargetsConfirmed, RelativeDirectionConfirmed;



    void OnEnable()
    {
        ActionMenuUI.AllTargetsSet += SetInteractable;
        ActionMenuUI.AllTargetsConfirmed += SetTargetedBodyParts;
        ActionMenuUI.RelativeDirectionConfirmed += SetRelativeDirection;
        ActionButton.FightMoveSelected += SetSelectedMove;
    }

    void OnDisable()
    {
        ActionMenuUI.AllTargetsSet -= SetInteractable;
        ActionMenuUI.AllTargetsConfirmed -= SetTargetedBodyParts;
        ActionMenuUI.RelativeDirectionConfirmed -= SetRelativeDirection;
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

    void SetRelativeDirection(int relativeDirection)
    {
        // RelativeDirection = relativeDirection;
        RelativeDirectionData[1] = relativeDirection;
        
    }

    public void OnClick()
    {
        ButtonPressed?.Invoke(false);
        AllTargetsConfirmed?.Invoke(TargetedBodyParts);
        RelativeDirectionConfirmed?.Invoke(RelativeDirectionData);
        FightMoveConfirmed?.Invoke(SelectedMove);
    }
}
