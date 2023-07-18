using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubSystemUI : MonoBehaviour
{
    public CharacterDisplay Character;
    public Slider[] SubSystems; // Assumes 5 parts: head is index 0; chest is index 1; L.Arm is index 2; R.Arm is index 3; Legs are index 4. 
    public GameObject SubSystemDisplay;

}
