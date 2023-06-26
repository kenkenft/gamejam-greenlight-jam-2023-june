using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    public int HealthBarNum;
    private int SubSystemAmount;
    private int[] _healthSystemData = new int[12], 
    // Assumes HealthSystemData is as follows: 
    // [HPMax, HPCurr, HPHeadMax, HPHeadCurr, 
    // HPChestMax, HPChestCurr, HPLeftArmMax, HPLeftArmCurr,
    // HPRightArmMax, HPRightArmCurr, HPLegsMax, HPLegsCurr]
    _healthBarData = new int[3]; 
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
        SubSystemAmount = SOCS.HPSubSystems.Length;
        SetUpSystemHealth();
        _healthBarData[0] = HealthBarNum;
        UpdateHealthBar();
    }

    void SetUpSystemHealth()
    {
        int tempInt;
        for(int i = 0; i < SubSystemAmount; i++)
        {
            tempInt = i * 2;
            _healthSystemData[2 + tempInt] = SOCS.HPSubSystems[i];
            _healthSystemData[3 + tempInt] = SOCS.HPSubSystems[i];
        }
        _healthSystemData[0] = (50/100) * (_healthSystemData[2] + _healthSystemData[4] + _healthSystemData[6] + _healthSystemData[8] + _healthSystemData[10]);
        _healthSystemData[1] = _healthSystemData[0];
    }

    public void UpdateCharacterHealth(int[] newDataChanges)
    {
        // Assumes newDataChanges is [changeAmount, isHeadHit, isChestHit, isLeftArmHit, isRightArmHit, areLegsHit]
        ApplyHealthChanges(newDataChanges);
        UpdateHealthBar();
    }

    void ApplyHealthChanges(int[] newDataChanges)
    {
        for(int i = 1; i < SubSystemAmount; i++)
        {
            int tempInt = (i - 1) * 2;
            if(newDataChanges[i] == 1)
            {
                _healthSystemData[3 + tempInt] += newDataChanges[0];
                _healthSystemData[3 + tempInt] = Mathf.Clamp(_healthSystemData[3 + tempInt], 0, _healthSystemData[2 + tempInt]);
            }
        }

        _healthSystemData[1] += newDataChanges[0];
        _healthSystemData[1] = Mathf.Clamp(_healthSystemData[1], 0, _healthSystemData[0]);
    }

    void UpdateHealthBar()
    {
        
        _healthBarData[1] = _healthSystemData[0];   // Store HPMax
        _healthBarData[2] = _healthSystemData[1];   // Store HPCurr 
        HealthAffected?.Invoke(_healthBarData);
    }
}
