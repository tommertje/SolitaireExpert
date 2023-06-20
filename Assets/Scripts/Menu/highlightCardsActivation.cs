using UnityEngine;
using System.Collections;

public class highlightCardsActivation : MonoBehaviour {
	public GameObject GameControllerScriptReference;
	public GameObject highlightInitiationReference;

	void Start () {
		//this is not necessary, already done in the Editor
		//gameObjectReference = GameObject.Find("GameController");
	}

	void OnMouseDown() {
		//the objects must be called 'highlightCardTWO' , 'highlightCardFOUR' etc. to parse the rank -> overstappen op TAG ?
		Rank highlightCardRank = (Rank)System.Enum.Parse( typeof( Rank ), this.name.Substring (13, this.name.Length - 13) );
		GameControllerScriptReference.GetComponent<GameController> ().HighlightCards(highlightCardRank /* , CardColor.BLACK */);
		//highlightInitiationReference.GetComponent<highlightInitiation> ().SetVisibilityHighlightCards (false);
	}

    private void OnMouseUp()
    {
       
    }
}