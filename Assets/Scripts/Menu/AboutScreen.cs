using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutScreen : MonoBehaviour
{
    [SerializeField] GameObject aboutScreen;
    [SerializeField] GameObject popUpScreen;
    [SerializeField] GameObject menuCamera;

    void OnMouseDown()
    {
        if (popUpScreen.activeSelf == false)
        {
            aboutScreen.SetActive(true);
            popUpScreen.SetActive(true);
            menuCamera.SetActive(true);
        }
    }
}
