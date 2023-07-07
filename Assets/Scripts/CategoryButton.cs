using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryButton : MonoBehaviour
{
    public enum ActionCategories{
                            Attack, Defend, Move, Special 
                        };
    public ActionCategories ActionType;

    [HideInInspector] 
    public delegate void SendInt(int data);
    public static SendInt OnCategoryButtonPressed;
    public delegate void OnSomeEvent();
    public static OnSomeEvent OnCategorySelected;

    public void OnClick()
    {
        OnCategoryButtonPressed?.Invoke((int)ActionType);
        OnCategorySelected?.Invoke();
    }
    
}
