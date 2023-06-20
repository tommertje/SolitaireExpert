using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckChanger : MonoBehaviour
{
    [SerializeField] List<GameObject> deck;
    [SerializeField] int deckDesignNumber;

    [SerializeField] GameObject popUpScreen;
    [SerializeField] GameObject menuCamera;

    void OnMouseDown()
    {
        foreach (GameObject card in deck)
        {
            CardDesign cardDesign = card.GetComponent<CardDesign>();
            if (cardDesign != null) //to be removed
                cardDesign.ChangeDesign(deckDesignNumber - 1);
        }

        for (int i = 1; i < popUpScreen.transform.childCount - 2; i++)
        {
            var child = popUpScreen.transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(false);
        }

        popUpScreen.SetActive(false);
        menuCamera.SetActive(false);
    }
}
