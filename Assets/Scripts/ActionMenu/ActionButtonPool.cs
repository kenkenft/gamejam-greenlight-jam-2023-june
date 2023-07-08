using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonPool : MonoBehaviour
{
    
    public List<ActionButton> ButtonPool;
    
    void OnEnable()
    {
        ActionMenuUI.ActionTrayActivated += SetUpRelevantActionButtons;
    }

    void OnDisable()
    {
        ActionMenuUI.ActionTrayActivated -= SetUpRelevantActionButtons;
    }

    void SetUpRelevantActionButtons(List<SOFightMoves> relevantActions)
    {
        int relevantActionsAmount = relevantActions.Count, ButtonPoolAmount = ButtonPool.Count; 
        for(int i = 0; i < ButtonPoolAmount; i++)
        {
            if(i < relevantActionsAmount)
            {    
                ButtonPool[i].FightMove = relevantActions[i];
                ButtonPool[i].SetUpActionButton();
                ButtonPool[i].gameObject.SetActive(true);
            }
            else
            {
                ButtonPool[i].gameObject.SetActive(false);
            }
        }
    }
}
