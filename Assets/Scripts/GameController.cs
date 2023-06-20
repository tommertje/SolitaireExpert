using UnityEngine;
using System.Collections;
using System.Linq;

public class GameController : MonoBehaviour {
	public GameObject[] cards;
	public GameObject[] handPlaces;
	public GameObject[] columns;

    public AcePlaceController acePlaceController;
    public HandPlaceController handPlaceController;

    [SerializeField] Undo undo;
    
    [SerializeField] private NewGameScreen newGame;

    private int numberOfFinalisedKings = 0;

	void Start () {
		NewGame ();
    }

    public void NewGame()
    {
        
		RandomizeDeck();
		//SetupCardsDebugMode();
		SetupCards();
        acePlaceController.SetupAcePlaces();
        handPlaceController.SetupHandPlaces();
        DifficultyCheck();
    }


	void SetupCardsDebugMode()
	{
		GameObject[] cardsSorted = new GameObject[104];
		int a, b;
		a = 0;
		b = 8;

		for (int i = 0; i < 104; i++)
		{
			Card card = cards[i].GetComponent<Card>();
			GameObject obj = cards[i];
			if ((int)card.rank == 1)
			{
				cardsSorted[a] = obj;
				a++;
			}
			else
			{
				cardsSorted[b] = obj;
				b++;
			}
		}
		cards = cardsSorted;
	}
	void RandomizeDeck() {
		cards = cards.OrderBy(x => Random.value).ToArray();
	}

	void SetupCards()
	{
		numberOfFinalisedKings = 0;
		// Distribute the cards in a 10x10 manner
		for (int i = 0; i < 10; i++) {
			Vector3 columnPosition = columns [i].transform.position;
            Card current = null;
            Column column = columns[i].GetComponent<Column>();
            column.ResetColumn();

            for (int PlaceInColumn = 0; PlaceInColumn < 10; PlaceInColumn++) {
				int arrayIndex = (i * 10) + PlaceInColumn;
				current = cards [arrayIndex].GetComponent<Card> ();

				current.SetCardAttributes (columnPosition,
                    PlaceInColumn,
					CardState.ON_COLUMN,
                    undo);
                current.ResetCardSurroundingAttributes();
                current.ClearCardsBelow();
                current.ClearCardsAbove();
                bool isFirst = PlaceInColumn == 0; //if it's the first card in column we set it
	            current.PlaceInColumn(column.gameObject, isFirst);
            }
            current.SetColliderSize(ColliderSize.FirstInColumn);//the last card in the for will be the first card in the column, we can change its collider size
		}

		// place the 4 remaining cards on the free spots
		for (int i = 0; i < 4; i++) {
			Card current = cards [i + 100].GetComponent<Card> ();

			current.SetCardAttributes (handPlaces[i].transform.position, 0 /*PlaceInColumn*/, CardState.ON_HAND_PLACE, undo);
            current.ResetCardSurroundingAttributes();
            current.ClearCardsBelow();
            current.ClearCardsAbove();
			current.SetCurrentColumn(null);
		}
	}

	public void HighlightCards(Rank rankToBeHighlighted) {
		for (int i = 0; i < 104; i++) {
			if (cards [i].GetComponent<Card> ().rank == rankToBeHighlighted) {
				StartCoroutine(PopUpCard(i)); //must use co-routine to be able to use Wait in PopUp
			}
		}
	}

// we could expand the menu with color-choice
//	public void HighlightCards(Rank rankToBeHighlighted, CardColor colorToBeHighlighted) {
//		for (int i = 0; i < 104; i++) {
//			if (cards [i].GetComponent<Card> ().GetColor() == colorToBeHighlighted && cards [i].GetComponent<Card> ().rank == rankToBeHighlighted) {
//				StartCoroutine(PopUpCard(i)); //must use co-routine to be able to use Wait in PopUp
//			}
//		}
//	}

	IEnumerator PopUpCard(int i) {
		cards [i].transform.position= new Vector3 (cards [i].transform.position.x,cards [i].transform.position.y+0.21f,cards [i].transform.position.z);
		yield return new WaitForSeconds (0.6f);
		cards [i].transform.position= new Vector3 (cards [i].transform.position.x,cards [i].transform.position.y-0.21f,cards [i].transform.position.z);
	}

	void DifficultyCheck()
	{
		int[] suit = new int[4];
		int[] rank = new int[13];
		
		//all the cards that are at the bottom of a column
		for (int i = 9; i < 100; i += 10)
		{
			Card card = cards[i].GetComponent<Card>();
			suit[(int)card.suit]++;
			rank[(int) card.rank - 1]++;
		}
		
		//the last 4 cards that are in the hand
		for (int j = 100; j < 104; j++)
		{
			Card card = cards[j].GetComponent<Card>();
			suit[(int)card.suit]++;
			rank[(int)card.rank - 1]++;
		}

		if (suit.Max() > 9)
		{
			NewGame();
			return;
		}

		if (rank.Max() > 3)
		{
			NewGame();
			return;
		}

		if (rank[10] + rank[11] + rank[12] > 2)//Jack, Queen and King check
		{
			NewGame();
			return;
		}

		if (numberOfFinalisedKings == 8)
		{
			NewGame();
			return;
		}
	}

	public void IncrementNumberOfKings()
	{
		if (numberOfFinalisedKings < 8)
		{
			numberOfFinalisedKings++;
		}
		
		if (numberOfFinalisedKings == 8)
		{
			newGame.OpenNewGameWindow(true);
		}
	}

	public void DecrementNumberOfKings()
	{
		if (numberOfFinalisedKings > 0)
		{
			numberOfFinalisedKings--;
		}
	}

}
	