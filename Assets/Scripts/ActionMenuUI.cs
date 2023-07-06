using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuUI : MonoBehaviour
{
    public GameObject[] TrayUIs; // CategoryTray, ActionTray, SubSystemTargetTray, DirectionTargetTray;
    public Button[] CategoryButtons;

    public enum ActionCategories{
                            Attack, Defend, Move, Special, None 
                        };
    Dictionary<int, ActionCategories> IntToActionCategory = new Dictionary<int, ActionCategories>(){
                                                                                                        {0, ActionCategories.Attack},
                                                                                                        {1, ActionCategories.Defend},
                                                                                                        {2, ActionCategories.Move},
                                                                                                        {3, ActionCategories.Special}
                                                                                                    };

    public List<SOFightMoves> ActionListAttack = new List<SOFightMoves>(), ActionListDefense = new List<SOFightMoves>(), 
                              ActionListMove = new List<SOFightMoves>(), ActionListSpecial = new List<SOFightMoves>();

    
    [HideInInspector] 
    public delegate void SOFightMovesRequired(List<SOFightMoves> relevantActions);
    public static SOFightMovesRequired ActionTrayActived;
    
    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        CategoryButton.OnCategoryButtonPressed += ShowActionTray;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted += SetUp;
        CategoryButton.OnCategoryButtonPressed -= ShowActionTray;
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

    void ToggleActionTray(bool targetState, ActionCategories actionType = ActionCategories.None)
    {
        if(targetState && actionType != ActionCategories.None)
        {
            // Enable ActionTray
            TrayUIs[1].SetActive(true);
            // ToDo - Load relevant attack moves and buttons based on ActionType
            List<SOFightMoves> relevantActions = GetRelevantActions(actionType);

            ActionTrayActived?.Invoke(relevantActions);
                //ToDo - Set Buttons to show the list of relevant actions
                // Hide leftover action buttons

            // Hide SubSystemTargetTray and DirectionTargetTray
            DisableTraysCascade(2);
        }
        else
        {
            // Hide ActionTray, SubSystemTargetTray and DirectionTargetTray
            DisableTraysCascade(1);
        }
    }

    List<SOFightMoves> GetRelevantActions(ActionCategories actionType)
    {
        List<SOFightMoves> relevantActions = new List<SOFightMoves>();
        // Get appropriate move List
        switch(actionType)
        {
            case ActionCategories.Attack:
            {
                relevantActions = ActionListAttack;
                break;
            }
            case ActionCategories.Defend:
            {
                relevantActions = ActionListDefense;
                break;
            }
            case ActionCategories.Move:
            {
                relevantActions = ActionListMove;
                break;
            }
            case ActionCategories.Special:
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
}
