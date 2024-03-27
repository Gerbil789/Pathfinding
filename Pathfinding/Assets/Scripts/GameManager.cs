using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            PauseGame();
        }
    }

    void PauseGame(){
        if(Time.timeScale == 0){
            Time.timeScale = 1;
        }else{
            Time.timeScale = 0;
        }
    }
}