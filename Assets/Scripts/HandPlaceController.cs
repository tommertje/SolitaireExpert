using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HandPlaceObject
{
    public GameObject handObject;
    public bool isOccupied;

    public HandPlaceObject(GameObject handObject, bool isOccupied)
    {
        this.handObject = handObject;
        this.isOccupied = isOccupied;
    }
}

public class HandPlaceController : MonoBehaviour
{
    private List<HandPlaceObject> handPlaces = new List<HandPlaceObject>();

    public void SetupHandPlaces()
    {
        handPlaces = new List<HandPlaceObject>();
        foreach (var handPlace in GameObject.FindGameObjectsWithTag("HandPlace"))
        {
            handPlaces.Add(new HandPlaceObject(handPlace, true));
        }
    }

    public KeyValuePair<bool, Vector3> SearchDroppableHandPlacePosition()
    {
        foreach (var handPlace in handPlaces)
        {
            if(!handPlace.isOccupied)
            {
                return new KeyValuePair<bool, Vector3>(true, handPlace.handObject.transform.position);
            }
        }

        return new KeyValuePair<bool, Vector3>(false, Vector3.zero);
    }

    public bool IsHandPlaceEmpty(Vector3 cardPosition)
    {
        foreach (var handPlace in handPlaces)
        {
            if (Mathf.Approximately(handPlace.handObject.transform.position.z, cardPosition.z))
            {
                return !handPlace.isOccupied;
            }
        }

        return false;
    }

    public void UpdateHandPlace(Vector3 cardPosition, bool wasRemoved)
    {
        for (int i = 0; i < handPlaces.Count; i++)
        {
            if (Mathf.Approximately(handPlaces[i].handObject.transform.position.z, cardPosition.z))
            {
                if (wasRemoved)
                {
                    RemoveCardInHandPlace(i);
                }
                else
                {
                    AddCardInHandPlace(i);
                }
                break;
            }
        }
    }

    private void AddCardInHandPlace(int handPlaceIndex)
    {
        handPlaces[handPlaceIndex] = new HandPlaceObject(handPlaces[handPlaceIndex].handObject, true);
    }

    private void RemoveCardInHandPlace(int handPlaceIndex)
    {
        handPlaces[handPlaceIndex] = new HandPlaceObject(handPlaces[handPlaceIndex].handObject, false);
    }
}
