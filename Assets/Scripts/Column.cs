using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    private int noOfCards = 0;
    private GameObject firstCard;
    
    public void IncrementCardsInColumn()
    {
        noOfCards++;
    }

    public void DecrementCardsInColumn()
    {
        if (noOfCards != 0)
        {
            noOfCards--;
        }
    }

    public bool IsColumnEmpty()
    {
        return noOfCards == 0;
    }

    public void ResetColumn()
    {
        noOfCards = 0;
    }

    public GameObject GetFirstCard()
    {
        return firstCard;
    }

    public void SetFirstCard(GameObject card)
    {
        firstCard = card;
    }
}
