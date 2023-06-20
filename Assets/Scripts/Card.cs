using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Suit {
	HEARTS,
	CLUBS,
	DIAMONDS,
	SPADES
}

public enum CardColor {
	BLACK,
	RED
}

public enum Rank {
	ACE = 1,
	TWO = 2,
	THREE = 3,
	FOUR = 4,
	FIVE = 5,
	SIX = 6,
	SEVEN = 7,
	EIGHT = 8,
	NINE = 9,
	TEN = 10,
	JACK = 11,
	QUEEN = 12,
	KING = 13
}

public enum CardState {
	ON_COLUMN, //list of cards; bottom can be a stack or a single
    ON_HAND_PLACE,
	ON_ACE_PLACE,
	IN_DRAG
}

public enum ColliderSize
{
    Default,
    FirstInColumn,
    Dragged
}

public class Card : MonoBehaviour {
	public Suit suit;
	public Rank rank;

    private const float yOffset = 0.2f;
    private const float zOffset = 0.32f;

    private const float mouseHeight = 6f;

    private List<GameObject> cardsBelow;
    private List<GameObject> cardsAbove;
    private List<GameObject> cardsInMovingStack;

	private Vector3 currentDragPosition;
	private Vector3 positionBeforeDrag;

	private Vector3 newHandPlacePosition;
	private Vector3 newAcePlacePosition;
    private Vector3 newColumnPosition;

    private CardState state;
    private CardState stateBeforeDrag;

    private BoxCollider cardCollider;

    private bool firstInStack = false;

    private GameObject currentColumn = null;
    private GameObject newColumn = null;

    private Undo undo;

    private Vector3 defaultSize = new Vector3(0.06350002f, 2, 0.08890001f);
    private Vector3 firstInColumnSize = new Vector3(0.06350002f, 2, 0.3628439f);
    private Vector3 draggedSized = new Vector3(0.02f, 10, 0.02f);

    private Vector3 firstInColumnCentre = new Vector3(0, 0, -0.1401029f);

    private float lastClickTime = 0f;
    private float doubleClickMargin = 1f;

    private AcePlaceController acePlace;
    private HandPlaceController handPlace;
    private GameController gameController;
    
    private bool wasKingAtTop = false;  //prevent incrementing the same King multiple times

    void Awake() {
        cardsBelow = new List<GameObject>();
		cardsAbove = new List<GameObject> ();
        cardsInMovingStack = new List<GameObject>();

		newHandPlacePosition = Vector3.zero;
		newAcePlacePosition = Vector3.zero;
        newColumnPosition = Vector3.zero;

        cardCollider = GetComponent<BoxCollider>();

        acePlace = GameObject.Find("Ace Places").GetComponent<AcePlaceController>();

        handPlace = GameObject.Find("Hand Places").GetComponent<HandPlaceController>();

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

	public CardColor GetColor() {
		switch (suit) {
		case Suit.DIAMONDS:
            return CardColor.RED;
        case Suit.HEARTS:
			return CardColor.RED;
		default: 
			return CardColor.BLACK;
		}
	}

	public void SetCardAttributes(Vector3 columnPosition, int PlaceInColumn, CardState state, Undo undo)
    {
        Vector3 initialPosition = new Vector3(columnPosition.x,
                                                columnPosition.y + (PlaceInColumn * yOffset),
                                                columnPosition.z - (PlaceInColumn * zOffset));
		transform.position = initialPosition;
		this.state = state;
        this.undo = undo;
    }

    public void ClearCardsBelow()
    {
        cardsBelow.Clear();
    }

    public void ClearCardsAbove()
    {
        cardsAbove.Clear();
    }

    bool BeingDragged() {
		return state == CardState.IN_DRAG;
    }

    void OnMouseDrag()
    {
        if (BeingDragged())
        {
            if (cardsInMovingStack.Count == 0 && cardsBelow.Count != 0)
            {
                cardsInMovingStack.AddRange(cardsBelow[0].GetComponent<Card>().ChangePositionOfCardBelow());
                SetStackToDragging();
            }

            currentDragPosition = GetCurrentMousePosition();
            if (firstInStack)
            {
                transform.position = currentDragPosition;
                MoveCardsInStack();
            }
        }
    }

    void OnMouseDown() {
		if (CanBePickedUp ()) {
            if (Time.time > 1f && Time.time - lastClickTime < doubleClickMargin) //double tap behaviour; preventing accidental double tap at start of game
            {
                lastClickTime = 0;
                KeyValuePair<bool, Vector3> acePlacePosition = acePlace.SearchDroppableAcePlacePosition(rank, suit);

                if(acePlacePosition.Key)
                {
	                stateBeforeDrag = state;
	                positionBeforeDrag = transform.position;
                    DropCard(acePlacePosition.Value, CardState.ON_ACE_PLACE);
                }
            }
            else //single tap behaviour
            {
                lastClickTime = Time.time;

                positionBeforeDrag = transform.position;
                currentDragPosition = GetCurrentMousePosition();

                stateBeforeDrag = state;
                state = CardState.IN_DRAG;
                firstInStack = true;
                SetColliderSize(ColliderSize.Dragged);
            }
        }
	}

	void OnMouseUp() {
		if (BeingDragged()) {

            bool canBeDropped = CanBeDropped();

            // Check if the card can stay where it is dropped
            if (canBeDropped)
            {
                DropCard(GetNiceDropPosition(), GetStateAfterDrop());
            }
            else
            {
                if (!TryToDropOnDroppablePlace())
                {
                    if (cardsBelow.Count != 0)
                    {
                        cardsBelow[0].GetComponent<Card>().SetColliderSize(ColliderSize.Default);
                    }

                    //the drop was denied, first change the state, then reset the card attributes               
                    state = stateBeforeDrag;
                    ResetCardSurroundingAttributes();

                    //return to its original position
                    transform.position = positionBeforeDrag;
                }

            }
            MoveCardsInStack();
            SetMovingStackAttributes(currentColumn, false);
            ClearMovingStack();
        }
	}

    private bool TryToDropOnDroppablePlace()
    {
        if (cardsAbove.Count != 0)
        {
            CardState aboveCardState = cardsAbove[0].GetComponent<Card>().state;

            if (aboveCardState == CardState.ON_ACE_PLACE)
            {
                KeyValuePair<bool, Vector3> acePlacePosition = acePlace.SearchDroppableAcePlacePosition(rank, suit);
                if (acePlacePosition.Key)
                {
                    DropCard(acePlacePosition.Value, CardState.ON_ACE_PLACE);
                    return true;
                }
            }
            else if(aboveCardState == CardState.ON_HAND_PLACE)
            {
                KeyValuePair<bool, Vector3> handPlacePosition = handPlace.SearchDroppableHandPlacePosition();
                if (handPlacePosition.Key)
                {
                    DropCard(handPlacePosition.Value, CardState.ON_HAND_PLACE);
                    return true;
                }
            }
        }
        else if (newAcePlacePosition != Vector3.zero)
        {
	        KeyValuePair<bool, Vector3> acePlacePosition = acePlace.SearchDroppableAcePlacePosition(rank, suit);
	        if (acePlacePosition.Key)
	        {
		        DropCard(acePlacePosition.Value, CardState.ON_ACE_PLACE);
		        return true;
	        }
        }
        return false;
    }

    public void DropCard(Vector3 newPosition, CardState newState, bool wasUndo = false)
    {
        undo.RecordCardAttributes(gameObject, positionBeforeDrag, currentColumn, stateBeforeDrag);

        Vector3 lastMovedCardPosition = transform.position;
        CardState previousState = state;

        // get a nice position for the card to drop
        transform.position = newPosition;

        //the drop was allowed, change the state then reset the card
        state = newState;        

        UpdateCardColumn(wasUndo);

        if(cardsAbove.Count != 0)
        {
            cardsAbove[0].GetComponent<Card>().SetColliderSize(ColliderSize.Default);
        }

        ResetCardSurroundingAttributes();

        if (wasUndo)
        {
	        if (rank == Rank.KING)
	        {
		        if (previousState == CardState.ON_ACE_PLACE)
		        {
			        gameController.DecrementNumberOfKings();
		        }

		        if (previousState == CardState.ON_COLUMN && wasKingAtTop)
		        {
			        gameController.DecrementNumberOfKings();
		        }
	        }

	        acePlace.UpdateAcePlace(lastMovedCardPosition, suit, wasUndo);
            handPlace.UpdateHandPlace(lastMovedCardPosition, wasUndo);

            if (cardsBelow.Count != 0)
            {
                SetMovingStackAttributes(currentColumn, wasUndo);
                MoveCardsInStack();
                ClearMovingStack();
            }
        }
        else if (state == CardState.ON_ACE_PLACE)
        {
            acePlace.UpdateAcePlace(transform.position, suit, wasUndo);
            if (rank == Rank.KING && !wasKingAtTop)
            {
	            gameController.IncrementNumberOfKings();
            }
        }
        else if (state == CardState.ON_HAND_PLACE)
        {
	        if (wasKingAtTop)
	        {
		        gameController.DecrementNumberOfKings();
	        }
	        
            handPlace.UpdateHandPlace(transform.position, wasUndo);
        }
        
        if(stateBeforeDrag == CardState.ON_HAND_PLACE)
        {
            handPlace.UpdateHandPlace(positionBeforeDrag, true);
        }
    }

    public void ResetCardSurroundingAttributes()
    {
        if (state == CardState.ON_COLUMN && cardsBelow.Count == 0)
        {
            SetColliderSize(ColliderSize.FirstInColumn);
        }
        else
        {
            SetColliderSize(ColliderSize.Default);
        }

        newHandPlacePosition = Vector3.zero;
        newAcePlacePosition = Vector3.zero;
        newColumnPosition = Vector3.zero;

        firstInStack = false;
    }

    public CardState GetStateAfterDrop() {
		if (cardsAbove.Count > 0) {
			return cardsAbove [0].GetComponent<Card> ().state;
		} else if (newAcePlacePosition != Vector3.zero) {
			return CardState.ON_ACE_PLACE;
		} else if (newHandPlacePosition != Vector3.zero) {
			return CardState.ON_HAND_PLACE;
		} else {
			return CardState.ON_COLUMN;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Card")) {
			
			GameObject otherCardObject = other.gameObject;

            if (IsThisCardAboveOtherCard(otherCardObject))
            {                
                AddToCardsBelow(otherCardObject);            
			} else if (IsThisCardBelowOtherCard (otherCardObject)) {
				AddToCardsAbove (otherCardObject);
			}
		} else if (string.Equals(other.tag, "HandPlace", StringComparison.Ordinal)) {
			newHandPlacePosition = other.transform.position;
		} else if (other.CompareTag("AcePlace")) {
			newAcePlacePosition = other.transform.position;
		} else if (other.CompareTag("Column"))
        {
            newColumnPosition = other.transform.position;
            SetNewColumn(other.gameObject);
        }
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Card"))
		{
			if (!cardsBelow.Contains(other.gameObject) && !cardsAbove.Contains(other.gameObject)) 
			{
				GameObject otherCardObject = other.gameObject;

				if (IsThisCardAboveOtherCard(otherCardObject))
				{                
					AddToCardsBelow(otherCardObject);            
				} else if (IsThisCardBelowOtherCard (otherCardObject)) {
					AddToCardsAbove (otherCardObject);
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Card")) {
			
			GameObject otherGameObject = other.gameObject;

			if (cardsBelow.Contains (otherGameObject) && state != CardState.IN_DRAG)
            {
                cardsBelow.Remove (otherGameObject);

                foreach (var card in otherGameObject.GetComponent<Card>().GetCardsBelow())
                {
                    if (cardsBelow.Contains(card))
                    {
                        cardsBelow.Remove(card);
                    }
                }

                if (cardsBelow.Count == 0 && state == CardState.ON_COLUMN)
                {
                    SetColliderSize(ColliderSize.FirstInColumn);
                }

                foreach (var card in cardsAbove)
                {
	                Card cardScript = card.GetComponent<Card>();
	                if (cardScript.GetCardsBelow().Contains(otherGameObject))
	                {
		                cardScript.RemoveFromCardsBelow(otherGameObject);
	                }
                }
			}
            else if (cardsAbove.Contains (otherGameObject)) {
				cardsAbove.Remove (otherGameObject);
			}
		} else if (other.CompareTag("HandPlace")) {
			if (newHandPlacePosition == other.transform.position) {
				newHandPlacePosition = Vector3.zero;
			}
		} else if (other.CompareTag("AcePlace")) {
			if (newAcePlacePosition == other.transform.position) {
				newAcePlacePosition = Vector3.zero;
			}
		} else if (other.CompareTag("Column")) {
            if (newColumnPosition == other.transform.position)
            {
                newColumnPosition = Vector3.zero;
            }
        }
    }

	private Vector3 GetCurrentMousePosition() {
		Vector3 origPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		return new Vector3 (origPosition.x, mouseHeight, origPosition.z);
	}
		
	public Vector3 GetNiceDropPosition() {
		if (cardsAbove.Count != 0) {
			Vector3 positionOfOther = cardsAbove [0].transform.position;

			if (cardsAbove [0].GetComponent<Card> ().state == CardState.ON_ACE_PLACE) {
				return new Vector3(positionOfOther.x, positionOfOther.y + yOffset, positionOfOther.z);
			} else {
				return new Vector3 (positionOfOther.x, positionOfOther.y + yOffset, positionOfOther.z - zOffset);
			}
		} else if (newHandPlacePosition != Vector3.zero) {
			return newHandPlacePosition;
		} else if (newAcePlacePosition != Vector3.zero) {
			return newAcePlacePosition;
		} else if (newColumnPosition != Vector3.zero) {
            return newColumnPosition;
        } else {
			return Vector3.zero;
        }
    }

	public bool CanBePickedUp() {
		switch(state) {
		case CardState.ON_COLUMN: return IsStack();
		case CardState.ON_HAND_PLACE: return true;
		case CardState.ON_ACE_PLACE: return false;
		default:
			return false;
		}
	}

	public bool CanBeDropped() {
		// IF hovering over a card, check if the card below would accept this card
		if (cardsAbove.Count != 0) {
			return cardsAbove [0].GetComponent<Card> ().CanThisCardAcceptOtherCard (this);
		} else if (newAcePlacePosition != Vector3.zero) {
            return rank == Rank.ACE && cardsAbove.Count == 0 && acePlace.CheckIfAcePlaceIsEmpty(newAcePlacePosition);
        } else if (newHandPlacePosition != Vector3.zero) {
			return cardsBelow.Count == 0 && handPlace.IsHandPlaceEmpty(newHandPlacePosition);
		} else if (newColumnPosition != Vector3.zero) {
 // bvdb 2023-06-20 you can drop any card
 //         return rank == Rank.KING && newColumn.GetComponent<Column>().IsColumnEmpty();
            return newColumn.GetComponent<Column>().IsColumnEmpty();
        }
        else {
            return false;
        }
	}

	public bool IsStack() {
        //this is the lowest card or the cards below form a stack
        if (cardsBelow.Count == 0) {
            //this is the lowest card in the column
            return true;
		} else {
            //at least 1 card is lying on this card
            Card cardDirectlyOnCurrentCard = cardsBelow[0].GetComponent<Card> ();
            //check if cardDirectlyOnCurrentCard forms a stack with the current card
            //if true, then also check the (possible) card lying on cardDirectlyOnCurrentCard
            return cardDirectlyOnCurrentCard.GetColor () != GetColor () && cardDirectlyOnCurrentCard.rank == rank - 1 && cardDirectlyOnCurrentCard.IsStack();
		}
	}


	public bool CanThisCardAcceptOtherCard(Card other) {

		switch(state) {
		case CardState.ON_COLUMN: {
				// a) must be the head of the stack and
				// b) must be a card of opposite color with a rank-1
				return cardsBelow.Count > 0 && cardsBelow[0].GetComponent<Card>().BeingDragged() && 
					other.GetColor() != GetColor() &&
					other.rank == rank - 1;
			}
		case CardState.ON_ACE_PLACE: {
				// a) must be the same suit
				// b) must be of rank + 1
				return other.suit == suit &&
					other.rank == rank + 1;
			}
		default: return false;
		}
	}

	void AddToCardsBelow(GameObject card) {
        if (!cardsBelow.Contains(card))
        {
            cardsBelow.Add(card);
            cardsBelow.Sort((x, y) => x.transform.position.y < y.transform.position.y ? -1 : 1);

            if (card.GetComponent<Card>().state == CardState.ON_COLUMN)
            {
                SetColliderSize(ColliderSize.Default);
            }
        }
            
	}

	void AddToCardsAbove(GameObject card) {
		cardsAbove.Add (card);
		cardsAbove.Sort ((x, y) => x.transform.position.y > y.transform.position.y ? -1 : 1);
	}

	bool IsThisCardAboveOtherCard(GameObject other) {
		return other.transform.position.y > transform.position.y;
	}

	bool IsThisCardBelowOtherCard(GameObject other) {
		return other.transform.position.y < transform.position.y;
	}
		
	Card GetCard(GameObject cardObject) {
		return cardObject.GetComponent<Card> ();
	}

    public List<GameObject> ChangePositionOfCardBelow()
    {
        List<GameObject> cardStack = new List<GameObject>();
        cardStack.Add(gameObject);

        if (cardsBelow.Count != 0)
        {
            cardStack.AddRange(cardsBelow[0].GetComponent<Card>().ChangePositionOfCardBelow());
        }


        return cardStack;
    }

    private void ClearMovingStack()
    {        
        cardsInMovingStack.Clear();
    }

    private void SetStackToDragging()
    {
        foreach(GameObject movingStackCardObject in cardsInMovingStack)
        {
            Card movingStackCardComponent = movingStackCardObject.GetComponent<Card>();
            movingStackCardComponent.SetColliderSize(ColliderSize.Dragged);
            movingStackCardComponent.state = CardState.IN_DRAG;
        }
    }

    private void SetMovingStackAttributes(GameObject newColumn, bool wasUndo)
    {
        Card movingStackCard;

        if (wasUndo)
        {
            cardsInMovingStack = new List<GameObject>(cardsBelow);
        }

        for (int i = 0; i < cardsInMovingStack.Count; i++)
        {
            movingStackCard = cardsInMovingStack[i].GetComponent<Card>();
            movingStackCard.state = CardState.ON_COLUMN;
            movingStackCard.ResetCardSurroundingAttributes();
            movingStackCard.SetNewColumn(newColumn);
            movingStackCard.UpdateCardColumn(wasUndo);
        }

        cardsBelow = new List<GameObject>(cardsInMovingStack);
    }

    public void PlaceInColumn(GameObject column, bool isFirst)
    {
        currentColumn = column;
        currentColumn.GetComponent<Column>().IncrementCardsInColumn();
        if (isFirst)
        {
	        currentColumn.GetComponent<Column>().SetFirstCard(gameObject);
	        
	        //king handler
	        if (rank == Rank.KING && !wasKingAtTop)
	        {
		        gameController.IncrementNumberOfKings();
	        }
        }
    }

    public GameObject GetCurrentColumn()
    {
        return currentColumn;
    }

    public void SetCurrentColumn(GameObject column)
    {
        currentColumn = column;
    }

    public void SetNewColumn(GameObject column)
    {
        newColumn = column;
    }

    private void UpdateCardColumn(bool firstInUndoStack)
    {
	    wasKingAtTop = false;
        if (currentColumn != null)
        {
	        Column column = currentColumn.GetComponent<Column>();
	        // scenario: card is being dropped on itself. prevent decrementing kings counter when it's a king. 
            if (!column.IsColumnEmpty())
            {
                Card firstCard = column.GetFirstCard().GetComponent<Card>();
                if (firstCard.rank == Rank.KING && firstCard.gameObject == gameObject)
                {
                    wasKingAtTop = true;
                }
                //card is being dropped on a column, so the number of cards in the card's current column must become 1 less
                column.DecrementCardsInColumn();
                if (column.IsColumnEmpty()) //if the column is now empty and has had a first card, we now remove it
                {
                    column.SetFirstCard(null);
                }
            }    
        }

        if (state == CardState.ON_COLUMN)
        {
            if (!firstInUndoStack && cardsAbove.Count != 0)
            {
                newColumn = cardsAbove[0].GetComponent<Card>().GetCurrentColumn();
            }

            bool isFirst = newColumn.GetComponent<Column>().IsColumnEmpty();//if the column was previously empty, we set a new card on top
            PlaceInColumn(newColumn, isFirst);
        }
        else
        {
            currentColumn = null;
        }
    }

    public void SetColliderSize(ColliderSize size)
    {
        switch(size)
        {
            case ColliderSize.Default:
                cardCollider.size = defaultSize;
                cardCollider.center = Vector3.zero;
                break;

            case ColliderSize.FirstInColumn:
                cardCollider.size = firstInColumnSize;
                cardCollider.center = firstInColumnCentre;
                break;

            case ColliderSize.Dragged:
                cardCollider.size = draggedSized;
                cardCollider.center = Vector3.zero;
                break;
        }
    }

    private void MoveCardsInStack()
    {
        for (int i = 0; i < cardsInMovingStack.Count; i++)
        {
            cardsInMovingStack[i].transform.position = new Vector3(
               transform.position.x,
               transform.position.y + (yOffset * (i + 1)),
               transform.position.z - (zOffset * (i + 1)));
        }
    }

    public List<GameObject> GetCardsBelow()
    {
        return cardsBelow;
    }

    public void RemoveFromCardsBelow(GameObject card)
    {
	    foreach (var aboveCard in cardsAbove)
	    {
		    Card cardScript = aboveCard.GetComponent<Card>();
		    if (cardScript.GetCardsBelow().Contains(card))
		    {
			    cardScript.RemoveFromCardsBelow(card);
		    }
	    }
	    cardsBelow.Remove(card);
    }
}
