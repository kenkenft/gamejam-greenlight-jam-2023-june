using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuUI : MonoBehaviour
{
    public GameObject[] TrayUIs; // CategoryTray, ActionTray, SubSystemSelfTargetTray, SubSystemOpponentTargetTray, DirectionTargetTray;
    public Button[] CategoryButtons;
    public int SelectableSubSystemsCounter = 0;
    public int[] TargetedBodyParts = {0, 0, 0, 0, 0};
    public Text SelectableSubSystemsCounterText;

    public GameProperties.ActionCategories _actionCategories;
    Dictionary<int, GameProperties.ActionCategories> IntToActionCategory = new Dictionary<int, GameProperties.ActionCategories>(){
                                                                                                        {0, GameProperties.ActionCategories.Attack},
                                                                                                        {1, GameProperties.ActionCategories.Defend},
                                                                                                        {2, GameProperties.ActionCategories.Move},
                                                                                                        {3, GameProperties.ActionCategories.Special}
                                                                                                    };

    public List<SOFightMoves> ActionListAttack = new List<SOFightMoves>(), ActionListDefense = new List<SOFightMoves>(), 
                              ActionListMove = new List<SOFightMoves>(), ActionListSpecial = new List<SOFightMoves>();

    
    [HideInInspector] 
     public delegate void SOFightMovesRequired(List<SOFightMoves> relevantActions);
    public static SOFightMovesRequired ActionTrayActivated;
    
    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        CategoryButton.OnCategoryButtonPressed += ShowActionTray;
        ActionButton.FightMoveSelected += CheckTargetTray;
        TargetButton.TargetButtonClicked += SetSubSystemTarget;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        CategoryButton.OnCategoryButtonPressed -= ShowActionTray;
        ActionButton.FightMoveSelected -= CheckTargetTray;
        TargetButton.TargetButtonClicked -= SetSubSystemTarget;
    }
    

    void SetUp()
    {
        // Hide ActionTray, SubsystemTargetTray, DirectionTargetTray.
        ToggleActionTray(false);
        
        // Enable all category buttons.
        ToggleCategoryButtonInteractable(-1);
    }

    public void ShowActionTray(int actionCategory)
    {
        Debug.Log("Category button pressed: " + IntToActionCategory[actionCategory]);
        ToggleCategoryButtonInteractable(actionCategory);
        ToggleActionTray(true, IntToActionCategory[actionCategory]);
    }

    void ToggleActionTray(bool targetState, GameProperties.ActionCategories actionType = GameProperties.ActionCategories.None)
    {
        if(targetState && actionType != GameProperties.ActionCategories.None)
        {
            // Enable ActionTray
            TrayUIs[1].SetActive(true);

            List<SOFightMoves> relevantActions = GetRelevantActions(actionType);
            ActionTrayActivated?.Invoke(relevantActions);

            // Hide SubSystemSelfTargetTray, SubSystemOpponentTargetTray, and DirectionTargetTray
            DisableTraysCascade(2);
        }
        else
        {
            // Hide ActionTray, SubSystemSelfTargetTray, SubSystemOpponentTargetTray, and DirectionTargetTray
            DisableTraysCascade(1);
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
        Debug.Log("Disabling from tray: " + TrayUIs[layerStart].name);
        for(int i = layerStart; i < TrayUIs.Length; i++)
            {
                if(TrayUIs[i].activeSelf)
                    TrayUIs[i].SetActive(false);
            }
    }

    void ToggleCategoryButtonInteractable(int buttonType)
    {
        // Will iterate through category button's enum variable ActionType. If the button does not match, then re-enable. 
        // Otherwise, disable the button. Therefore, to enable ALL buttons, send -1.
        for(int i = 0; i < CategoryButtons.Length; i++)
        {
            if((int)CategoryButtons[i].gameObject.GetComponent<CategoryButton>().ActionType != buttonType)
                CategoryButtons[i].interactable = true;
            else
                CategoryButtons[i].interactable = false;
        }
    }

    void CheckTargetTray(SOFightMoves selectedMove)
    {
        Debug.Log("CheckTargetTray called");
        switch(selectedMove.MainTarget)
        {
            case GameProperties.TargetType.Self:
            {
                Debug.Log("Move target: " + selectedMove.MainTarget);
                EnableTargetTrays(2);
                SetUpTargetTrayDefaults(2, selectedMove);    // Set up default targets, and selectable body parts counter
                break;
            }
            case GameProperties.TargetType.Opponent:
            {
                Debug.Log("Move target: " + selectedMove.MainTarget);
                EnableTargetTrays(3);
                SetUpTargetTrayDefaults(3, selectedMove);
                break;
            }
            case GameProperties.TargetType.Grid:
            {
                Debug.Log("Move target: " + selectedMove.MainTarget);
                EnableTargetTrays(4);
                SetUpTargetTrayDefaults(4, selectedMove);
                break;
            }
            case GameProperties.TargetType.None:
            {
                Debug.Log("Move target: " + selectedMove.MainTarget);
                EnableTargetTrays();
                break;
            }
            default:
                break;
        } 
    }

    void EnableTargetTrays(int targetTrayIndex = -1)
    {
        for(int i = 2; i < TrayUIs.Length; i++)
        {
            TrayUIs[i].SetActive(i == targetTrayIndex);
            // Debug.Log( TrayUIs[i].name + ": " + TrayUIs[i].activeSelf);
        }
    }

    Text FindCounterText(GameObject tray)
    {
        Text[] texts = tray.gameObject.GetComponentsInChildren<Text>();
        foreach(Text text in texts)
        {
            if(text.name == "Counter")
                return text;
        }
        return null;
    }

    void SetUpTargetTrayDefaults(int targetTrayIndex, SOFightMoves selectedMove)
    {
        SelectableSubSystemsCounter = selectedMove.HasExtraTargets[1];
        SelectableSubSystemsCounterText = FindCounterText(TrayUIs[targetTrayIndex]);
        SetCounterText();

        // ToDo Select default targets
    }

    void SetSubSystemTarget(int[] targetData)
    {
        // Assumes targetData is an array of size 2. Index 0 is the subsystem part; index 1 is a truthy integer
        TargetedBodyParts[targetData[0]] = targetData[1];
        if(targetData[1] == 1)
            SelectableSubSystemsCounter--;
        else
            SelectableSubSystemsCounter++;
        
        SetCounterText();
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

}
