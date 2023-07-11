using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryButton : MonoBehaviour
{
    public GameProperties.ActionCategories ActionType;

    [HideInInspector] 
    public static SendInt OnCategoryButtonPressed;
    public static OnSomeEvent OnCategorySelected;

    public void OnClick()
    {
        OnCategoryButtonPressed?.Invoke((int)ActionType);
        OnCategorySelected?.Invoke();
    }
    
}
