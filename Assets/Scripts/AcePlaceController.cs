using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AcePlaceObject
{
    public int count;
    public Suit suit;

    public AcePlaceObject(int count, Suit suit)
    {
        this.count = count;
        this.suit = suit;
    }
}

public class AcePlaceController : MonoBehaviour
{
    private Dictionary<GameObject, AcePlaceObject> acePlaces = new Dictionary<GameObject, AcePlaceObject>();

    private Vector3 yOffset = new Vector3(0, 0.1f, 0);

    [SerializeField] private NewGameScreen newGame;

    public void SetupAcePlaces()
    {
        acePlaces = new Dictionary<GameObject, AcePlaceObject>();
        GameObject[] allAcePlaces = GameObject.FindGameObjectsWithTag("AcePlace");
        System.Array.Sort(allAcePlaces, CompareObNames);

        foreach (var acePlace in allAcePlaces)
        {
            acePlaces.Add(acePlace, new AcePlaceObject(0, Suit.CLUBS)); //when rank = 0 we don't check for the suit
        }
    }

    public KeyValuePair<bool, Vector3> SearchDroppableAcePlacePosition(Rank rank, Suit suit)
    {
        GameObject occupiedPlace = null;

        foreach (var place in acePlaces)
        {
            if ((int)rank - 1 == 0 && place.Value.count == 0)//if the ace place is empty we don't need to check for the suit
            {
                occupiedPlace = place.Key;
                break;
            }
            else if ((int)rank - 1 == place.Value.count && suit == place.Value.suit)
            {
                occupiedPlace = place.Key;                
                break;
            }
        }

        if (occupiedPlace == null)
        {
            return new KeyValuePair<bool, Vector3>(false, Vector3.zero);
        }
        else
        {
            return new KeyValuePair<bool, Vector3>(true, occupiedPlace.transform.position + (yOffset * (acePlaces[occupiedPlace].count)));
        }
    }

    public void UpdateAcePlace(Vector3 cardPosition, Suit cardSuit, bool wasRemoved)
    {
        GameObject occupiedPlace = null;

        foreach (var place in acePlaces)
        {
            occupiedPlace = place.Key;
            if (Mathf.Approximately(occupiedPlace.transform.position.x, cardPosition.x))
            {
                if(wasRemoved)
                {
                    RemoveFromAcePlace(occupiedPlace, cardSuit);
                }
                else
                {
                    AddToAcePlace(occupiedPlace, cardSuit);
                }
                break;
            }
        }
    }

    public bool CheckIfAcePlaceIsEmpty(Vector3 cardPosition)
    {
        GameObject occupiedPlace = null;

        foreach (var place in acePlaces)
        {
            occupiedPlace = place.Key;
            if (Mathf.Approximately(occupiedPlace.transform.position.x, cardPosition.x))
            {
                //we have found the ace place that is below the dragged card
                if (place.Value.count == 0)
                {
                    return true;
                }
                else
                {
                    break; //we can exit the for loop if the place is occupied
                }
            }
        }

        return false;
    }

    private void AddToAcePlace(GameObject occupiedPlace, Suit cardSuit)
    {
        acePlaces[occupiedPlace] = new AcePlaceObject(acePlaces[occupiedPlace].count + 1, cardSuit);
    }

    private void RemoveFromAcePlace(GameObject occupiedPlace, Suit cardSuit)
    {
        acePlaces[occupiedPlace] = new AcePlaceObject(acePlaces[occupiedPlace].count - 1, cardSuit);
    }

    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }
}
