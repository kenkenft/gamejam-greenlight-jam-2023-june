using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ColorTintButtonSetUp : MonoBehaviour
{
    public Image Icon;
    public Button UIButton;

    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
    }

    void SetUp()
    {
        // Debug.Log("ActionButton.SetUp called");
        Icon.color = Color.white; // Needs to be white, otherwise image colour overlaps with button's color tint transitions 
        SetUpButtonColours();
    }
    void SetUpButtonColours()
    {
        ColorBlock colorVar = UIButton.colors; 

        colorVar.normalColor = GameProperties.ColourPalleteRGBA["MediumGrey"];
        colorVar.highlightedColor = GameProperties.ColourPalleteRGBA["Special"]; 
        colorVar.pressedColor = GameProperties.ColourPalleteRGBA["DarkGrey"];
        colorVar.selectedColor = GameProperties.ColourPalleteRGBA["LightGrey"];
        colorVar.disabledColor = GameProperties.ColourPalleteRGBA["Black"];

        UIButton.colors = colorVar;
    }
}
