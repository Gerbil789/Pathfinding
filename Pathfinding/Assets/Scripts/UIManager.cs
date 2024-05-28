using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

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

            List<Algorithm> algorithms = new List<Algorithm>() { Algorithm.ASTAR, Algorithm.DOTS_ASTAR, Algorithm.DFS};

            foreach (var value in algorithms)
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