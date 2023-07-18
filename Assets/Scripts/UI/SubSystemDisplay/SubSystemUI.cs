using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubSystemUI : MonoBehaviour
{
    public CharacterDisplay Character;
    public Slider[] SubSystems; // Assumes 5 parts: head is index 0; chest is index 1; L.Arm is index 2; R.Arm is index 3; Legs are index 4. 
    public GameObject SubSystemDisplay;
    public ColorTintButtonSetUp ToggleButton;

    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
    }
    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
    }

    private bool _isShowing = false;

    void SetUp()
    {
        SubSystemDisplay.SetActive(true); //Enabled so that colour fill can be BoolRequested
        ToggleButton.SetUpButtonColours(GameProperties.ColorCombo.TargetIsNotSelected);

        foreach(Slider subSystem in SubSystems)
            subSystem.gameObject.GetComponent<SubSystemPart>().PlayerID = Character.HealthBarNum;
        SetUpBorderAndFill();
        _isShowing = false;
        SubSystemDisplay.SetActive(false);
    }

    public void ToggleAndUpdateDisplay()
    {
        SubSystemDisplay.SetActive(!_isShowing);
        _isShowing = !_isShowing;
        if(_isShowing)
        {
            ToggleButton.SetUpButtonColours(GameProperties.ColorCombo.TargetSelected);
            UpdateSubSystemDisplay();
        }
        else
            ToggleButton.SetUpButtonColours(GameProperties.ColorCombo.TargetIsNotSelected);
    }

    void UpdateSubSystemDisplay()
    {
        // Get health precentages
        // Apply percentages to fill
        float[] subSystemsHealth = {0f, 0f, 0f, 0f, 0f};

        for(int i = 0; i < subSystemsHealth.Length; i++)
        {
            subSystemsHealth[i] = Character.GetSystemHealthPercentage(i + 1);
            SubSystems[i].value = subSystemsHealth[i];
            SubSystems[i].gameObject.GetComponent<SubSystemPart>().Health = subSystemsHealth[i];
            
            // Debug.Log($"Subsystem {i}: {subSystemsHealth[i].ToString("0.00")}");
            if(subSystemsHealth[i] <= 0f)
                SetDisabledPart(i);
            else
                SubSystems[i].gameObject.GetComponent<SubSystemPart>().IsDestroyed = true;

        }
        
    }

    void SetDisabledPart(int subSystemIndex)
    {
        SubSystems[subSystemIndex].gameObject.GetComponent<SubSystemPart>().IsDestroyed = false;
        GameObject border = SubSystems[subSystemIndex].transform.Find("Border").gameObject;
        if(border != null)
        {
            border.GetComponent<Image>().color = GameProperties.ColourPalleteRGBA["Black"];
        }
    }

    void SetUpBorderAndFill()
    {
        GameObject tempObj;
        for(int i = 0; i < SubSystems.Length; i++)
        {
            tempObj = SubSystems[i].transform.Find("Border").gameObject;
            if(tempObj != null)
                tempObj.GetComponent<Image>().color = GameProperties.ColourPalleteRGBA["LightGrey"];
            tempObj = SubSystems[i].transform.Find("Fill").gameObject;
            if(tempObj != null)
                tempObj.GetComponent<Image>().color = GameProperties.ColourPalleteRGBA["LightBlue"];
        }
    }
}
