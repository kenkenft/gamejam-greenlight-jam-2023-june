using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuUI : MonoBehaviour
{
    public CharacterDisplay Player;
    public GameObject[] TrayUIs; // CategoryTray, ActionTray, SubSystemSelfTargetTray, SubSystemOpponentTargetTray, DirectionTargetTray;
    
    public GameObject ActiveTargetTray = null;

    public Button[] CategoryButtons;
    public int SelectableSubSystemsCounter = 0;
    public int[] TargetedBodyParts = {0, 0, 0, 0, 0};
    public Text SelectableSubSystemsCounterText;

    public GameProperties.ActionCategories _actionCategories;

    public List<SOFightMoves> ActionListAttack = new List<SOFightMoves>(), ActionListDefense = new List<SOFightMoves>(), 
                              ActionListMove = new List<SOFightMoves>(), ActionListSpecial = new List<SOFightMoves>();

    
    [HideInInspector] 
    public static SOFightMovesRequired ActionTrayActivated;
    public static SendBool AllTargetsSet;
    public static SendInt RelativeDirectionConfirmed;
    public static SendIntArray AllTargetsConfirmed;
    
    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        CategoryButton.OnCategoryButtonPressed += ShowActionTray;
        ActionButton.FightMoveSelected += CheckTargetTray;
        TargetButton.TargetButtonClicked += SetSubSystemTarget;
        TargetDirectionButton.DirectionButtonClicked += SetMoveDirection;
        ConfirmationButton.ButtonPressed += CloseOtherTrays;
        MatchFlowManager.EndConditionsMet += ToggleCategoryButtonInteractable;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        CategoryButton.OnCategoryButtonPressed -= ShowActionTray;
        ActionButton.FightMoveSelected -= CheckTargetTray;
        TargetButton.TargetButtonClicked -= SetSubSystemTarget;
        TargetDirectionButton.DirectionButtonClicked -= SetMoveDirection;
        ConfirmationButton.ButtonPressed -= CloseOtherTrays;
        MatchFlowManager.EndConditionsMet -= ToggleCategoryButtonInteractable;
    }
    

    void SetUp()
    {
        ToggleActionTray(false);    // Hide ActionTray, SubSystemSelfTargetTray, SubSystemOpponentTargetTray, and DirectionTargetTray
        ToggleCategoryButtonInteractable(-1);   // Enable all category buttons.
        AllTargetsSet?.Invoke(false);   // Disable confirmation button.
    }

    public void ShowActionTray(int actionCategory)
    {
        _actionCategories = (GameProperties.ActionCategories)actionCategory;
        ToggleCategoryButtonInteractable((int)_actionCategories);
        ToggleActionTray(true, _actionCategories);
    }

    void CloseOtherTrays(bool state)
    {
        ToggleActionTray(state, _actionCategories);
    }

    void ToggleActionTray(bool targetState, GameProperties.ActionCategories actionType = GameProperties.ActionCategories.None)
    {
        if(targetState && actionType != GameProperties.ActionCategories.None)
        {
            TrayUIs[1].SetActive(true);   // Enable ActionTray
            List<SOFightMoves> relevantActions = GetRelevantActions(actionType);
            ActionTrayActivated?.Invoke(relevantActions);

            DisableTraysCascade(2); // Hide SubSystemSelfTargetTray, SubSystemOpponentTargetTray, and DirectionTargetTray
        }
        else
        {
            // Hide ActionTray, SubSystemSelfTargetTray, SubSystemOpponentTargetTray, and DirectionTargetTray
            DisableTraysCascade(1);
            ToggleCategoryButtonInteractable(-1);
            AllTargetsSet.Invoke(false);
        }
    }

    List<SOFightMoves> GetRelevantActions(GameProperties.ActionCategories actionType)
    {
        List<SOFightMoves> relevantActions = new List<SOFightMoves>();
        // Get appropriate move List
        switch(actionType)
        {
            case GameProperties.ActionCategories.Attack:
            {
                relevantActions = ActionListAttack;
                break;
            }
            case GameProperties.ActionCategories.Defend:
            {
                relevantActions = ActionListDefense;
                break;
            }
            case GameProperties.ActionCategories.Move:
            {
                relevantActions = ActionListMove;
                break;
            }
            case GameProperties.ActionCategories.Special:
            {
                relevantActions = ActionListSpecial;
                break;
            }
            default:
                break;
        }
        return relevantActions;
    }   // End of SetRelevantActionButtons

    void DisableTraysCascade(int layerStart)
    {
        for(int i = layerStart; i < TrayUIs.Length; i++)
            {
                if(TrayUIs[i].activeSelf)
                    TrayUIs[i].SetActive(false);
            }
    }

    void ToggleCategoryButtonInteractable(int buttonType)
    {
        // Will iterate through category button's enum variable ActionType. If the button does not match, then re-enable. 
        // Otherwise, disable the button. Therefore, to enable ALL buttons, send -1. To disable all buttons, send -2.
        bool areAllButtonsToBeDisabled = buttonType == -2;

        for(int i = 0; i < CategoryButtons.Length; i++)
        {
            if((int)CategoryButtons[i].gameObject.GetComponent<CategoryButton>().ActionType != buttonType && !areAllButtonsToBeDisabled) // Category button not selected but is still interactable
                CategoryButtons[i].interactable = true; 
            else if (areAllButtonsToBeDisabled) //All category buttons are to be disabled
                CategoryButtons[i].interactable = false;
            else // This category button has been clicked, therefore disable interactable
                CategoryButtons[i].interactable = false; 
        }
    }

    void CheckTargetTray(SOFightMoves selectedMove)
    {
        ResetTargetedBodyParts();   // Reset TargetedBodyParts array on clicking an ActionButton
        int targetTrayIndex = (int)selectedMove.MainTarget + 2; // Targetting trays are at indexes 2, 3, and 4
        ToggleTargetTrays(targetTrayIndex);
        SetUpTargetTrayDefaults(selectedMove);    // Set up default targets, and selectable body parts counter
        SetDefaultTargets(selectedMove);
        ToggleButtonsInteractable();
        DisabledTargetIfZeroHealth(selectedMove);
    }
    void ResetTargetedBodyParts()
    {
        for(int i = 0; i < TargetedBodyParts.Length; i++)
            TargetedBodyParts[i] = 0;
    }

    void ToggleTargetTrays(int targetTrayIndex = -1)
    {
        bool isFound = false;
        for(int i = 2; i < TrayUIs.Length; i++)
        {
            TrayUIs[i].SetActive(i == targetTrayIndex);
            // Debug.Log( TrayUIs[i].name + ": " + TrayUIs[i].activeSelf);
            if(i == targetTrayIndex)
            {
                ActiveTargetTray = TrayUIs[i];
                isFound = true;
            }
        }

        if(!isFound)
            ActiveTargetTray = null;
    }

    void SetUpTargetTrayDefaults(SOFightMoves selectedMove)
    {
        SelectableSubSystemsCounter = selectedMove.HasExtraTargets[1];
        SelectableSubSystemsCounterText = FindCounterText(ActiveTargetTray);
        SetCounterText();
        
    }

    Text FindCounterText(GameObject tray)
    {
        if(tray != null)
        {
            Text[] texts = tray.gameObject.GetComponentsInChildren<Text>();
            foreach(Text text in texts)
            {
                if(text.name == "Counter")
                    return text;
            }
        }
        return null;
    }

    void SetSubSystemTarget(int[] targetData)
    {
        // Debug.Log($"ActionMenuUI.SetSubSystemTarget {(GameProperties.SubSystem)targetData[0]}: {targetData[1]}");
        TargetedBodyParts[targetData[0]] = targetData[1];  // Assumes targetData is an array of size 2. Index 0 is the subsystem part; index 1 is a truthy integer
        if(targetData[1] == 1)
            SelectableSubSystemsCounter--;
        else
            SelectableSubSystemsCounter++;
        ToggleButtonsInteractable();
        SetCounterText();
        if(SelectableSubSystemsCounter == 0)
            AllTargetsConfirmed?.Invoke(TargetedBodyParts);    // Send TargetedBodyParts to ConfirmationButton

    }

    void ToggleButtonsInteractable()
    {
        bool areTargetsRemaining = SelectableSubSystemsCounter > 0; 
        if(ActiveTargetTray != null)
        {
            TargetButton[] buttons = ActiveTargetTray.GetComponentsInChildren<TargetButton>();   
            // For target buttons that are not default targets and are no already selected, sets button.interactable state to true 
            // if there's there's still requried targets to select. Otherwise set to false to prevent additional selections.
            foreach(TargetButton button in buttons)
            {
                if(!button.IsDefaultTarget && !button.IsSelected)
                    button.TargettingButton.interactable = areTargetsRemaining;
            }
        }
        AllTargetsSet?.Invoke(!areTargetsRemaining);  //Enable confirmation button if no more targets, else disable button until all targets selected.

    }

    void SetCounterText()
    {
        if(SelectableSubSystemsCounterText != null)
        {
            if(SelectableSubSystemsCounter > 1)
                SelectableSubSystemsCounterText.text = "Select " + SelectableSubSystemsCounter + " more subsystems.";
            else if(SelectableSubSystemsCounter == 1)
                SelectableSubSystemsCounterText.text = "Select " + SelectableSubSystemsCounter + " more subsystem.";
            else
                SelectableSubSystemsCounterText.text = "Targeting complete. Please confirm selection";
        }
    }

    void SetDefaultTargets(SOFightMoves selectedMove)
    {
        if(ActiveTargetTray != null)
        {
            TargetButton[] buttons = ActiveTargetTray.GetComponentsInChildren<TargetButton>();
            bool isDefaultTarget = false;
            foreach(TargetButton button in buttons)
            {
                isDefaultTarget = selectedMove.DefaultSubSystemTargets[(int)button.WhichSubSystem] == 1;
                button.IsDefaultTarget = isDefaultTarget;
                button.IsSelected = isDefaultTarget;
                button.TargettingButton.interactable = !isDefaultTarget;
                if(isDefaultTarget)
                    button.gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetButtonIsDefault);
                else
                    button.gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetIsNotSelected);
            } 
        }
    }

    void SetMoveDirection(int relativeDirection)
    {
        // Debug.Log("TargetDirection clicked: " + relativeDirection);
        bool isDirectionSet = relativeDirection != 0;
        AllTargetsSet?.Invoke(isDirectionSet);

        if(isDirectionSet)
            RelativeDirectionConfirmed?.Invoke(relativeDirection);
    }

    void DisabledTargetIfZeroHealth(SOFightMoves selectedMove)
    {
        // This only applies to Healing type moves, as repairing destroyed parts is not allowed
        // If true, then disable any target buttons whose health is zero
        if(selectedMove.ActionType == GameProperties.ActionType.Repair && ActiveTargetTray != null)
        {
            // If true, then disable any target buttons whose health is zero
            TargetButton[] buttons = ActiveTargetTray.GetComponentsInChildren<TargetButton>();
            bool isSubSystemActive = true;
            foreach(TargetButton button in buttons)
            {
                isSubSystemActive = Player.GetSystemHealthPercentage((int)button.WhichSubSystem + 1) > 0f;
                if(!isSubSystemActive)
                {    
                    button.TargettingButton.interactable = isSubSystemActive;
                    button.gameObject.GetComponent<ColorTintButtonSetUp>().SetUpButtonColours(GameProperties.ColorCombo.TargetIsNotSelected);
                    TargetedBodyParts[(int)button.WhichSubSystem] = 0;
                }
            } 
        }
    }
}
