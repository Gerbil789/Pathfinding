using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserInterface : MonoBehaviour
{


    [SerializeField] private TMPro.TMP_InputField seedInputField;
    [SerializeField] private Animator mapGeneratorAnimator;

    private void Start()
    {
        
    }

    public void RandomSeed()
    {
        var seed = Seed.GetRandomSeed();
        seedInputField.text = seed.ToString();
    }


    public void SetMapType(int type)
    {
        if (type == 1)
        {
            mapGeneratorAnimator.SetBool("AdvancedSettings", false);
        }
        else
        {
            mapGeneratorAnimator.SetBool("AdvancedSettings", true);
        }
    }


}
