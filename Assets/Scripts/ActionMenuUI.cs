using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuUI : MonoBehaviour
{
    public GameObject CategoryTray, ActionTray, SubSystemTargetTray, DirectionTargetTray;

    public enum ActionCategories{
                            Attack, Defend, Move, Special 
                        };
    Dictionary<int, ActionCategories> IntToActionCategory = new Dictionary<int, ActionCategories>(){
                                                                                                        {0, ActionCategories.Attack},
                                                                                                        {1, ActionCategories.Defend},
                                                                                                        {2, ActionCategories.Move},
                                                                                                        {3, ActionCategories.Special}
                                                                                                    };

    public List<SOFightMoves> ActionListAttack = new List<SOFightMoves>();
    public List<SOFightMoves> ActionListDefense = new List<SOFightMoves>();
    public List<SOFightMoves> ActionListMove = new List<SOFightMoves>();
    public List<SOFightMoves> ActionListSpecial = new List<SOFightMoves>();

    void OnEnable()
    {
        CategoryButton.OnCategoryButtonPressed += ShowActionTray;
    }

    void OnDisable()
    {
        CategoryButton.OnCategoryButtonPressed -= ShowActionTray;
    }
    

    void SetUp()
    {
        // Hide ActionTray.
        // Hide SubsystemTargetTray.
        // Hide DirectionTargetTray.
        // Enable all category buttons.
    }

    public void ShowActionTray(int categoryID)
    {
        Debug.Log("Category button pressed: " + IntToActionCategory[categoryID]);
    }
}
