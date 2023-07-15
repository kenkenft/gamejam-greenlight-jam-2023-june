using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // public static System.Action<GameObject, PointerEventData> onPointerEnter;
    // public static System.Action<GameObject> onPointerExit;

    public ActionButton actionButton;

    public string Content, Header;
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("ShowToolTip called");
        Content = actionButton.FightMove.Description;
        Header = actionButton.FightMove.Name;
        
        foreach(GameObject item in eventData.hovered)
            Debug.Log($"{item.name}");
        TooltipSystem.Show(Content, Header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("HideToolTip called");
        TooltipSystem.Hide();
    }

}
