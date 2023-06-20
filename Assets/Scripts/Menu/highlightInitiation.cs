using UnityEngine;
using System.Collections;

public class highlightInitiation : MonoBehaviour {

    [SerializeField] Animator highlightCardSlider;
    [SerializeField] Animator menuSlider;

	void OnMouseDown() {
		SetVisibilityHighlightCards (true);
    }

    private void OnDisable()
    {
       // SetVisibilityHighlightCards(false);
    }

    public void SetVisibilityHighlightCards(bool Show) {
        if(Show)
        {
            highlightCardSlider.Play("SlideIn");
            menuSlider.Play("SlideOut");
        }
        else
        {
            highlightCardSlider.Play("SlideOut");
            menuSlider.Play("SlideIn");
        }
    }
}
