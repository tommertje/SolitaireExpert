using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RulesScreen : MonoBehaviour
{
    [SerializeField] GameObject rulesScreen;
    [SerializeField] GameObject popUpScreen;
    [SerializeField] GameObject menuCamera;

    void OnMouseDown()
    {
        if(popUpScreen.activeSelf == false)
        {
            rulesScreen.SetActive(true);
            popUpScreen.SetActive(true);
            menuCamera.SetActive(true);
        }
    }
}
