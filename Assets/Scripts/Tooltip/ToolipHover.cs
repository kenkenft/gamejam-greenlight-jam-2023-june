using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // public static System.Action<GameObject, PointerEventData> onPointerEnter;
    // public static System.Action<GameObject> onPointerExit;

    public string Content, Header;
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("ShowToolTip called");
        TooltipSystem.Show(Content, Header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("HideToolTip called");
        TooltipSystem.Hide();
    }

}
