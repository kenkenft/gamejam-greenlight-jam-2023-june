using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UIButtonType
{
    Category, Action, TargetSubSystem, TargetGrid, DisplaySubSystem
};
public class ToolipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // public static System.Action<GameObject, PointerEventData> onPointerEnter;
    // public static System.Action<GameObject> onPointerExit;

    public ActionButton AB;
    public SubSystemPart SSB;
    public UIButtonType ButtonType;

    public string Content, Header;
    private string[,] subSystemStates = 
                    {
                        {
                            "Critical; Repair before permanent damage is sustained",
                            "Heavy damage; Repair before permanent damage is sustained", 
                            "Moderate damage; Repairable",  "Minor damage; Repairable",
                            "Optimal; Repairable"
                        },
                        {
                            "Critically damaged","Heavily wounded","Moderate injuries","Minor injuries","Healthy"
                        }
                    };
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("ShowToolTip called");
        SetStringsByButtonType();
        
        
        // foreach(GameObject item in eventData.hovered)
        //     Debug.Log($"{item.name}");
        TooltipSystem.Show(Content, Header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("HideToolTip called");
        TooltipSystem.Hide();
    }

    void SetStringsByButtonType()
    {
        switch(ButtonType)
        {
            case UIButtonType.Action:
            {
                Header = AB.FightMove.Name;
                Content = AB.FightMove.Description;
                break;
            }
            case UIButtonType.DisplaySubSystem:
            {
                string partState = GetSubSystemState();
                string fullStatus = "";
                if(SSB.PlayerID == 0)
                    fullStatus =  "Status: " + partState;
                else
                    fullStatus = "Analysis: " + partState;
                Header = SSB.Name;
                Content = $"{SSB.Health.ToString("0.00")}%\n{fullStatus}";
                break;
            }
        }
    }

    string GetSubSystemState()
    {
        float[] healthBoundaries = {0f, 25f, 50f, 75f, 99f, 100f}; //{100f, 99f, 75f, 50f, 25f, 0f};
        float health = SSB.Health;
        int playerID = SSB.PlayerID;

        for(int i = 0; i < healthBoundaries.Length-1; i++)
        {
            if(health > healthBoundaries[i] && health <= healthBoundaries[i + 1])
                return subSystemStates[playerID, i];
        }

        if(health <= 0f)
        {   
            if(playerID == 0) 
                return "Disabled; unrepairable";
            else
                return "Crippled";
        }

        return "Unknown";
    }

}
