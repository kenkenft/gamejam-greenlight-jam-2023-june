using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ColorTintButtonSetUp : MonoBehaviour
{
    public Image Icon;
    public Button UIButton;

    public GameProperties.ColorCombo TargetPalleteCombo;

    private Dictionary<GameProperties.ColorCombo, Color[]> PalleteCombo = new Dictionary<GameProperties.ColorCombo, Color[]>()
    {
        {GameProperties.ColorCombo.Standard, new [] {GameProperties.ColourPalleteRGBA["MediumGrey"], GameProperties.ColourPalleteRGBA["Special"], GameProperties.ColourPalleteRGBA["DarkGrey"],GameProperties.ColourPalleteRGBA["LightGrey"], GameProperties.ColourPalleteRGBA["Black"] }},
        {GameProperties.ColorCombo.TargetButtonIsDefault, new [] {GameProperties.ColourPalleteRGBA["MediumGrey"], GameProperties.ColourPalleteRGBA["Special"], GameProperties.ColourPalleteRGBA["DarkGrey"],GameProperties.ColourPalleteRGBA["LightGrey"], GameProperties.ColourPalleteRGBA["Special"] }},
        {GameProperties.ColorCombo.TargetButtonIsOptional, new [] {GameProperties.ColourPalleteRGBA["MediumGrey"], GameProperties.ColourPalleteRGBA["LightGrey"], GameProperties.ColourPalleteRGBA["DarkGrey"],GameProperties.ColourPalleteRGBA["Special"], GameProperties.ColourPalleteRGBA["Black"] }}
    };

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
        SetUpButtonColours(TargetPalleteCombo);
    }
    public void SetUpButtonColours(GameProperties.ColorCombo targetPallete = GameProperties.ColorCombo.Standard)
    {
        ColorBlock colorVar = UIButton.colors; 
        
        colorVar.normalColor = PalleteCombo[targetPallete][0];
        colorVar.highlightedColor = PalleteCombo[targetPallete][1];
        colorVar.pressedColor = PalleteCombo[targetPallete][2];
        colorVar.selectedColor = PalleteCombo[targetPallete][3];
        colorVar.disabledColor = PalleteCombo[targetPallete][4];

        // colorVar.normalColor = GameProperties.ColourPalleteRGBA["MediumGrey"];
        // colorVar.highlightedColor = GameProperties.ColourPalleteRGBA["Special"]; 
        // colorVar.pressedColor = GameProperties.ColourPalleteRGBA["DarkGrey"];
        // colorVar.selectedColor = GameProperties.ColourPalleteRGBA["LightGrey"];
        // colorVar.disabledColor = GameProperties.ColourPalleteRGBA["Black"];

        UIButton.colors = colorVar;
    }
}
