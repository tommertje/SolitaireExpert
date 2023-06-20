using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameScreen : MonoBehaviour
{
    [SerializeField] GameObject newgameScreen;
    [SerializeField] GameObject popUpScreen;
    [SerializeField] GameObject menuCamera;

    [SerializeField] private GameObject youWonMessage;

    void OnMouseDown()
    {
        OpenNewGameWindow(false);
    }

    public void OpenNewGameWindow(bool victory)
    {
        if (popUpScreen.activeSelf == false)
        {
            newgameScreen.SetActive(true);
            popUpScreen.SetActive(true);
            menuCamera.SetActive(true);
            
            youWonMessage.SetActive(victory);
        }
    }
}
