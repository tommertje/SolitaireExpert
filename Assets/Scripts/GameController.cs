using UnityEngine;
using System.Collections;
using System.Linq;
using System.Reflection;

public class GameController : MonoBehaviour {
	public GameObject[] cards;
	public GameObject[] handPlaces;
	public GameObject[] columns;

    public AcePlaceController acePlaceController;
    public HandPlaceController handPlaceController;

    [SerializeField] Undo undo;
    
    [SerializeField] private NewGameScreen newGame;
    [SerializeField] private VictoryEffectsController victoryEffects;
    [SerializeField] private bool allowFullscreenToggle = true;
    [SerializeField] private bool showOverlayWindowControlButtons = false;
    [SerializeField] private float windowControlButtonWidth = 150f;
    [SerializeField] private float windowControlButtonHeight = 42f;
    [SerializeField] private float windowControlButtonMargin = 14f;
    [SerializeField] private bool addWindowControlButtonsToMenu = true;
    [SerializeField] private bool addVictoryTestButtonToMenu = true;
    [SerializeField] private float menuWindowButtonVerticalSpacing = 1.1f;

    private bool utilityButtonsAddedToMenu;
    private bool hasPlayedVictoryEffectsThisGame;

    private int numberOfFinalisedKings = 0;

	void Start () {
		NewGame ();
    }

    void Update()
    {
        TryAddWindowControlButtonsToMenu();

        if (!allowFullscreenToggle)
        {
            return;
        }

        bool f11Pressed = Input.GetKeyDown(KeyCode.F11);
        bool altEnterPressed = (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) &&
                               (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));

        if (f11Pressed || altEnterPressed)
        {
            ToggleWindowMode();
        }
    }

    private void OnGUI()
    {
        if (!allowFullscreenToggle || !showOverlayWindowControlButtons)
        {
            return;
        }

#if !(UNITY_STANDALONE || UNITY_EDITOR)
        return;
#endif

        float buttonWidth = Mathf.Max(80f, windowControlButtonWidth);
        float buttonHeight = Mathf.Max(28f, windowControlButtonHeight);
        float margin = Mathf.Max(0f, windowControlButtonMargin);
        float top = margin;
        float right = Screen.width - margin - buttonWidth;

        Rect maximizeRect = new Rect(right, top, buttonWidth, buttonHeight);
        Rect restoreRect = new Rect(right, top + buttonHeight + 6f, buttonWidth, buttonHeight);

        if (GUI.Button(maximizeRect, "Maximize"))
        {
            SetWindowMaximized();
        }

        if (GUI.Button(restoreRect, "Restore Window"))
        {
            SetWindowedMode();
        }
    }

    private void TryAddWindowControlButtonsToMenu()
    {
        if (utilityButtonsAddedToMenu)
        {
            return;
        }

        if (!addWindowControlButtonsToMenu && !addVictoryTestButtonToMenu)
        {
            utilityButtonsAddedToMenu = true;
            return;
        }

#if !(UNITY_STANDALONE || UNITY_EDITOR)
        return;
#endif

        MenuButton menuButton = FindObjectOfType<MenuButton>();
        if (menuButton == null || menuButton.transform.childCount == 0)
        {
            return;
        }

        Transform slider = menuButton.transform.GetChild(0);
        Transform template = FindMenuButtonTemplate(slider);
        if (template == null)
        {
            return;
        }

        int rowIndex = 1;
        if (allowFullscreenToggle && addWindowControlButtonsToMenu)
        {
            CreateMenuWindowControlButton(slider, template, "Maximize", WindowModeButtonMode.Maximize, rowIndex++);
            CreateMenuWindowControlButton(slider, template, "Restore", WindowModeButtonMode.RestoreWindow, rowIndex++);
        }

        if (addVictoryTestButtonToMenu)
        {
            CreateMenuWindowControlButton(slider, template, "Test Victory", WindowModeButtonMode.TestVictory, rowIndex);
        }

        utilityButtonsAddedToMenu = true;
    }

    private Transform FindMenuButtonTemplate(Transform slider)
    {
        for (int i = 0; i < slider.childCount; i++)
        {
            Transform child = slider.GetChild(i);
            if (child.GetComponent<Collider>() != null || child.GetComponent<Collider2D>() != null)
            {
                return child;
            }
        }

        if (slider.childCount == 0)
        {
            return null;
        }

        return slider.GetChild(0);
    }

    private void CreateMenuWindowControlButton(Transform slider, Transform template, string buttonLabel, WindowModeButtonMode mode, int rowIndex)
    {
        GameObject buttonObject = Instantiate(template.gameObject, slider);
        buttonObject.name = buttonLabel + "WindowButton";
        buttonObject.transform.localPosition = template.localPosition + new Vector3(0f, -menuWindowButtonVerticalSpacing * rowIndex, 0f);
        buttonObject.transform.localRotation = template.localRotation;
        buttonObject.transform.localScale = template.localScale;

        MonoBehaviour[] behaviours = buttonObject.GetComponents<MonoBehaviour>();
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] != null)
            {
                Destroy(behaviours[i]);
            }
        }

        WindowModeButton windowModeButton = buttonObject.AddComponent<WindowModeButton>();
        windowModeButton.Initialize(this, mode);
        TrySetButtonLabel(buttonObject, buttonLabel);
    }

    private void TrySetButtonLabel(GameObject buttonObject, string buttonLabel)
    {
        Component[] components = buttonObject.GetComponentsInChildren<Component>(true);
        for (int i = 0; i < components.Length; i++)
        {
            Component component = components[i];
            if (component == null)
            {
                continue;
            }

            PropertyInfo textProperty = component.GetType().GetProperty("text");
            if (textProperty == null || textProperty.PropertyType != typeof(string) || !textProperty.CanWrite)
            {
                continue;
            }

            textProperty.SetValue(component, buttonLabel, null);
        }
    }

    public void ToggleWindowMode()
    {
        if (IsWindowed())
        {
            SetWindowMaximized();
        }
        else
        {
            SetWindowedMode();
        }
    }

    public void SetWindowMaximized()
    {
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        Screen.fullScreen = true;
    }

    public void SetWindowedMode()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.fullScreen = false;
    }

    public bool IsWindowed()
    {
        return Screen.fullScreenMode == FullScreenMode.Windowed || !Screen.fullScreen;
    }

    public void TriggerVictoryEffectsTest()
    {
        PlayVictoryEffects();
    }

    public void NewGame()
    {
        hasPlayedVictoryEffectsThisGame = false;
        
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
            if (!hasPlayedVictoryEffectsThisGame)
            {
                PlayVictoryEffects();
                hasPlayedVictoryEffectsThisGame = true;
            }

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

    private void PlayVictoryEffects()
    {
        if (victoryEffects == null)
        {
            victoryEffects = FindObjectOfType<VictoryEffectsController>();
        }

        if (victoryEffects == null)
        {
            GameObject effectsObject = new GameObject("VictoryEffectsController");
            victoryEffects = effectsObject.AddComponent<VictoryEffectsController>();
        }

        if (victoryEffects != null)
        {
            victoryEffects.PlayVictoryCelebration();
        }
    }

}
	