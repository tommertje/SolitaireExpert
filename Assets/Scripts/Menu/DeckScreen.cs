using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScreen : MonoBehaviour
{
    [SerializeField] GameObject deckScreen;
    [SerializeField] GameObject popUpScreen;
    [SerializeField] GameObject menuCamera;

    void OnMouseDown()
    {
        if (popUpScreen.activeSelf == false)
        {
            deckScreen.SetActive(true);
            popUpScreen.SetActive(true);
            menuCamera.SetActive(true);
        }
    }
}
