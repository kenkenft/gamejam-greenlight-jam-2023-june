using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubSystemUI : MonoBehaviour
{
    public CharacterDisplay Character;
    public Slider[] SubSystems; // Assumes 5 parts: head is index 0; chest is index 1; L.Arm is index 2; R.Arm is index 3; Legs are index 4. 
    public GameObject SubSystemDisplay;

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
        _isShowing = false;
        SubSystemDisplay.SetActive(false);
    }

    public void ToggleAndUpdateDisplay()
    {
        SubSystemDisplay.SetActive(!_isShowing);
        _isShowing = !_isShowing;
        if(_isShowing)
        {
            // ToDo update subsystem fill
            // Debug.Log("Showing display");
            UpdateSubSystemDisplay();
        }
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
            // Debug.Log($"Subsystem {i}: {subSystemsHealth[i].ToString("0.00")}");
        }
        
    }
}
