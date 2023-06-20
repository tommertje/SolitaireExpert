using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undo : MonoBehaviour
{

    GameObject lastMovedCard;
    Vector3 lastMovedCardPosition;
    GameObject lastMovedCardColumn;
    CardState lastMovedCardState;
     
    public void RecordCardAttributes(GameObject card, Vector3 position, GameObject column, CardState state)
    {
        lastMovedCard = card;
        lastMovedCardPosition = position;
        lastMovedCardColumn = column;
        lastMovedCardState = state;
    }

    private void OnMouseDown()
    {
        if (lastMovedCard != null && (lastMovedCardColumn != null || lastMovedCardState == CardState.ON_HAND_PLACE))
        {
            Card currentCard = lastMovedCard.GetComponent<Card>();

            currentCard.SetNewColumn(lastMovedCardColumn);

            currentCard.DropCard(lastMovedCardPosition, lastMovedCardState, true);
        }
    }
}
