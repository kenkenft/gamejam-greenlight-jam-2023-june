using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public SOFightMoves FightMove;
    public Image FightMoveImage;
    public Button FightMoveButton;

    [HideInInspector] 
    public delegate void FightMoveSent(SOFightMoves fightMove);
    public static FightMoveSent FightMoveSelected;

    public delegate void OnSomeEvent();
    public static OnSomeEvent MoveSelected;
    
    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        MatchFlowManager.ButtonStatusUpdated += AbleToDoMove;
        UIManager.DisableButtonsRequested += DisableButton;

    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        MatchFlowManager.ButtonStatusUpdated -= AbleToDoMove;
        UIManager.DisableButtonsRequested -= DisableButton;
    }

    void SetUp()
    {
        Debug.Log("ActionButton.SetUp called");
        FightMoveImage.color = Color.white; // Needs to be white, otherwise image colour overlaps with button's color tint transitions 
        SetUpButtonColours();
    }

    public void SendSelectedMove()
    {
        // SFXRequested?.Invoke("Click");
        MoveSelected?.Invoke(); // Should invoke UIManager.DisableAllMoveButtons()
        FightMoveSelected?.Invoke(FightMove); // Should invoke MatchFlowManager.SetPlayerMove()
    }

    void AbleToDoMove(int[] playerEnergyAndSubSystemsData)
    {
        Debug.Log("Button: " + gameObject.name);
        // playerEnergyAndSubSystemsData // index 0 is the amount of energy available; indexes 1 through 5 are truthy values (0 is broken; 1 is not broken)
        bool isRequirementMet = true;
        // Check if enough energy is available
        if(playerEnergyAndSubSystemsData[0] < FightMove.Requirements[0])
        {    
            Debug.Log("storedEnergy " + playerEnergyAndSubSystemsData[0] + " < required energy" + FightMove.Requirements[0] + ".Not enough energy for fightMove: " + FightMove.Name);
            isRequirementMet = false;
        }

        //Check if required subsystems are still active
        isRequirementMet = CheckSubSystems(playerEnergyAndSubSystemsData,  isRequirementMet);

        if(isRequirementMet)
        {    
            Debug.Log("Requirement MET for fightmove: " + FightMove.Name);
            // FightMoveImage.color = GameProperties.ColourPalleteRGBA["Special"];
            FightMoveButton.interactable = true;
        }
        else
        {
            Debug.Log("Requirement NOT MET for fightmove: " + FightMove.Name);
            // FightMoveImage.color = GameProperties.ColourPalleteRGBA["DarkGrey"];
            FightMoveButton.interactable = false;
        }
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
                    Debug.Log("Mandatory subsytem broken for fightMove: " + FightMove.Name);
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
            Debug.Log("All candidate subsytems broken for fightMove: " + FightMove.Name);
            return false;   // i.e. All valid subsystem candidate that could be used for the flexible requirement are broken
        }
        return true;    // i.e All mandatory subsystems are active
    }   // End of CheckSubSystemRequirement

    public void DisableButton()
    {
        FightMoveButton.interactable = false;
    }

    void SetUpButtonColours()
    {
        ColorBlock colorVar = FightMoveButton.colors; 

        colorVar.normalColor = GameProperties.ColourPalleteRGBA["MediumGrey"];
        colorVar.highlightedColor = GameProperties.ColourPalleteRGBA["Special"]; 
        colorVar.pressedColor = GameProperties.ColourPalleteRGBA["DarkGrey"];
        colorVar.selectedColor = GameProperties.ColourPalleteRGBA["LightGrey"];
        colorVar.disabledColor = GameProperties.ColourPalleteRGBA["Black"];

        FightMoveButton.colors = colorVar;
    }
}
