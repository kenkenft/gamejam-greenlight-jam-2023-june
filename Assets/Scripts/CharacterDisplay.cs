using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    public int HealthBarNum;
    private int _subSystemAmount, _energyStored = 30, _buffEnergyCost = 0;
    public int[] HealthSystemData = new int[12], 
    // Assumes HealthSystemData is as follows: 
    // [HPMax, HPCurr, HPHeadMax, HPHeadCurr, 
    // HPChestMax, HPChestCurr, HPLeftArmMax, HPLeftArmCurr,
    // HPRightArmMax, HPRightArmCurr, HPLegsMax, HPLegsCurr]
    HealthBarData = new int[3]; 
    // Assumes HealthBarData is as follows:
    // [HealthBarNum, HPMax, HPCurr]
    private string[] buffTypes = {"None", "Repair", "Defense", "Move", "Attack"},
                     effectTypes = {"HyperArmour", "Blocking", "Flinching", "BlockBroken", "AttackFail"};


    // int array that stores bonus modifiers. 100 = 100% = 1.0 = no change.
    public Dictionary<string, int> TempEffects = new Dictionary<string, int>(){
                                                        {"HyperArmour", 0},
                                                        {"Blocking", 0},
                                                        {"Flinching", 0},
                                                        {"BlockBroken", 0},
                                                        {"AttackFail", 0}
                                                        };

    public Dictionary<string, int> HeadBuffs = new Dictionary<string, int>(){
                                                        {"None", 1},
                                                        {"Repair", 0},
                                                        {"Defense", 0},
                                                        {"Move", 0},
                                                        {"Attack", 0}
                                                        }; 
    public SOCharacterStats SOCS;

    [HideInInspector]
    public delegate void OnIntArrayRequested(int[] intArray);
    public static OnIntArrayRequested HealthAffected;

    // void OnEnable()
    // {
    //     if(HealthBarNum == 0)
    //         MatchFlowManager.PlayerStatusRequested += GetCharacterData;
    //     if(HealthBarNum == 1)
    //         MatchFlowManager.CPUStatusRequested += GetCharacterData;
    // }
    // void OnDisable()
    // {
    //     if(HealthBarNum == 0)
    //         MatchFlowManager.PlayerStatusRequested -= GetCharacterData;
    //     if(HealthBarNum == 1)
    //         MatchFlowManager.CPUStatusRequested -= GetCharacterData;
    // }

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

    public void SetActiveHeadBuff(int targetBuffID)
    {
        string targetBuff = buffTypes[targetBuffID];

        List<string> keyList = new List<string>(HeadBuffs.Keys);
        //Enables target buff, and disables all others.
        //  foreach(string buffKey in HeadBuffs.Keys)
        foreach(string buffKey in keyList)
         {
            if(buffKey == targetBuff)
                HeadBuffs[buffKey] = 1;
            else
                HeadBuffs[buffKey] = 0;
         }

    }

    public string GetActiveHeadBuff()
    {
        foreach(var buff in HeadBuffs)
         {
            if(buff.Value == 1)
                return buff.Key;
         }
         return "None"; // In the event an improper string is given, just send back "None"
    }

    public int CheckWhichHeadBuff(string targetBuff)
    {
        if(GetActiveHeadBuff() == targetBuff)
            return 1;   // Enquired buff is active
        else
            return 0;   // Enquired buff is not active
    }

    public void SetTempEffectActive(int targetEffect, int state)
    {
        TempEffects[effectTypes[targetEffect]] = state;
    }

    public int GetTempEffectActive(int targetEffect)
    {
        foreach(var tempEffect in TempEffects)
        {
            if(tempEffect.Key == effectTypes[targetEffect])
                return tempEffect.Value;
        }

        return 0;   // In the event an improper string is given, just send back 0
    }

    // public int GetCharacterData(string targetCharacterProperty, string DictKey = "Default")
    // {
    //     int result = 0;

    //     switch(targetCharacterProperty)
    //     {
    //         case "HeadBuffs":
    //         {
    //             result = CheckWhichHeadBuff(DictKey);
    //             break;
    //         }
    //         case "TempEffects":
    //         {
    //             result = GetTempEffectActive(DictKey);
    //             break;
    //         }
    //         case "HealthBarNum":
    //         {
    //             result = HealthBarNum;
    //             break;
    //         }
    //         default:
    //             break;
    //     }

    //     return result;
    // }
}
