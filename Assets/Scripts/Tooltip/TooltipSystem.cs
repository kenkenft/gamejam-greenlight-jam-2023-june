using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem _current;
    public TooltipManager Tooltip;
    public void Awake()
    {
        _current = this;
        Hide();
    }

    public static void Show(string content, string header = "")
    {
        _current.Tooltip.SetText(content, header);
        _current.Tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        _current.Tooltip.gameObject.SetActive(false);
    }
}
