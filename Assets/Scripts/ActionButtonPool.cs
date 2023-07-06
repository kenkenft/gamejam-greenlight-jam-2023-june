using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonPool : MonoBehaviour
{
    void OnEnable()
    {
        ActionMenuUI.ActionTrayActived += SetUpRelevantActionButtons;
    }

    void OnDisable()
    {
        ActionMenuUI.ActionTrayActived -= SetUpRelevantActionButtons;
    }

    void SetUpRelevantActionButtons(List<SOFightMoves> relevantActions)
    {
        foreach(SOFightMoves fightMove in relevantActions)
        {
            Debug.Log("Action name: " + fightMove.Name);
        }
    }
}
