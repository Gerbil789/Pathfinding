using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public InputManager inputManager;


    public TMP_Dropdown strategyDropdown; 

    void Start()
    {
        InitializeDropdown();
    }

    private void InitializeDropdown()
    {
        if (strategyDropdown != null)
        {
            strategyDropdown.options.Clear();
            foreach (var value in System.Enum.GetValues(typeof(Algorithm)))
            {
                strategyDropdown.options.Add(new TMP_Dropdown.OptionData(value.ToString()));
            }
            strategyDropdown.value = (int)inputManager.strategy;
            strategyDropdown.RefreshShownValue();
        }
    }

    public void OnDropdownChange(Int32 index)
    {
        inputManager.strategy = (Algorithm)index;
        Debug.Log("New Strategy Selected: " + inputManager.strategy);
    }
}