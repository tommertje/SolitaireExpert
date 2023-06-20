using UnityEngine;
using System.Collections;

public class newGameScript : MonoBehaviour {

	public GameObject GameControllerScriptReference;

	void OnMouseDown() {
		GameControllerScriptReference.GetComponent<GameController> ().NewGame();
	}
}
