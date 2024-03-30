using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField seedInputField;

    public void RandomSeed()
    {
        var seed = Seed.GetRandomSeed();
        seedInputField.text = seed.ToString();
    }

}
