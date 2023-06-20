using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HelpScreen : MonoBehaviour
{
    [SerializeField] GameObject helpScreen;
    [SerializeField] GameObject popUpScreen;
    [SerializeField] GameObject menuCamera;

    void OnMouseDown()
    {
        if(popUpScreen.activeSelf == false)
        {
            helpScreen.SetActive(true);
            popUpScreen.SetActive(true);
            menuCamera.SetActive(true);
        }
    }
}
