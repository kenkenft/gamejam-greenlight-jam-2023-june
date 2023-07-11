using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    public int HealthBarNum, SubSystemAmount, StartEnergy, BuffEnergyCost = 0, EnergyPerTurn = 20, CurrentTileID = 0;
    public int[] HealthSystemData = new int[12], 
    // Assumes HealthSystemData is as follows: 
    // [HPMax, HPCurr, HPHeadMax, HPHeadCurr, 
    // HPChestMax, HPChestCurr, HPLeftArmMax, HPLeftArmCurr,
    // HPRightArmMax, HPRightArmCurr, HPLegsMax, HPLegsCurr]
    HealthBarData = new int[3], 
    // Assumes HealthBarData is as follows:
    // [HealthBarNum, HPMax, HPCurr]
    EnergyData = new int[3],
    // Assumes EnergyData is as follows:
    // [EnergyStored, BuffEnergyCost, EnergyPerTurn]
    EnergyBarData = new int[3];
    // Assumes EnergyBarData is as follows:
    // [HealthBarNum, EnergyMax, EnergyCurr]
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

    [SerializeField] public GameProperties.BuffTypes ActiveBuff;

    public SOCharacterStats SOCS;

    [HideInInspector]
    public static SendIntArray HealthAffected, EnergyAffected;

    void OnEnable()
    {
        // GameManager.CharacterSetUpRequested += SetUp;
        GameManager.RoundHasStarted += SetUp;
    }
    void OnDisable()
    {
        // GameManager.CharacterSetUpRequested -= SetUp;
        GameManager.RoundHasStarted -= SetUp;

    }

    void SetUp()
    {
        // Debug.Log("CharacterDisplay.SetUp called");
        SubSystemAmount = SOCS.HPSubSystems.Length;
        SetUpSystemHealth();
        HealthBarData[0] = HealthBarNum;
        UpdateHealthBar();
        EnergyData[0] = StartEnergy;
        EnergyData[1] = BuffEnergyCost;
        EnergyData[2] = EnergyPerTurn;
        EnergyBarData[0] = HealthBarNum;
        EnergyBarData[1] = 100; 

        SetActiveHeadBuff((int)GameProperties.BuffTypes.None);
    }

    void SetUpSystemHealth()
    {
        int tempInt;
        for(int i = 0; i < SubSystemAmount; i++)
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
        // Assumes newDataChanges is a size 6 int array.
        for(int i = SubSystemAmount; i > 0 ; i--)
        {
            int tempInt = i * 2;
            if(newDataChanges[i] != 0)
            {
                HealthSystemData[1 + tempInt] += newDataChanges[i];
                HealthSystemData[1 + tempInt] = Mathf.Clamp(HealthSystemData[1 + tempInt], 0, HealthSystemData[tempInt]);
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
        ActiveBuff = (GameProperties.BuffTypes)targetBuffID;
    }

    public int GetActiveHeadBuff()
    {
        return (int)ActiveBuff;
    }

    public int CheckWhichHeadBuff(int targetBuff)
    {
        return (int)ActiveBuff;
    }

    public void SetTempEffectActive(int targetEffect, int state)
    {
        TempEffects[effectTypes[targetEffect]] = state;
    }

    public int GetTempEffectActive(int targetEffect)
    {
        // Debug.Log("targetEffect: " + effectTypes[targetEffect]);
        foreach(var tempEffect in TempEffects)
        {
            if(tempEffect.Key == effectTypes[targetEffect])
                return tempEffect.Value;
        }
        // Debug.Log("Nothing found return 0");

        return 0;   // In the event an improper string is given, just send back 0
    }

    public void ConsumeEnergy(int energyUsed)
    {
        EnergyData[0] -= energyUsed;
        EnergyBarData[2] = EnergyData[0];
        EnergyAffected?.Invoke(EnergyBarData);
    } 

    public void GenerateEnergy()
    {
        int EnergyGenerated = EnergyData[2] - ( (EnergyData[1] * EnergyData[2]) / 100); // Generated energy after applying buff penalty cost
        EnergyData[0] += EnergyGenerated;
        EnergyData[0] = Mathf.Clamp(EnergyData[0], 0, 100);
        
        EnergyBarData[2] = EnergyData[0];
        EnergyAffected?.Invoke(EnergyBarData);

        // Debug.Log(gameObject.name + " Energy generated: " + EnergyGenerated + ". Total energy: " + EnergyData[0] + "%"); 
    }

}
