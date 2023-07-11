using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public SOFightMoves FightMove;
    public Image FightMoveImage;
    public Button FightMoveButton;

    public bool IsSelected = false, PreventSelfDisable = false, AreRequirementsMet = false;

    [HideInInspector] 
    
    public static FightMoveSent FightMoveSelected;
    public static SendBool CheckIsSelectedRequested;
    public static IntForBool CurrentBuffRequested; 
    
    void OnEnable()
    {
        ActionButton.CheckIsSelectedRequested += ToggleSelectedState;
        MatchFlowManager.ButtonStatusUpdated += AbleToDoMove;
    }

    void OnDisable()
    {
        ActionButton.CheckIsSelectedRequested -= ToggleSelectedState;
        MatchFlowManager.ButtonStatusUpdated -= AbleToDoMove;
    }


    public void SendSelectedMove()
    {
        // SFXRequested?.Invoke("Click");
        IsSelected = !IsSelected;
        PreventSelfDisable = true;
        CheckIsSelectedRequested?.Invoke(IsSelected);
        FightMoveSelected?.Invoke(FightMove); // Should invoke MatchFlowManager.SetPlayerMove() // SetPlayerMove() will be invoked elswhere
    }

    void AbleToDoMove(int[] playerEnergyAndSubSystemsData)
    {
        // playerEnergyAndSubSystemsData // index 0 is the amount of energy available; indexes 1 through 5 are truthy values (0 is broken; 1 is not broken)
        bool hasEnoughEnergy = true;
        // Check if enough energy is available
        if(playerEnergyAndSubSystemsData[0] < FightMove.Requirements[0])   
            hasEnoughEnergy = false;

        //Check if required subsystems are still active
        AreRequirementsMet = CheckSubSystems(playerEnergyAndSubSystemsData, hasEnoughEnergy);
        
        //For Buff/Special moves, check whether corresponding buff is already active before enabling button interactable to true 
        if(FightMove.ActionType == GameProperties.ActionType.Special)
            FightMoveButton.interactable = !CurrentBuffRequested.Invoke(FightMove.MainEffectValue[0]);
        else    
            FightMoveButton.interactable = AreRequirementsMet;
    }

    bool CheckSubSystems(int[] playerEnergyAndSubSystemsData, bool isRequirementMet)
    {
        if(isRequirementMet)
        {
            List<int> mandatoryRequirements = new List<int>(), flexibleRequirements = new List<int>();
            // Parse list into mandatory and flexible
            for(int i = 1 ; i < playerEnergyAndSubSystemsData.Length ; i++)
            {
                // int index = i - 1;
                switch(FightMove.Requirements[i])
                {
                    case 1:
                    {
                        mandatoryRequirements.Add(i);
                        break;
                    }
                    case 2:
                    {
                        flexibleRequirements.Add(i);
                        break;
                    }
                    default:
                        break;
                }
            }

            // Check for mandatory subsystems i.e. playerEnergyAndSubSystemsData[i] = 1
            isRequirementMet = CheckSubSystemRequirement(playerEnergyAndSubSystemsData, mandatoryRequirements, true);
            
            // Check for requirements where it requires only one of the arms i.e. playerEnergyAndSubSystemsData[i] = 2
            if(isRequirementMet && flexibleRequirements.Count > 0)
                isRequirementMet = CheckSubSystemRequirement(playerEnergyAndSubSystemsData, flexibleRequirements, false);
        } 

        return isRequirementMet;
    }   // End of CheckSubSystems

    bool CheckSubSystemRequirement(int[] playerEnergyAndSubSystemsData, List<int> requirements, bool isMandatory)
    {
        if(isMandatory)
        {
            for(int i = 0; i < requirements.Count ; i++)
            {
                int index = requirements[i];
                if(playerEnergyAndSubSystemsData[index] == 0)
                {    
                    // Debug.Log("Mandatory subsytem broken for fightMove: " + FightMove.Name);
                    return false;   // i.e. At least one mandatory subsystem is broken
                }
            }
        }
        else
        {
            for(int i = 0; i < requirements.Count ; i++)
            {
                int index = requirements[i];
                if(playerEnergyAndSubSystemsData[index] == 1)
                    return true;    // i.e. At least one of the valid subsystem candidates is available for the flexible requirement
            }
            // Debug.Log("All candidate subsytems broken for fightMove: " + FightMove.Name);
            return false;   // i.e. All valid subsystem candidate that could be used for the flexible requirement are broken
        }
        return true;    // i.e All mandatory subsystems are active
    }   // End of CheckSubSystemRequirement

    public void DisableButton()
    {
        FightMoveButton.interactable = false;
    }

    public void SetUpActionButton()
    {
        FightMoveImage.sprite = FightMove.Icon;
        gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetIsNotSelected);
        IsSelected = false;
        FightMoveButton.interactable = true;
    }

    void ToggleSelectedState(bool otherButtonIsSelected)
    {
        if(otherButtonIsSelected && !PreventSelfDisable)
            IsSelected = false;

        if(IsSelected)
        {
            gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetSelected);
            FightMoveButton.interactable = false;
        }
        else
        {
            gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetIsNotSelected);
            if(FightMove.ActionType == GameProperties.ActionType.Special)
                FightMoveButton.interactable = !CurrentBuffRequested.Invoke(FightMove.MainEffectValue[0]);
            else
                FightMoveButton.interactable = AreRequirementsMet;
        }

        PreventSelfDisable = false;
    }

}
