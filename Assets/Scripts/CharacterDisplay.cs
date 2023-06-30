using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    public int HealthBarNum;
    private int _subSystemAmount;
    public int[] HealthSystemData = new int[12], 
    // Assumes HealthSystemData is as follows: 
    // [HPMax, HPCurr, HPHeadMax, HPHeadCurr, 
    // HPChestMax, HPChestCurr, HPLeftArmMax, HPLeftArmCurr,
    // HPRightArmMax, HPRightArmCurr, HPLegsMax, HPLegsCurr]
    HealthBarData = new int[3]; 
    // Assumes HealthBarData is as follows:
    // [HealthBarNum, HPMax, HPCurr]

    public SOCharacterStats SOCS;

    [HideInInspector]
    public delegate void OnIntArrayRequested(int[] intArray);
    public static OnIntArrayRequested HealthAffected;

    void Start()
    {
        SetUp();
    }

    void SetUp()
    {
        _subSystemAmount = SOCS.HPSubSystems.Length;
        SetUpSystemHealth();
        HealthBarData[0] = HealthBarNum;
        UpdateHealthBar();
    }

    void SetUpSystemHealth()
    {
        int tempInt;
        for(int i = 0; i < _subSystemAmount; i++)
        {
            tempInt = i * 2;
            HealthSystemData[2 + tempInt] = SOCS.HPSubSystems[i];
            HealthSystemData[3 + tempInt] = SOCS.HPSubSystems[i];
        }
        tempInt = (HealthSystemData[2] + HealthSystemData[4] + HealthSystemData[6] + HealthSystemData[8] + HealthSystemData[10]);
        HealthSystemData[0] = tempInt + (tempInt /2);
        HealthSystemData[1] = HealthSystemData[0];
    }

    public void UpdateCharacterHealth(int[] newDataChanges)
    {
        // Assumes newDataChanges is [changeAmount, isHeadHit, isChestHit, isLeftArmHit, isRightArmHit, areLegsHit]
        ApplyHealthChanges(newDataChanges);
        UpdateHealthBar();
    }

    void ApplyHealthChanges(int[] newDataChanges)
    {
        for(int i = 1; i < _subSystemAmount; i++)
        {
            int tempInt = (i - 1) * 2;
            if(newDataChanges[i] == 1)
            {
                HealthSystemData[3 + tempInt] += newDataChanges[0];
                HealthSystemData[3 + tempInt] = Mathf.Clamp(HealthSystemData[3 + tempInt], 0, HealthSystemData[2 + tempInt]);
            }
        }

        HealthSystemData[1] += newDataChanges[0];
        HealthSystemData[1] = Mathf.Clamp(HealthSystemData[1], 0, HealthSystemData[0]);
    }

    void UpdateHealthBar()
    {
        
        HealthBarData[1] = HealthSystemData[0];   // Store HPMax
        HealthBarData[2] = HealthSystemData[1];   // Store HPCurr 
        HealthAffected?.Invoke(HealthBarData);
    }
}
